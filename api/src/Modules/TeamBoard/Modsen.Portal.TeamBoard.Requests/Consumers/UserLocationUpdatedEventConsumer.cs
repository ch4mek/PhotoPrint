using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class UserLocationUpdatedEventConsumer : IConsumer<UserLocationUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public UserLocationUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _connectionFactory = connectionFactory;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task Consume(ConsumeContext<UserLocationUpdatedEvent> context)
    {
        var @event = context.Message;

        await UpdateLocationAsync(@event.UserId, @event.Location, context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private async Task UpdateLocationAsync(int userId, LocationType location, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(user => user.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user != null)
        {
            user.Location = location.GetDescription();
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }

    internal sealed class UserLocationUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserLocationUpdatedEventConsumer>(
            "user-location-updated-teamboard-queue",
            "user-location-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserLocationUpdatedEventConsumer,
        UserLocationUpdatedEventConsumerDefinition>;
}
