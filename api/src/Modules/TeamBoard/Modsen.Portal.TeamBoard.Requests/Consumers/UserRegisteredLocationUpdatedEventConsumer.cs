using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class UserRegisteredLocationUpdatedEventConsumer : IConsumer<UserRegisteredLocationUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public UserRegisteredLocationUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService)
    {
        _connectionFactory = connectionFactory;
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredLocationUpdatedEvent> context)
    {
        var @event = context.Message;

        await UpdateLocationsAsync(@event, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task UpdateLocationsAsync(UserRegisteredLocationUpdatedEvent @event, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == @event.UserId)
            .Include(user => user.RegisteredLocations)
            .ThenInclude(locationHistory => locationHistory.RegisteredLocation)
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            var currentRegisteredLocation = user.RegisteredLocations.FirstOrDefault(x => x.DateTo == null);

            if (currentRegisteredLocation?.RegisteredLocation?.Id != @event.RegisteredLocationId)
            {
                if (currentRegisteredLocation != null)
                {
                    currentRegisteredLocation.DateTo = _clockService.TodayUtc();
                }

                user.RegisteredLocations.Add(new RegisteredLocationHistory
                {
                    RegisteredLocationId = @event.RegisteredLocationId,
                    DateFrom = _clockService.TodayUtc(),
                });
            }

            user.CurrentLocation = @event.CurrentLocation;

            user.Location = @event.Location.ToString();
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }

    internal sealed class UserRegisteredLocationUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserRegisteredLocationUpdatedEventConsumer>(
            "user-registered-location-updated-teamboard-queue",
            "user-registered-location-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserRegisteredLocationUpdatedEventConsumer,
        UserRegisteredLocationUpdatedEventConsumerDefinition>;
}
