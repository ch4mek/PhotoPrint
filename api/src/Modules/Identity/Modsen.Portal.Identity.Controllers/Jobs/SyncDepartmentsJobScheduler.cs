using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Requests.Departments.Requests;

namespace Modsen.Portal.Identity.Controllers.Jobs;

internal sealed class SyncDepartmentsJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "SyncDepartments";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IServiceProvider _serviceProvider;

    public SyncDepartmentsJobScheduler(
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
            methodCall: () => SyncDepartments(),
            cronExpression: () => "0 0 * * *");
    }

    public async Task SyncDepartments()
    {
        using var scope = _serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new SyncDepartmentsRequest.Command());
    }
}
