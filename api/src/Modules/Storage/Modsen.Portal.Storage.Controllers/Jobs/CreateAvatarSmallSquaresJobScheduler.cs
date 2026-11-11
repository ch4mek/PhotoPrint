using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.Requests.Jobs;

namespace Modsen.Portal.Storage.Controllers.Jobs;

internal sealed class CreateAvatarSmallSquaresJobScheduler(
    IBackgroundJobManager backgroundJobManager,
    IServiceProvider serviceProvider) : IRecurringJobScheduler
{
    private const string s_jobId = nameof(CreateAvatarSmallSquaresJob);

    public void Schedule()
    {
        const string queueName = "default";

        backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => Execute(),
            cronExpression: Cron.Never);
    }

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public async Task Execute()
    {
        using var scope = serviceProvider.CreateScope();

        var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();
        await using var transaction = connectionFactory.GetTransaction();

        var job = scope.ServiceProvider.GetRequiredService<CreateAvatarSmallSquaresJob>();
        await job.Execute();
    }
}
