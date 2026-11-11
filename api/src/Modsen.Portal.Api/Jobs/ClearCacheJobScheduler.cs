using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Api.Jobs;

internal sealed class ClearCacheJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "ClearCache";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IEnumerable<IRecyclableCache> _recyclableCaches;

    public ClearCacheJobScheduler(
        IBackgroundJobManager backgroundJobManager,
        IEnumerable<IRecyclableCache> recyclableCaches)
    {
        _backgroundJobManager = backgroundJobManager;
        _recyclableCaches = recyclableCaches;
    }

    public void Schedule()
    {
        var queueName = "default";

        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => ClearCache(),
            cronExpression: () => "0 0 30 2 *");
    }

    public Task ClearCache()
    {
        foreach (var cache in _recyclableCaches)
        {
            cache.Clear();
        }

        return Task.CompletedTask;
    }
}
