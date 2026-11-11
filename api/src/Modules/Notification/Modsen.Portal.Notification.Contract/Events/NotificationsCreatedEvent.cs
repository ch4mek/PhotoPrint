using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Notification;

namespace Modsen.Portal.Notification.Contract.Events;

public class NotificationsCreatedEvent : IIntegrationEvent
{
    public required string EventId { get; set; }
    public required IReadOnlyCollection<int> UserIds { get; set; }
    public required string Arguments { get; set; }
    public required NotificationTypes NotificationType { get; set; }
    public required NotificationChannels NotificationChannel { get; set; }
}

internal sealed class NotificationCreatedExchangeDefinition : ExchangeDefinitionBase<NotificationsCreatedEvent>
{
    public override string ExchangeName => "notification-created-notification-exchange";
}
