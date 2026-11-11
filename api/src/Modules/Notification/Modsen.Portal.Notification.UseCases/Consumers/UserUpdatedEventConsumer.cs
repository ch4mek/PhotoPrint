using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.UseCases.Consumers;

internal sealed class UserUpdatedEventConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly INotificationDbContext _dbContext;

    public UserUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        INotificationDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
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
                IsActive = true
            };

            _dbContext.NotificationUsers.Add(user);
        }

        user.UserId = @event.UserId;
        user.Email = @event.Email;
        user.FirstName = @event.FirstName;
        user.LastName = @event.LastName;
        user.AvatarFileId = @event.AvatarFileId;

        await _dbContext.SaveChanges(context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-notifications-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
