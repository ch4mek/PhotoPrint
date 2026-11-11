using MassTransit;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Core;
using Modsen.Portal.Notification.UseCases.Core.Composers;

namespace Modsen.Portal.Notification.UseCases;

internal sealed class NotificationsCreatedEventConsumer : IConsumer<NotificationsCreatedEvent>
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly IConnectionFactory _connectionFactory;
    private readonly INotificationSender _notificationSender;
    private readonly INotificationComposer _notificationComposer;

    public NotificationsCreatedEventConsumer(
        INotificationDbContext notificationDbContext,
        IConnectionFactory connectionFactory,
        INotificationSender notificationSender,
        INotificationComposer notificationComposer)
    {
        _notificationDbContext = notificationDbContext;
        _connectionFactory = connectionFactory;
        _notificationSender = notificationSender;
        _notificationComposer = notificationComposer;   
    }

    public async Task Consume(ConsumeContext<NotificationsCreatedEvent> context)
    {
        var notifications = await _notificationComposer.Compose(context.Message, context.CancellationToken);
        _notificationDbContext.Notifications.AddRange(notifications);

        await _notificationDbContext.SaveChanges(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);

        foreach (var notification in notifications)
        {
            await _notificationSender.SendNotification(notification);
        }
    }

    internal sealed class NotificationsCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<NotificationsCreatedEventConsumer>(
            "notification-created-notification-queue",
            "notification-created-notification-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        NotificationsCreatedEventConsumer,
        NotificationsCreatedEventConsumerDefinition>;
}
