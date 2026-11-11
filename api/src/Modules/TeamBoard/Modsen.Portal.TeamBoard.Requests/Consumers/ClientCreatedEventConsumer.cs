using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.TeamBoard.Requests.Consumers;

internal sealed class ClientCreatedEventConsumer : IConsumer<ClientCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ITeamBoardDbContext _dbContext;

    public ClientCreatedEventConsumer(IConnectionFactory connectionFactory, ITeamBoardDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientCreatedEvent> context)
    {
        var @event = context.Message;

        var clientById = await _dbContext.TeamBoardClients
            .FirstOrDefaultAsync(
                p => p.ClientId == @event.ClientId,
                context.CancellationToken);

        if (clientById is null)
        {
            var clientByName = await _dbContext.TeamBoardClients
                .Include(x => x.ProjectExits)
                .FirstOrDefaultAsync(
                    p => p.Name == @event.ClientName,
                    context.CancellationToken);

            if (clientByName is null)
            {
                var newClient = new TeamBoardClient
                {
                    ClientId = @event.ClientId,
                    Name = @event.ClientName,
                    IsActive = @event.IsActive
                };
                _dbContext.TeamBoardClients.Add(newClient);
            }
            else
            {
                var newClient = new TeamBoardClient
                {
                    ClientId = @event.ClientId,
                    Name = @event.ClientName,
                    IsActive = @event.IsActive,
                    ProjectExits = clientByName.ProjectExits,
                };

                _dbContext.TeamBoardClients.Remove(clientByName);
                _dbContext.TeamBoardClients.Add(newClient);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ClientCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientCreatedEventConsumer>("client-created-teamboard-queue", "client-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientCreatedEventConsumer,
        ClientCreatedEventConsumerDefinition>;
}
