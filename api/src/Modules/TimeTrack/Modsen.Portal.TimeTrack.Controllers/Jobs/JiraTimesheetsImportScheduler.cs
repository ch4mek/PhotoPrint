using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Jobs;

internal sealed class JiraTimesheetsImportScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "ImportJiraTimesheets";

    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IConfiguration _configuration;
    private readonly IClockService _clockService;
    private readonly IServiceProvider _serviceProvider;

    public JiraTimesheetsImportScheduler(
        IBackgroundJobManager backgroundJobManager,
        IConfiguration configuration,
        IClockService clockService,
        IServiceProvider serviceProvider)
    {
        _backgroundJobManager = backgroundJobManager;
        _configuration = configuration;
        _clockService = clockService;
        _serviceProvider = serviceProvider;
    }

    public void Schedule()
    {
        var queueName = _configuration["AppSettings:JiraSettings:TimesheetsImportQueue"]!;

        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => ImportJiraTimesheets(),
            cronExpression: () => "0 1 * * *");
    }

    public async Task ImportJiraTimesheets()
    {
        var yesterday = _clockService.TodayUtc().AddDays(-1);
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new ImportJiraTimesheetRequest.Command(yesterday));
    }
}
