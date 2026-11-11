using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.UseCases.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly INotificationDbContext _dbContext;

    public UserSyncEventConsumer(
        IConnectionFactory connectionFactory,
        INotificationDbContext dbContext)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
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
        }
        else
        {
            user.UserId = @event.UserId;
            user.Email = @event.Email;
            user.FirstName = @event.FirstName;
            user.LastName = @event.LastName;
            user.AvatarFileId = @event.AvatarFileId;
            user.IsActive = @event.IsActive;
        }

        await _dbContext.SaveChanges(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-notifications-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
