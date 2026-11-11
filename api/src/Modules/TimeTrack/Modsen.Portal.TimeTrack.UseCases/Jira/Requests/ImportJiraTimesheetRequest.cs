using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Jira.Contract;
using Modsen.Portal.Jira.Contract.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class ImportJiraTimesheetRequest : ICommandHandler<ImportJiraTimesheetRequest.Command, Unit>
{
    private readonly ITimeTrackDbContext _dbContext;
    private readonly IIdentityContract _identityContract;
    private readonly IConfiguration _configuration;
    private readonly IDataProtectionService _dataProtectionService;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IJiraService _jiraService;
    private readonly IJiraTimesheetImportComposer _jiraTimesheetImportComposer;
    private readonly ILogger<ImportJiraTimesheetRequest> _logger;
    private readonly IConnectionFactory _connectionFactory;

    public ImportJiraTimesheetRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IIdentityContract identityContract,
        IConfiguration configuration,
        IJiraSettingsDataProtectionFactory protectionFactory,
        IBackgroundJobManager backgroundJobManager,
        IJiraService jiraService,
        IJiraTimesheetImportComposer jiraTimesheetImportComposer,
        ILogger<ImportJiraTimesheetRequest> logger,
        IConnectionFactory connectionFactory)
    {
        _dataProtectionService = protectionFactory.Create();
        _identityContract = identityContract;
        _jiraTimesheetImportComposer = jiraTimesheetImportComposer;
        _jiraService = jiraService;
        _dbContext = timeTrackDbContext;
        _logger = logger;
        _configuration = configuration;
        _backgroundJobManager = backgroundJobManager;
        _connectionFactory = connectionFactory;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var userIds = await _identityContract.GetUsersByClaim(JiraClaimTypeNames.ViewJiraImport.Name, cancellationToken);

        var jiraSettingIds = await _dbContext.JiraSettings
            .Where(x => x.IsActive)
            .Where(x => x.User!.IsActive)
            .Where(x => userIds.Contains(x.UserId))
            .Where(x => x.Project!.IsActive)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var queueName = _configuration["AppSettings:JiraSettings:OneTimesheetImportQueue"]!;

        foreach (var jiraSettingId in jiraSettingIds)
        {
            _backgroundJobManager.Schedule(
                queueName,
                () => ImportTimesheets(
                    jiraSettingId, request.Date,
                    cancellationToken));
        }

        return Unit.Value;
    }

    public async Task ImportTimesheets(int jiraSettingId, DateOnly date, CancellationToken cancellationToken)
    {
        var jiraSetting = await _dbContext.JiraSettings
            .FirstOrDefaultAsync(x => x.Id == jiraSettingId, cancellationToken)
            ?? throw new EntityNotFoundException($"Jira setting with id {jiraSettingId} not found");

        var pat = _dataProtectionService.Decrypt(jiraSetting.Pat);
        if (string.IsNullOrWhiteSpace(pat))
        {
            _logger.LogError("Empty PAT for user {UserId} and project {ProjectId}", jiraSetting.UserId, jiraSetting.ProjectId);
            return;
        }

        IReadOnlyCollection<JiraImportedWorklog> worklog;

        try
        {
            worklog = await _jiraService.GetWorklogsAsync(
                jiraSetting.Url,
                jiraSetting.JiraProjectTag,
                pat,
                jiraSetting.Email,
                date,
                date,
                cancellationToken);
        }
        catch (Exception ex) when (ex is not JiraTemporarilyUnavailableException)
        {
            _logger.LogError(ex, "User {UserId}, project {ProjectId}: Jira import failed", jiraSetting.UserId, jiraSetting.ProjectId);
            return;
        }

        try
        {
            var result = await _jiraTimesheetImportComposer.Compose(
                worklog,
                jiraSetting.UserId,
                jiraSetting.ProjectId,
                date,
                date,
                cancellationToken);

            await _dbContext.SaveChanges(cancellationToken);
            await _connectionFactory.CommitCurrentTransaction(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {UserId}, project {ProjectId}: Created {Created}, updated {Updated}, skipped {Skipped}, total {Total}",
                jiraSetting.UserId, jiraSetting.ProjectId, result.Created, result.Updated, result.Skipped, result.Total);
            }
        }
        catch (ActionNotAllowedException ex)
        {
            _logger.LogError(ex, "User {UserId}, project {ProjectId}: Import not allowed", jiraSetting.UserId, jiraSetting.ProjectId);
        }
    }

    internal sealed record Command(DateOnly Date) : ICommand<Unit>, ITransactionalRequest;
}
