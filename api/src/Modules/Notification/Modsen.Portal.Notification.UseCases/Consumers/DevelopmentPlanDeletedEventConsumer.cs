using System.Text.Json;
using System.Text.Json.Nodes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Core;

namespace Modsen.Portal.Notification.UseCases.Consumers;

internal sealed class DevelopmentPlanDeletedEventConsumer : IConsumer<DevelopmentPlanDeletedEvent>
{
    private readonly INotificationDbContext _notificationDbContext;
    private readonly IConnectionFactory _connectionFactory;
    private readonly INotificationSender _notificationSender;

    public DevelopmentPlanDeletedEventConsumer(
        INotificationDbContext notificationDbContext,
        IConnectionFactory connectionFactory,
        INotificationSender notificationSender)
    {
        _notificationDbContext = notificationDbContext;
        _connectionFactory = connectionFactory;
        _notificationSender = notificationSender;
    }

    public async Task Consume(ConsumeContext<DevelopmentPlanDeletedEvent> context)
    {
        var @event = context.Message;
        var notifications = await _notificationDbContext.Notifications
            .Where(notification => notification.NotificationType == NotificationTypes.DevelopmentPlanSentForReview ||
                notification.NotificationType == NotificationTypes.DevelopmentPlanReviewed)
            .ToListAsync(context.CancellationToken);

        _notificationDbContext.Notifications.RemoveRange(notifications.Where(x => HasDevelopmentPlanId(x.Arguments, @event.DevelopmentPlanId)));

        await _notificationDbContext.SaveChanges(context.CancellationToken);
        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        await _notificationSender.SendViaSignalR(@event.UserIds, JsonSerializer.Serialize(new { @event.DevelopmentPlanId }),
            NotificationTypes.EventRecordsRemoved);
    }

    private static bool HasDevelopmentPlanId(string arguments, int developmentPlanId)
    {
        var json = JsonNode.Parse(arguments);

        return json?["DevelopmentPlanId"]?.GetValue<int?>() == developmentPlanId;
    }

    internal sealed class DevelopmentPlanDeletedEventConsumerDefinition() :
        ConsumerDefinitionBase<DevelopmentPlanDeletedEventConsumer>(
            "development-plan-deleted-queue",
            "development-plan-deleted-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        DevelopmentPlanDeletedEventConsumer,
        DevelopmentPlanDeletedEventConsumerDefinition>;

}

