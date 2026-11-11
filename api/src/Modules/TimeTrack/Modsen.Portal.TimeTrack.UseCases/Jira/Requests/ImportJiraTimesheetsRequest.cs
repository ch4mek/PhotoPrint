using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class ImportJiraTimesheetsRequest :
    ICommandHandler<ImportJiraTimesheetsRequest.Command, JiraImportResultDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IDataProtectionService _dataProtectionService;
    private readonly IJiraTimesheetImportComposer _jiraTimesheetImportComposer;
    private readonly ITimeTrackDbContext _dbContext;
    private readonly IJiraService _jiraService;
    private readonly ILogger<ImportJiraTimesheetsRequest> _logger;

    public ImportJiraTimesheetsRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IJiraSettingsDataProtectionFactory protectionFactory,
        IJiraTimesheetImportComposer jiraTimesheetImportComposer,
        ITimeTrackDbContext dbContext,
        IJiraService jiraService,
        ILogger<ImportJiraTimesheetsRequest> logger)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _dataProtectionService = protectionFactory.Create();
        _jiraTimesheetImportComposer = jiraTimesheetImportComposer;
        _dbContext = dbContext;
        _jiraService = jiraService;
        _logger = logger;
    }

    public async Task<JiraImportResultDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserContext.GetRequiredUserId();

        var jiraSettings = await _dbContext.JiraSettings
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .Where(x => x.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);

        if (jiraSettings.Count == 0)
        {
            throw new EntityNotFoundException(PortalMessages.JiraSettingNotFound);
        }

        var successfulAttemptsCount = 0;

        var worklogs = new List<JiraImportedWorklog>();

        var exceptions = new List<string>();

        foreach (var jiraSetting in jiraSettings)
        {
            try
            {
                var pat = _dataProtectionService.Decrypt(jiraSetting.Pat);

                if (string.IsNullOrWhiteSpace(pat))
                {
                    throw new InvalidDataAppException();
                }

                await _jiraService.CheckAccessAsync(jiraSetting.Url, jiraSetting.JiraProjectTag, pat, jiraSetting.Email, cancellationToken);

                var worklogsByJiraSetting = await _jiraService.GetWorklogsAsync(
                    jiraSetting.Url,
                    jiraSetting.JiraProjectTag,
                    pat,
                    jiraSetting.Email,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

                worklogs.AddRange(worklogsByJiraSetting);

                successfulAttemptsCount++;
            }
            catch (Exception ex) when (
                ex is HttpRequestException ||
                ex is InvalidDataAppException ||
                ex is EntityNotFoundException ||
                ex is ArgumentNullException ||
                ex is ArgumentException)
            {
                var message = $"Failed to fetch worklogs from Jira setting {jiraSetting.Id} with exception: {ex.Message}. Skipping this source.";

                exceptions.Add(message);
                _logger.LogWarning(ex, message);
            }
        }

        if (successfulAttemptsCount == 0)
        {
            throw new AppException($"Failed to import worklogs from Jira for Project Id: {request.ProjectId}.");
        }

        var result = await _jiraTimesheetImportComposer.Compose(
            worklogs,
            userId,
            request.ProjectId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        result.ExceptionMessages = exceptions;

        return result;
    }

    internal sealed record Command(int ProjectId, DateOnly StartDate, DateOnly EndDate) :
        ICommand<JiraImportResultDto>,
        ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).GreaterThan(0);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThanOrEqualTo(x => x.StartDate)
                .Must((x, endDate) => endDate <= x.StartDate.AddYears(1))
                .WithMessage("The date range is invalid");
        }
    }
}
