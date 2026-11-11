using System.Globalization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class ClientUpdatedEventConsumer : IConsumer<ClientUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICvDbContext _dbContext;

    public ClientUpdatedEventConsumer(IConnectionFactory connectionFactory, ICvDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientUpdatedEvent> context)
    {
        var @event = context.Message;

        var client = await _dbContext.CvClients
            .FirstOrDefaultAsync(p => p.ClientId == @event.ClientId.ToString(CultureInfo.InvariantCulture), context.CancellationToken);

        if (client is not null)
        {
            client.Name = @event.ClientName;
            client.IsActive = @event.IsActive;            

            _dbContext.CvClients.Update(client);
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ClientUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientUpdatedEventConsumer>("client-updated-cv-queue", "client-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientUpdatedEventConsumer,
        ClientUpdatedEventConsumerDefinition>;
}
