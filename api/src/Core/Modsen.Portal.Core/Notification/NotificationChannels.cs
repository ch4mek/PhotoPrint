using System.ComponentModel;

namespace Modsen.Portal.Core.Notification;

[Flags]
public enum NotificationChannels
{
    [Description("Unknown")]
    None = 0,

    [Description("WebNotification")]
    WebNotification = 1,

    [Description("SignalR")]
    SignalR = 2,

    [Description("Teams")]
    Teams = 4,
}
