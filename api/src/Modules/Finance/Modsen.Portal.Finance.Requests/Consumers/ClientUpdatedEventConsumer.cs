using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Finance.Requests.Consumers;

internal sealed class ClientUpdatedEventConsumer : IConsumer<ClientUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFinanceDbContext _dbContext;

    public ClientUpdatedEventConsumer(IConnectionFactory connectionFactory, IFinanceDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientUpdatedEvent> context)
    {
        var @event = context.Message;

        var client = await _dbContext.FinanceClients
                .FirstOrDefaultAsync(p => p.Id == @event.ClientId, context.CancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ClientNotFound); 

        
        client.Name = @event.ClientName;
        client.LegalEntity = @event.ClientLegalEntity;
        client.IsActive = @event.IsActive;
        client.SalesManagerId = @event.SalesManagerId;

        _dbContext.FinanceClients.Update(client);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class ClientUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientUpdatedEventConsumer>("client-updated-finance-queue", "client-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientUpdatedEventConsumer,
        ClientUpdatedEventConsumerDefinition>;
}
