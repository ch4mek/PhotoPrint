using Modsen.Portal.Core.Communication.Notifications;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.UseCases.Core;

internal interface INotificationSender
{
    Task SendNotification(NotificationEntity notification);
    Task SendViaSignalR(IReadOnlyCollection<int> userIds, string arguments, NotificationTypes notificationType);
}

internal class NotificationSender : INotificationSender
{
    private readonly ISignalRNotifier _notifier;
    private readonly IMicrosoftTeamsBotApiService _microsoftBotApiService;

    public NotificationSender(ISignalRNotifier notifier, IMicrosoftTeamsBotApiService microsoftBotApiService)
    {
        _notifier = notifier;
        _microsoftBotApiService = microsoftBotApiService;
    }

    public async Task SendNotification(NotificationEntity notification)
    {
        var notificationOutput = new NotificationOutput
        {
            NotificationType = notification.NotificationType.ToString(),
            OutputJson = notification.Arguments,
            UserId = notification.UserId,
            Id = notification.Id
        };

        if (notification.NotificationChannel.HasFlag(NotificationChannels.SignalR))
        {
            await _notifier.SendNotificationAsync(notificationOutput.UserId, notificationOutput);
        }

        if (notification.NotificationChannel.HasFlag(NotificationChannels.Teams))
        {
            await _microsoftBotApiService.SendNotificationAsync(notificationOutput.OutputJson);
        }
    }

    public async Task SendViaSignalR(IReadOnlyCollection<int> userIds, string arguments, NotificationTypes notificationType)
    {
        var type = notificationType.ToString();

        foreach (var userId in userIds)
        {
            var notificationOutput = new NotificationOutput
            {
                NotificationType = type,
                OutputJson = arguments,
                UserId = userId
            };

            await _notifier.SendNotificationAsync(notificationOutput.UserId, notificationOutput);
        }
    }
}
