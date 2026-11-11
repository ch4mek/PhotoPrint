using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Booking.Controllers.Jobs;

internal sealed class UpdateReservationsForRepetitiveMeetingRoomJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "UpdateReservationsForRepetitiveMeetingRoom";

    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public UpdateReservationsForRepetitiveMeetingRoomJobScheduler(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IBackgroundJobManager backgroundJobManager)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _backgroundJobManager = backgroundJobManager;
    }

    public void Schedule()
    {
        var queueName = _configuration["BookingSettings:QueueName"]!;
        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => UpdateReservations(),
            cronExpression: () => "0 0 * * *");
    }

    public async Task UpdateReservations()
    {
        using var scope = _serviceProvider.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<ISender>()
            .Send(new UpdateReservationsForRepetitiveMeetingRoomRequest.Command());
    }
}
