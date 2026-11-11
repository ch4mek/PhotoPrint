using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Controllers.Jobs;

internal sealed class SyncUsersJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "SyncUsers";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IServiceProvider _serviceProvider;

    public SyncUsersJobScheduler(
        IBackgroundJobManager backgroundJobManager,
        IServiceProvider serviceProvider)
    {
        _backgroundJobManager = backgroundJobManager;
        _serviceProvider = serviceProvider;
    }

    public void Schedule()
    {
        var queueName = "default";

        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => SyncUsers(),
            cronExpression: () => "0 0 30 2 *");
    }

    public async Task SyncUsers()
    {
        using var scope = _serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new SyncUsersRequest.Command());
    }
}
