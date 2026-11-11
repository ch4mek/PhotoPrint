using System.Globalization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class ClientCreatedEventConsumer : IConsumer<ClientCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICvDbContext _dbContext;

    public ClientCreatedEventConsumer(IConnectionFactory connectionFactory, ICvDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ClientCreatedEvent> context)
    {
        var @event = context.Message;

        var clientById = await _dbContext.CvClients
            .FirstOrDefaultAsync(
                p => p.ClientId == @event.ClientId.ToString(CultureInfo.InvariantCulture),
                context.CancellationToken);

        if (clientById is null)
        {
            var clientByName = await _dbContext.CvClients
                .Include(p => p.Cvs)
                .FirstOrDefaultAsync(
                    p => p.Name == @event.ClientName,
                    context.CancellationToken);

            if (clientByName is null)
            {
                var newClient = new CvClientEntity
                {
                    ClientId = @event.ClientId.ToString(CultureInfo.InvariantCulture),
                    Name = @event.ClientName,
                    IsActive = @event.IsActive,
                };
                _dbContext.CvClients.Add(newClient);
            }
            else
            {
                var newClient = new CvClientEntity
                {
                    ClientId = @event.ClientId.ToString(CultureInfo.InvariantCulture),
                    Name = @event.ClientName,
                    IsActive = @event.IsActive,
                    Cvs = clientByName.Cvs,
                };

                _dbContext.CvClients.Remove(clientByName);
                _dbContext.CvClients.Add(newClient);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class ClientCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<ClientCreatedEventConsumer>("client-created-cv-queue", "client-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        ClientCreatedEventConsumer,
        ClientCreatedEventConsumerDefinition>;
}
