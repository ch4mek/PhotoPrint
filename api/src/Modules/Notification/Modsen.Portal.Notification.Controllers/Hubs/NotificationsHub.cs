using Microsoft.AspNetCore.SignalR;

namespace Modsen.Portal.Notification.Controllers.Hubs;

public class NotificationsHub : Hub<ISignalRNotifierClient>
{
};
