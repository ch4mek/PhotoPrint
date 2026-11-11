using System.Globalization;
using System.Net.Mime;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Cache;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Options;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class ProcessReportRequest : ICommandHandler<ProcessReportRequest.Command, ProcessReportStatusDto>
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _userContext;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ReportProcessingOptions _options;
    private readonly IReportNameComposer _reportNameComposer;
    private readonly IReportQuerySelector _reportQuerySelector;
    private readonly ICanUserReportBeExportedBusinessRule _canUserReportBeExportedBusinessRule;
    private readonly IServiceProvider _serviceProvider;
    private readonly IStorageContract _storageContract;
    private readonly IClockService _clockService;
    private readonly IHashService _hashService;
    private readonly IReportGenerationQueueCache _reportGenerationQueueCache;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public ProcessReportRequest(
        ISender sender,
        IAuthenticatedUserContext userContext,
        IBackgroundJobManager backgroundJobManager,
        IOptions<ReportProcessingOptions> options,
        IReportNameComposer reportNameComposer,
        IReportQuerySelector reportQuerySelector,
        ICanUserReportBeExportedBusinessRule canUserReportBeExportedBusinessRule,
        IServiceProvider serviceProvider,
        IStorageContract storageContract,
        IClockService clockService,
        IHashService hashService,
        IReportGenerationQueueCache reportGenerationQueueCache,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _sender = sender;
        _userContext = userContext;
        _backgroundJobManager = backgroundJobManager;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _reportNameComposer = reportNameComposer;
        _reportQuerySelector = reportQuerySelector;
        _canUserReportBeExportedBusinessRule = canUserReportBeExportedBusinessRule;
        _serviceProvider = serviceProvider;
        _storageContract = storageContract;
        _clockService = clockService;
        _hashService = hashService;
        _reportGenerationQueueCache = reportGenerationQueueCache;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<ProcessReportStatusDto> Handle(Command command, CancellationToken cancellationToken)
    {
        if (command.UserId is not null)
        {
            var result = await _canUserReportBeExportedBusinessRule
                .IsCorrespondsToBusiness(command.UserId.Value, cancellationToken);

            if (!result.IsSuccess)
            {
                throw new ActionNotAllowedException(result.Reason!);
            }
        }

        if (command.ProjectId is not null)
        {
            var projectName = await _timeTrackDbContext.Projects
                .Where(project => project.Id == command.ProjectId.Value)
                .Select(project => project.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (projectName is null)
            {
                throw new EntityNotFoundException($"Project with id {command.ProjectId.Value} does not exist");
            }
        }

        var timesheetsFilter = new TimesheetFilter
        {
            UserId = command.UserId,
            DepartmentId = command.DepartmentId,
            ResourceGroupId = command.ResourceGroupId,
            ClientId = command.ClientId,
            ProjectId = command.ProjectId,
            TrackedTimeType = command.TrackedTimeType,
        };

        var userIds = await _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .FilterByPeriod(command.Year, command.MonthOfYear, command.WeekOfYear)
            .ApplyTimesheetFilter(timesheetsFilter)
            .Select(t => t.UserId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        var userId = command.UserId;
        if (userId is null && userIds.Length == 1)
        {
            userId = userIds.FirstOrDefault();
        }

        string reportFileNameDto;

        if (command.IsPerUserExport && userId is null)
        {
            reportFileNameDto = await _reportNameComposer.ComposeForPerUserArchiveAsync(command, cancellationToken);
        }
        else
        {
            reportFileNameDto = await _reportNameComposer.ComposeForSingleFileAsync(command, cancellationToken);
        }

        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = _hashService.GetSha256Hash(filterArgs);

        var queryModel = new GetTrackedTimeToExportQueryModel(
            command.Year,
            command.MonthOfYear,
            command.WeekOfYear,
            command.DepartmentId,
            command.ClientId,
            userId,
            command.ProjectId,
            command.ResourceGroupId,
            reportFileNameDto,
            command.TrackedTimeType,
            command.Format,
            command.IsPerUserExport);

        var query = _reportQuerySelector.GetQueryByParameters(queryModel);

        var currentUserId = _userContext.UserId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;

        var userRoles = _userContext.GetRoles();

        if (userRoles.IsEmployee() || userRoles.IsSystemAdministrator())
        {
            var response = await _timeTrackDbContext.ReportRequests
                .Where(reportRequest => reportRequest.FilterHash == filterHash &&
                                        reportRequest.RequestIsValidTill > _clockService.UtcNow &&
                                        reportRequest.RequestedById == _userContext.GetRequiredUserId()
                )
                .Select(reportRequest => new
                {
                    FileId = reportRequest.ReportFileId,
                    reportRequest.SizeInKilobytes
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (response is not null)
            {
                using var scope = _serviceProvider.CreateScope();

                var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
                var communicationBus = scope.ServiceProvider.GetRequiredService<ICommunicationBus>();

                var reportAlreadyGeneratedNotification = GetReportGeneratedNotification(
                    command,
                    userId,
                    response.FileId,
                    reportFileNameDto,
                    response.SizeInKilobytes,
                    _userContext.GetRequiredUserId());

                await communicationBus.PublishAsync(reportAlreadyGeneratedNotification, cancellationToken);

                await connectionFactory.CommitCurrentTransaction(cancellationToken);

                return new ProcessReportStatusDto
                {
                    ToastType = "Info",
                    Body = "The same report has been already generated. Please see the notifications"
                };
            }
        }

        if (_reportGenerationQueueCache.Exists(filterHash, currentUserId))
        {
            throw new EntityConflictException("The same report is being generated by the moment");
        }

        _reportGenerationQueueCache.Set(filterHash, currentUserId);

        ArgumentException.ThrowIfNullOrEmpty(_options.QueueName);

        _backgroundJobManager.Schedule(_options.QueueName, () => ExecuteReportGeneration(
            command,
            userId,
            filterArgs,
            filterHash,
            query,
            reportFileNameDto,
            _userContext.GetRequiredUserId(),
            cancellationToken));

        return new ProcessReportStatusDto
        {
            ToastType = "Success",
            Body = "Report file is being generated. You will be notified once the process is completed"
        };
    }

    public async Task ExecuteReportGeneration(
        Command command,
        int? userId,
        string filterArgs,
        string filterHash,
        IQuery<byte[]> query,
        string fileName,
        int userIdFor,
        CancellationToken cancellationToken)
    {
        var notificationCreatedEvents = new List<NotificationsCreatedEvent>();

        try
        {
            await PublishReportGenerating(command, userIdFor, fileName, cancellationToken);

            var content = await GenerateReportAsync(query, cancellationToken);

            var contentSizeInKilobytes = content.LongLength / 1024;

            var fileId = await UploadFile(command, userId, content, contentSizeInKilobytes, fileName,
                filterArgs, filterHash, userIdFor, cancellationToken);

            var reportGeneratedNotification = GetReportGeneratedNotification(command, userId, fileId,
               fileName, contentSizeInKilobytes, userIdFor);

            notificationCreatedEvents.Add(reportGeneratedNotification);
        }
        catch
        {
            var failNotification = GetReportGenerationFailedNotification(command, fileName, userIdFor);

            notificationCreatedEvents.Add(failNotification);
        }
        finally
        {
            using var scope = _serviceProvider.CreateScope();

            var communicationBus = scope.ServiceProvider.GetRequiredService<ICommunicationBus>();
            var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

            foreach (var @event in notificationCreatedEvents)
            {
                await communicationBus.PublishAsync(@event, cancellationToken);
            }

            await connectionFactory.CommitCurrentTransaction(cancellationToken);

            _reportGenerationQueueCache.Remove(filterHash, userIdFor.ToString(CultureInfo.InvariantCulture));
        }
    }

    private async Task PublishReportGenerating(
        Command command,
        int userId,
        string fileName,
        CancellationToken cancellationToken)
    {
        var reportGeneratingNotification = GetReportGeneratingNotification(command, fileName, userId);

        using var scope = _serviceProvider.CreateScope();

        var communicationBus = scope.ServiceProvider.GetRequiredService<ICommunicationBus>();
        var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

        await communicationBus.PublishAsync(reportGeneratingNotification, cancellationToken);

        await connectionFactory.CommitCurrentTransaction(cancellationToken);
    }

    private async Task<string> UploadFile(
        Command command,
        int? userId,
        byte[] content,
        double contentSizeInKilobytes,
        string fileName,
        string filterArgs,
        string filterHash,
        int userIdFor,
        CancellationToken cancellationToken)
    {
        var availableTill = _clockService.UtcNow.AddSeconds(_options.FileAccessibilityInSeconds);
        var mimeTypes = new Dictionary<DocumentType, string>
        {
            { DocumentType.Pdf, MediaTypeNames.Application.Pdf },
            { DocumentType.Word, PortalMediaTypeNames.Word },
            { DocumentType.Excel, PortalMediaTypeNames.Excel },
            { DocumentType.ExcelAccount, PortalMediaTypeNames.Excel },
        };

        var fileId = string.Empty;

        using (var _ = _serviceProvider.CreateScope())
        {
            fileId = await _storageContract.UploadFile(new UploadFileDto
            {
                FileContent = content,
                FileName = fileName,
                FileSize = content.LongLength,
                UserId = userIdFor,
                AvailableTill = availableTill,
                ContentType = command.IsPerUserExport && userId is null
                    ? PortalMediaTypeNames.ZipResponse
                    : mimeTypes.GetValueOrDefault(command.Format, MediaTypeNames.Application.Octet),
                FileSource = FileSource.System,
            }, cancellationToken);
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbRequest = new CreateReportRequestDbRequest(fileName, contentSizeInKilobytes, availableTill, userIdFor,
                fileId, filterHash, filterArgs);

            await scope.ServiceProvider.GetRequiredService<ICreateReportRequestDbQuery>()
                .ExecuteAsync(dbRequest, cancellationToken);
            await scope.ServiceProvider.GetRequiredService<IConnectionFactory>()
                .CommitCurrentTransaction(cancellationToken);
        }

        return fileId;
    }

    private async Task<byte[]> GenerateReportAsync(IQuery<byte[]> query, CancellationToken cancellationToken)
    {
        var contentGeneration = _sender.Send(query, cancellationToken);

        var delayTask = Task.Delay(_options.ReportGenerationTimeoutInSeconds * 1000, cancellationToken);

        if (await Task.WhenAny(contentGeneration, delayTask) == delayTask)
        {
            throw new TimeoutException();
        }

        return await contentGeneration;
    }

    private static NotificationsCreatedEvent GetReportGeneratingNotification(Command command,
        string fileName, int userId)
    {
        var reportGeneratingOutputJson = new TimeTrackReportGeneratingNotificationOutputDto
        {
            ReportName = fileName,
            DocumentType = command.Format.ToString()
        };

        var notification = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserIds = new List<int> { userId },
            Arguments = JsonSerializer.Serialize(reportGeneratingOutputJson),
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = NotificationTypes.TimeTrackReportGenerating
        };

        return notification;
    }

    private static NotificationsCreatedEvent GetReportGenerationFailedNotification(Command command,
        string fileName, int userId)
    {
        var reportGeneratingOutputJson = new TimeTrackReportGenerationFailedNotificationOutputDto
        {
            ReportName = fileName,
            DocumentType = command.Format.ToString()
        };

        var notification = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserIds = new List<int> { userId },
            Arguments = JsonSerializer.Serialize(reportGeneratingOutputJson),
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = NotificationTypes.TimeTrackReportGenerationFailed
        };

        return notification;
    }

    private static NotificationsCreatedEvent GetReportGeneratedNotification(
        Command command,
        int? userId,
        string? fileId,
        string fileName,
        double sizeInKilobytes,
        int userIdFor)
    {
        var reportCreatedOutputJson = new TimeTrackReportCreatedNotificationOutputDto
        {
            ReportName = fileName,
            ReportFileId = fileId,
            DocumentType = command.IsPerUserExport && userId is null
                ? "Archive"
                : command.Format.ToString(),
            SizeInKilobytes = sizeInKilobytes
        };

        var notification = new NotificationsCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserIds = new List<int> { userIdFor },
            Arguments = JsonSerializer.Serialize(reportCreatedOutputJson),
            NotificationChannel = NotificationChannels.SignalR,
            NotificationType = NotificationTypes.TimeTrackReportCreated
        };

        return notification;
    }

    internal sealed record Command(
        int? Year,
        int[] MonthOfYear,
        int? WeekOfYear,
        string? DepartmentId,
        int? ClientId,
        int? UserId,
        int? ProjectId,
        int? ResourceGroupId,
        TrackedTimeType TrackedTimeType,
        DocumentType Format,
        bool IsPerUserExport)
        : ICommand<ProcessReportStatusDto>;
}
