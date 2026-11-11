using Modsen.Portal.Core.Communication.Notifications;

namespace Modsen.Portal.Notification.Controllers.Hubs;

public interface ISignalRNotifierClient
{
    Task ReceiveNotification(NotificationOutput notification);
}
