using System.Globalization;
using Microsoft.AspNetCore.SignalR;
using Modsen.Portal.Core.Communication.Notifications;
using Modsen.Portal.Notification.Controllers.Hubs;
using Modsen.Portal.Notification.UseCases;

namespace Modsen.Portal.Notification.Controllers;

internal class SignalRNotifier : ISignalRNotifier
{
    private readonly IHubContext<NotificationsHub, ISignalRNotifierClient> _hubContext;

    public SignalRNotifier(IHubContext<NotificationsHub,
        ISignalRNotifierClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(int userId, NotificationOutput notification)
    {
        await _hubContext.Clients
            .User(userId.ToString(CultureInfo.InvariantCulture))
            .ReceiveNotification(notification);
    }
}
