using System.Linq.Expressions;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Cache;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Options;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class ProcessReportRequestTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly Mock<IAuthenticatedUserContext> _userContextMock;
    private readonly Mock<IBackgroundJobManager> _backgroundJobManagerMock;
    private readonly Mock<IReportNameComposer> _reportNameComposerMock;
    private readonly Mock<IReportQuerySelector> _reportQuerySelectorMock;
    private readonly Mock<ICanUserReportBeExportedBusinessRule> _canUserReportBeExportedBusinessRuleMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IStorageContract> _storageContractMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly Mock<IHashService> _hashServiceMock;
    private readonly Mock<IReportGenerationQueueCache> _reportGenerationQueueCacheMock;
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock;
    private readonly IOptions<ReportProcessingOptions> _options;
    private readonly ProcessReportRequest _useCase;
    private readonly CancellationToken _cancellationToken;

    public ProcessReportRequestTests()
    {
        _senderMock = new Mock<ISender>();
        _userContextMock = new Mock<IAuthenticatedUserContext>();
        _backgroundJobManagerMock = new Mock<IBackgroundJobManager>();
        _reportNameComposerMock = new Mock<IReportNameComposer>();
        _reportQuerySelectorMock = new Mock<IReportQuerySelector>();
        _canUserReportBeExportedBusinessRuleMock = new Mock<ICanUserReportBeExportedBusinessRule>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _storageContractMock = new Mock<IStorageContract>();
        _clockServiceMock = new Mock<IClockService>();
        _hashServiceMock = new Mock<IHashService>();
        _reportGenerationQueueCacheMock = new Mock<IReportGenerationQueueCache>();
        _timeTrackDbContextMock = new Mock<ITimeTrackDbContext>();
        _options = Options.Create(new ReportProcessingOptions
        {
            QueueName = "queue",
            FileAccessibilityInSeconds = 3600,
            ReportGenerationTimeoutInSeconds = 30
        });
        _cancellationToken = CancellationToken.None;

        _useCase = new ProcessReportRequest(
            _senderMock.Object,
            _userContextMock.Object,
            _backgroundJobManagerMock.Object,
            _options,
            _reportNameComposerMock.Object,
            _reportQuerySelectorMock.Object,
            _canUserReportBeExportedBusinessRuleMock.Object,
            _serviceProviderMock.Object,
            _storageContractMock.Object,
            _clockServiceMock.Object,
            _hashServiceMock.Object,
            _reportGenerationQueueCacheMock.Object,
            _timeTrackDbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WhenReportCannotBeExported_ThrowsActionNotAllowedException()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(command.UserId!.Value, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("User cannot export report"));

        // Act
        var action = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage("User cannot export report");
    }

    [Fact]
    public async Task Handle_WhenProjectDoesNotExist_ThrowsEntityNotFoundException()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: null,
            ProjectId: 1,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        _timeTrackDbContextMock.Setup(x => x.Projects)
            .Returns(new List<Project>().BuildMockDbSet().Object);

        // Act
        var action = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("Project with id 1 does not exist");
    }

    [Fact]
    public async Task Handle_WhenSingleUserIdFromTimesheets_AssignsUserIdAndSchedulesJob()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: null,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";

        var fileId = "file123";
        var sizeInKilobytes = 100.0;
        var validTill = DateTime.UtcNow.AddDays(1);

        var userId = 1;
        var timesheets = new List<Timesheet>
        {
            new() { Id = 1, UserId = userId, TaskName = "Test 1" }
        };
        _timeTrackDbContextMock.Setup(x => x.Timesheets)
            .Returns(timesheets.BuildMockDbSet().Object);

        _timeTrackDbContextMock.Setup(x => x.ReportRequests)
            .Returns(new List<ReportRequest>
            {
                new()
                {
                    FilterHash = filterHash,
                    RequestedById = userId,
                    RequestIsValidTill = validTill,
                    ReportFileId = fileId,
                    SizeInKilobytes = sizeInKilobytes
                }
            }.BuildMockDbSet().Object);

        var reportFileNameDto = "Report_2024_Week20.pdf";
        _reportNameComposerMock.Setup(x =>
                x.ComposeForSingleFileAsync(command, _cancellationToken))
            .ReturnsAsync(reportFileNameDto);

        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

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
        var queryMock = new Mock<IQuery<byte[]>>();
        _reportQuerySelectorMock.Setup(x => x.GetQueryByParameters(queryModel))
            .Returns(queryMock.Object);

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _userContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _reportGenerationQueueCacheMock.Setup(x => x.Exists(filterHash, userId.ToString()))
            .Returns(false);

        // Act
        var result = await _useCase.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(new ProcessReportStatusDto
        {
            ToastType = "Success",
            Body = "Report file is being generated. You will be notified once the process is completed"
        });

        _backgroundJobManagerMock.Verify(x => x.Schedule(_options.Value.QueueName,
            It.IsAny<Expression<Func<Task>>>()), Times.Once());
        _reportGenerationQueueCacheMock.Verify(x => x.Set(filterHash, userId.ToString()), Times.Once());
    }

    [Fact]
    public async Task Handle_WhenReportAlreadyExists_ReturnsInfoStatus()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var userId = 1;
        var fileId = "file123";
        var fileName = "Report_2024_Week20.pdf";
        var sizeInKilobytes = 100.0;
        var validTill = DateTime.UtcNow.AddDays(1);

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _reportNameComposerMock.Setup(x =>
                x.ComposeForSingleFileAsync(command, _cancellationToken))
            .ReturnsAsync(fileName);

        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";
        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _userContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.Employee]);

        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _timeTrackDbContextMock.Setup(x => x.ReportRequests)
            .Returns(new List<ReportRequest>
            {
                new()
                {
                    FilterHash = filterHash,
                    RequestedById = userId,
                    RequestIsValidTill = validTill,
                    ReportFileId = fileId,
                    SizeInKilobytes = sizeInKilobytes
                }
            }.BuildMockDbSet().Object);

        _timeTrackDbContextMock.Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>
            {
                new() { Id = 1, UserId = userId , TaskName = "Test 1"}
            }.BuildMockDbSet().Object);

        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        var communicationBusMock = new Mock<ICommunicationBus>();
        var connectionFactoryMock = new Mock<IConnectionFactory>();
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(ICommunicationBus)))
            .Returns(communicationBusMock.Object);
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(IConnectionFactory)))
            .Returns(connectionFactoryMock.Object);

        // Act
        var result = await _useCase.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(new ProcessReportStatusDto
        {
            ToastType = "Info",
            Body = "The same report has been already generated. Please see the notifications"
        });

        communicationBusMock.Verify(x => x.PublishAsync(It.IsAny<NotificationsCreatedEvent>(), _cancellationToken), Times.Once());
        connectionFactoryMock.Verify(x => x.CommitCurrentTransaction(_cancellationToken), Times.Once());
        _backgroundJobManagerMock.Verify(x => x.Schedule(It.IsAny<string>(), It.IsAny<Expression<Func<Task>>>()), Times.Never());
    }

    [Fact]
    public async Task Handle_WhenReportInGenerationQueue_ThrowsEntityConflictException()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var userId = 1;
        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";
        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

        _timeTrackDbContextMock.Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>
            {
                new() { Id = 1, UserId = userId, TaskName = "Test 1" }
            }.BuildMockDbSet().Object);

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _userContextMock.Setup(context => context.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _reportGenerationQueueCacheMock.Setup(x => x.Exists(filterHash, userId.ToString()))
            .Returns(true);

        _reportNameComposerMock.Setup(x =>
                x.ComposeForSingleFileAsync(command, _cancellationToken))
            .ReturnsAsync("Report_2024_Week20.pdf");

        // Act
        var action = () => _useCase.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityConflictException>()
            .WithMessage("The same report is being generated by the moment");
        _backgroundJobManagerMock.Verify(x => x.Schedule(It.IsAny<string>(), It.IsAny<Expression<Func<Task>>>()), Times.Never());
    }

    [Fact]
    public async Task ExecuteReportGeneration_WhenSuccessful_UploadsFileAndSendsNotification()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var userId = 1;
        var fileName = "Report_2024_Week20.pdf";
        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";
        var fileId = "file123";
        var content = new byte[] { 1, 2, 3 };
        var availableTill = DateTime.UtcNow.AddSeconds(_options.Value.FileAccessibilityInSeconds);

        var queryMock = new Mock<IQuery<byte[]>>();
        _senderMock.Setup(x => x.Send(queryMock.Object, _cancellationToken))
            .ReturnsAsync(content);

        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _storageContractMock
            .Setup(x => x.UploadFile(
                It.Is<UploadFileDto>(dto =>
                    dto.FileContent.ToArray().SequenceEqual(content) &&
                    dto.FileName == fileName &&
                    dto.FileSize == content.Length &&
                    dto.UserId == userId &&
                    dto.AvailableTill == availableTill),
                _cancellationToken))
            .ReturnsAsync(fileId);

        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        var communicationBusMock = new Mock<ICommunicationBus>();
        var connectionFactoryMock = new Mock<IConnectionFactory>();
        var createReportRequestDbQueryMock = new Mock<ICreateReportRequestDbQuery>();
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(ICommunicationBus)))
            .Returns(communicationBusMock.Object);
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(IConnectionFactory)))
            .Returns(connectionFactoryMock.Object);
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(ICreateReportRequestDbQuery)))
            .Returns(createReportRequestDbQueryMock.Object);

        // Act
        await _useCase.ExecuteReportGeneration(command, userId, filterArgs, filterHash, queryMock.Object, fileName, userId, _cancellationToken);

        // Assert
        _storageContractMock.Verify(x => x.UploadFile(
            It.IsAny<UploadFileDto>(), _cancellationToken), Times.Once());
        createReportRequestDbQueryMock.Verify(x => x.ExecuteAsync(It.IsAny<CreateReportRequestDbRequest>(), _cancellationToken), Times.Once());
        communicationBusMock.Verify(x => x.PublishAsync(It.Is<NotificationsCreatedEvent>(e => e.NotificationType == NotificationTypes.TimeTrackReportCreated), _cancellationToken), Times.Once());
        communicationBusMock.Verify(x => x.PublishAsync(It.Is<NotificationsCreatedEvent>(e => e.NotificationType == NotificationTypes.TimeTrackReportGenerating), _cancellationToken), Times.Once());
        _reportGenerationQueueCacheMock.Verify(x => x.Remove(filterHash, userId.ToString()), Times.Once());
    }

    [Fact]
    public async Task ExecuteReportGeneration_WhenGenerationFails_SendsFailureNotification()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var userId = 1;
        var fileName = "Report_2024_Week20.pdf";
        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";

        var queryMock = new Mock<IQuery<byte[]>>();
        _senderMock.Setup(x => x.Send(queryMock.Object, _cancellationToken))
            .ThrowsAsync(new Exception("Generation failed"));

        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);

        var communicationBusMock = new Mock<ICommunicationBus>();
        var connectionFactoryMock = new Mock<IConnectionFactory>();
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(ICommunicationBus)))
            .Returns(communicationBusMock.Object);
        scopeMock.Setup(x => x.ServiceProvider.GetService(typeof(IConnectionFactory)))
            .Returns(connectionFactoryMock.Object);

        // Act
        await _useCase.ExecuteReportGeneration(command, userId, filterArgs, filterHash, queryMock.Object, fileName, userId, _cancellationToken);

        // Assert
        communicationBusMock.Verify(x => x.PublishAsync(It.Is<NotificationsCreatedEvent>(e => e.NotificationType == NotificationTypes.TimeTrackReportGenerationFailed), _cancellationToken), Times.Once());
        communicationBusMock.Verify(x => x.PublishAsync(It.Is<NotificationsCreatedEvent>(e => e.NotificationType == NotificationTypes.TimeTrackReportGenerating), _cancellationToken), Times.Once());
        _reportGenerationQueueCacheMock.Verify(x => x.Remove(filterHash, userId.ToString()), Times.Once());
        _storageContractMock.Verify(x => x.UploadFile(It.IsAny<UploadFileDto>(), _cancellationToken), Times.Never());
    }

    [Fact]
    public async Task Handle_WhenNonEmployeeRole_SkipsExistingReportCheck()
    {
        // Arrange
        var command = new ProcessReportRequest.Command(
            Year: 2024,
            MonthOfYear: [],
            WeekOfYear: 20,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ProjectId: null,
            ResourceGroupId: null,
            TrackedTimeType: TrackedTimeType.Billable,
            Format: DocumentType.Pdf,
            IsPerUserExport: false);

        var userId = 1;
        var reportFileNameDto = "Report_2024_Week20.pdf";
        var filterArgs = JsonSerializer.Serialize(command);
        var filterHash = "hash123";

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(userId, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        _reportNameComposerMock.Setup(x =>
                x.ComposeForSingleFileAsync(command, _cancellationToken))
            .ReturnsAsync(reportFileNameDto);

        _hashServiceMock.Setup(x => x.GetSha256Hash(filterArgs))
            .Returns(filterHash);

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _userContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _userContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

        _timeTrackDbContextMock.Setup(x => x.Timesheets)
            .Returns(new List<Timesheet>
            {
                new() { Id = 1, UserId = userId, TaskName = "Test 1" }
            }.BuildMockDbSet().Object);

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
        var queryMock = new Mock<IQuery<byte[]>>();
        _reportQuerySelectorMock.Setup(x => x.GetQueryByParameters(queryModel))
            .Returns(queryMock.Object);

        _reportGenerationQueueCacheMock.Setup(x => x.Exists(filterHash, userId.ToString()))
            .Returns(false);

        // Act
        var result = await _useCase.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(new ProcessReportStatusDto
        {
            ToastType = "Success",
            Body = "Report file is being generated. You will be notified once the process is completed"
        });

        _timeTrackDbContextMock.Verify(x => x.ReportRequests, Times.Never());
        _backgroundJobManagerMock.Verify(x => x.Schedule(_options.Value.QueueName,
            It.IsAny<Expression<Func<Task>>>()), Times.Once());
        _reportGenerationQueueCacheMock.Verify(x => x.Set(filterHash, userId.ToString()), Times.Once());
    }
}
