using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.HostBuilder.Jobs;

public static class BackgroundJobScheduler
{
    public static void ScheduleRecurringJobs(this IServiceProvider serviceProvider)
    {
        foreach (var scheduler in serviceProvider.GetServices<IRecurringJobScheduler>())
        {
            scheduler.Schedule();
        }
    }
}
