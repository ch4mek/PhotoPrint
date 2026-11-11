using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ClientUpdatedEventConsumer : IConsumer<ClientUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ITeamBoardDbContext _dbContext;

    public ClientUpdatedEventConsumer(IConnectionFactory connectionFactory, ITeamBoardDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientUpdatedEvent> context)
    {
        var @event = context.Message;

        var client = await _dbContext.TeamBoardClients
            .FirstOrDefaultAsync(p => p.ClientId == @event.ClientId, context.CancellationToken);

        if (client is not null)
        {
            client.Name = @event.ClientName;
            client.IsActive = @event.IsActive;            

            _dbContext.TeamBoardClients.Update(client);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ClientUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientUpdatedEventConsumer>("client-updated-teamboard-queue", "client-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientUpdatedEventConsumer,
        ClientUpdatedEventConsumerDefinition>;
}
