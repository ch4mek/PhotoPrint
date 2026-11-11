using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal sealed class ClientCreatedEventConsumer : IConsumer<ClientCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFinanceDbContext _dbContext;

    public ClientCreatedEventConsumer(IConnectionFactory connectionFactory, IFinanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientCreatedEvent> context)
    {
        var @event = context.Message;

        var clientById = await _dbContext.FinanceClients
            .FirstOrDefaultAsync(
                p => p.Id == @event.ClientId,
                context.CancellationToken);

        if (clientById is null)
        {
            var clientByName = await _dbContext.FinanceClients
                .FirstOrDefaultAsync(
                    p => p.Name == @event.ClientName,
                    context.CancellationToken);

            if (clientByName is null)
            {
                var newClient = new FinanceClientEntity
                {
                    Id = @event.ClientId,
                    Name = @event.ClientName,
                    LegalEntity = @event.ClientLegalEntity,
                    IsActive = @event.IsActive,
                    SalesManagerId = @event.SalesManagerId
                };
                _dbContext.FinanceClients.Add(newClient);
            }
            else
            {
                var newClient = new FinanceClientEntity
                {
                    Id = @event.ClientId,
                    Name = @event.ClientName,
                    LegalEntity = @event.ClientLegalEntity,
                    IsActive = @event.IsActive,
                    SalesManagerId = @event.SalesManagerId
                };

                _dbContext.FinanceClients.Remove(clientByName);
                _dbContext.FinanceClients.Add(newClient);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ClientCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientCreatedEventConsumer>("client-created-finance-queue", "client-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientCreatedEventConsumer,
        ClientCreatedEventConsumerDefinition>;
}
