using Modsen.Portal.Core.Communication.Notifications;

namespace Modsen.Portal.Notification.UseCases;

public interface ISignalRNotifier
{
    Task SendNotificationAsync(int userId, NotificationOutput notification);
}
