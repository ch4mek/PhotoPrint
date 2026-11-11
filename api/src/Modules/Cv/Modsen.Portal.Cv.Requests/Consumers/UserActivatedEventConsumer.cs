using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Cv.Requests.Consumers;

internal sealed class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ICvDbContext _dbContext;

    public UserActivatedEventConsumer(IConnectionFactory connectionFactory, ICvDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;

        var user = await _dbContext.CvUsers
            .FirstOrDefaultAsync(user => user.UserId == @event.UserId, context.CancellationToken);

        if (user is not null)
        {
            user.IsActive = true;

            await _dbContext.SaveChangesAsync(context.CancellationToken);
        }

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-cv-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
