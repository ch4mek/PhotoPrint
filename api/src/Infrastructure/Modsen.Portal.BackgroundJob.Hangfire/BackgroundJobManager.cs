using System.Linq.Expressions;
using Hangfire;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.BackgroundJob.Hangfire;

public sealed class BackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BackgroundJobManager(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public string Schedule(string queue, Expression<Func<Task>> methodCall)
    {
        return _backgroundJobClient.Enqueue(queue, methodCall);
    }

    public void ScheduleRecurringJob(
        string jobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeZoneInfo? timeZoneInfo = null)
    {
        var options = new RecurringJobOptions
        {
            TimeZone = timeZoneInfo ?? TimeZoneInfo.Local
        };

        RecurringJob.AddOrUpdate(jobId, queue, methodCall, cronExpression, options);
    }

}
