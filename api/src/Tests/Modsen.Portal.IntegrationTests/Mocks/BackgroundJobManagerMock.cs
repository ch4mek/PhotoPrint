using System.Linq.Expressions;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.IntegrationTests.Mocks;

internal class BackgroundJobManagerMock : IBackgroundJobManager
{
    public string Schedule(string queue, Expression<Func<Task>> methodCall)
    {
        var method = methodCall.Compile();
        method.Invoke().GetAwaiter().GetResult();

        return string.Empty;
    }

    public void ScheduleRecurringJob(string jobId, string queue, Expression<Func<Task>> methodCall,
        Func<string> cronExpression, TimeZoneInfo? timeZoneInfo = null) { }
}
