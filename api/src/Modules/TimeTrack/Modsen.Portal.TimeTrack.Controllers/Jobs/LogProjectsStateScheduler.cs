using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.Controllers.Jobs;

internal sealed class LogProjectsStateScheduler : IRecurringJobScheduler
{

    private const string s_jobId = "ShowDashboard";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ILogger _logger;
    private readonly IEnumerable<IOutboxEventRepository> _outboxEventRepositories;

    public LogProjectsStateScheduler(IBackgroundJobManager backgroundJobManager,
        ILogger<LogProjectsStateScheduler> logger,
        IEnumerable<IOutboxEventRepository> outboxEventRepositories)
    {
        _backgroundJobManager = backgroundJobManager;
        _logger = logger;
        _outboxEventRepositories = outboxEventRepositories;
    }

    public void Schedule()
    {
        var queueName = "default";

        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => LogState(),
            cronExpression: () => "0 0 30 2 *");
    }

    public async Task LogState()
    {
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var cancellationToken = cancellationTokenSource.Token;

        foreach (var repository in _outboxEventRepositories)
        {
            var scope = repository.StorageScope;
            var countOfUndelivered = await repository.GetCountOfUndeliveredEvents(cancellationToken);

            _logger.LogInformation("Total pending outbox events for {Scope}: {CountOfUndelivered}", scope, countOfUndelivered);

            if (countOfUndelivered > 0)
            {
                var details = await repository.GetDetailsOfUndeliveredEvents(cancellationToken);
                foreach (var detail in details)
                {
                    _logger.LogInformation("Details of pending outbox events for {Scope}: {Key} [{Value}]", scope, detail.Key, detail.Value);
                }
            }
        }
    }
}
