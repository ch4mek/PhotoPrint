using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.UseCases.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly INotificationDbContext _dbContext;

    public UserCreatedEventConsumer(
        IConnectionFactory connectionFactory,
        INotificationDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;

        var user = await _dbContext.NotificationUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user == null)
        {
            user = new NotificationUserEntity
            {
                UserId = @event.UserId,
                Email = @event.Email,
                AvatarFileId = @event.AvatarFileId,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                IsActive = true,
            };

            _dbContext.NotificationUsers.Add(user);

            await _dbContext.SaveChanges(context.CancellationToken);
            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-notifications-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
