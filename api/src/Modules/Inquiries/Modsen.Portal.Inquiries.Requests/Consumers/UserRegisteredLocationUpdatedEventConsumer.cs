using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserRegisteredLocationUpdatedEventConsumer  : IConsumer<UserRegisteredLocationUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IInquiriesDbContext _dbContext;

    public UserRegisteredLocationUpdatedEventConsumer(IConnectionFactory connectionFactory, IInquiriesDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserRegisteredLocationUpdatedEvent> context)
    {
        var @event = context.Message;

        var user = await _dbContext.InquiriesUsers.FirstOrDefaultAsync(
            user => user.UserId == @event.UserId,
            context.CancellationToken);

        if (user != null)
        {
            user.RegisteredLocationId = @event.RegisteredLocationId;

            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserRegisteredLocationUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserRegisteredLocationUpdatedEventConsumer>(
            "user-registered-location-updated-inquiries-queue",
            "user-registered-location-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserRegisteredLocationUpdatedEventConsumer,
        UserRegisteredLocationUpdatedEventConsumerDefinition>;
}
