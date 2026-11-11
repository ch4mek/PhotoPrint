using System.Linq.Expressions;

namespace Modsen.Portal.Core.Services;

public interface IBackgroundJobManager
{
    string Schedule(string queue, Expression<Func<Task>> methodCall);

    void ScheduleRecurringJob(
        string jobId,
        string queue,
        Expression<Func<Task>> methodCall,
        Func<string> cronExpression,
        TimeZoneInfo? timeZoneInfo = null);
}
