using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Jobs;

public sealed class ContractExpirationNoticeJobScheduler: IRecurringJobScheduler
{
    private const string s_jobId = "ContractExpirationNotice";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IServiceProvider _serviceProvider;

    public ContractExpirationNoticeJobScheduler(IBackgroundJobManager backgroundJobManager,
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
            methodCall: () => DoJob(),
            cronExpression: () => "0 0 * * *");
    }

    public async Task DoJob()
    {
        using var scope = _serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new ContractExpirationNoticeRequest.Command());
    }
}
