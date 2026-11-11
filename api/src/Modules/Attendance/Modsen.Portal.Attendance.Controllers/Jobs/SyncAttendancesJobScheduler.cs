using MediatR;
using Modsen.Portal.Attendance.Requests.Jobs;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Attendance.Controllers.Jobs;

internal sealed class SyncAttendancesJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = nameof(SyncAttendancesJob);

    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly ISender _sender;

    public SyncAttendancesJobScheduler(
        IBackgroundJobManager backgroundJobManager,
        IHostEnvironmentContext hostEnvironmentContext,
        ISender sender)
    {
        _backgroundJobManager = backgroundJobManager;
        _hostEnvironmentContext = hostEnvironmentContext;
        _sender = sender;
    }

    public void Schedule()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            var queueName = "default";
            _backgroundJobManager.ScheduleRecurringJob(s_jobId, queueName,
                methodCall: () => _sender.Send(new SyncAttendancesJob.Command(), CancellationToken.None),
                cronExpression: () => "*/15 * * * *"
            );
        }
    }
}
