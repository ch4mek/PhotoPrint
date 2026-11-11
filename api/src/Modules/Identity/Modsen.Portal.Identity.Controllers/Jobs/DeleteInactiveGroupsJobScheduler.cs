using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

namespace Modsen.Portal.Identity.Controllers.Jobs;

internal sealed class DeleteInactiveGroupsJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "DeleteInactiveGroups";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IServiceProvider _serviceProvider;

    public DeleteInactiveGroupsJobScheduler(IBackgroundJobManager backgroundJobManager,
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
            methodCall: () => DeleteInactiveGroups(),
            cronExpression: () => "0 0 1 * *");
    }

    public async Task DeleteInactiveGroups()
    {
        using var scope = _serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new DeleteInactiveGroupsRequest.Command());
    }
}
