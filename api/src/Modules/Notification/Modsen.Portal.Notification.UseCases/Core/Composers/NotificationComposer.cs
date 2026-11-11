using System.Text.Json;
using System.Text.Json.Serialization;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Notification.UseCases.Core.Composers;

internal interface INotificationComposer
{
    Task<NotificationEntity[]> Compose(NotificationsCreatedEvent message, CancellationToken cancellationToken);
}

internal class NotificationComposer : INotificationComposer
{
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly IInquiryTeamsNotificationComposer _inquiryTeamsNotificationComposer;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public NotificationComposer(
        IHostEnvironmentContext hostEnvironmentContext,
        IInquiryTeamsNotificationComposer inquiryTeamsNotificationComposer)
    {
        _hostEnvironmentContext = hostEnvironmentContext;
        _inquiryTeamsNotificationComposer = inquiryTeamsNotificationComposer;
    }

    public async Task<NotificationEntity[]> Compose(
        NotificationsCreatedEvent message,
        CancellationToken cancellationToken)
    {
        var notifications = new List<NotificationEntity>();

        switch (message.NotificationType)
        {
            case NotificationTypes.UserInquiryCommentCreated:
            case NotificationTypes.UserInquiryCreated:
            case NotificationTypes.UserInquiryApproved:
            case NotificationTypes.UserInquiryRejected:
                    
                AddNotificationsForChannelIfEnabled(notifications, message, ComposeUserInquiryNotifications, NotificationChannels.SignalR);
                AddNotificationsForChannelIfEnabled(notifications, message, ComposeUserInquiryNotifications, NotificationChannels.WebNotification);
                await AddTeamsNotifications(notifications, message, cancellationToken);                    
                break;

            default:

                AddNotificationsForChannelIfEnabled(notifications, message, ComposeDefaultNotifications, NotificationChannels.SignalR);
                AddNotificationsForChannelIfEnabled(notifications, message, ComposeDefaultNotifications, NotificationChannels.WebNotification);
                break;
        }

        return notifications.ToArray();
    }

    private static void AddNotificationsForChannelIfEnabled(
        List<NotificationEntity> notifications,
        NotificationsCreatedEvent message,
        Func<NotificationsCreatedEvent, NotificationChannels, NotificationEntity[]> composer,
        NotificationChannels channel)
    {
        if (message.NotificationChannel.HasFlag(channel))
        {
            notifications.AddRange(composer(message, channel));
        }
    }

    private async Task AddTeamsNotifications(
        List<NotificationEntity> notifications,
        NotificationsCreatedEvent message, 
        CancellationToken cancellationToken)
    {
        if (message.NotificationChannel.HasFlag(NotificationChannels.Teams) &&
            !_hostEnvironmentContext.IsFunctionalTestEnvironment() &&
            !_hostEnvironmentContext.IsIntegrationTestEnvironment())
        {
            notifications.AddRange(
                await _inquiryTeamsNotificationComposer.Compose(message, cancellationToken));
        }
    }

    private static NotificationEntity[] ComposeDefaultNotifications(
        NotificationsCreatedEvent message, NotificationChannels notificationChannel)
    {
        return message.UserIds
            .Select(userId => new NotificationEntity
            {
                UserId = userId,
                NotificationChannel = notificationChannel,
                Sent = true,
                Read = false,
                IsDeleted = false,
                Arguments = message.Arguments,
                NotificationType = message.NotificationType
            })
            .ToArray();
    }

    private static NotificationEntity[] ComposeUserInquiryNotifications(
        NotificationsCreatedEvent message, NotificationChannels notificationChannel)
    {
        using var doc = JsonDocument.Parse(message.Arguments);
        var root = doc.RootElement;

        var notificationInfo = JsonSerializer.Deserialize<NotificationInfo>(root.GetRawText(), _jsonOptions);

        if (notificationInfo == null)
        {
            return [];
        }

        return message.UserIds
            .Select(userId => new NotificationEntity
            {
                UserId = userId,
                Arguments = JsonSerializer.Serialize(new
                {
                    notificationInfo.UserId,
                    notificationInfo.InquiryId,
                    notificationInfo.Message,
                    notificationInfo.InquiryCommentId,
                    InquiryType = notificationInfo.InquiryType?.ToString() ?? nameof(InquiryType.Unknown)
                }),
                NotificationChannel = notificationChannel,
                NotificationType = message.NotificationType,
                Sent = true,
                Read = false,
                IsDeleted = false,
            }).ToArray();
    }

    public class NotificationInfo
    {
        public int UserId { get; set; }
        public int InquiryId { get; set; }
        public string? Message { get; set; }
        public int? InquiryCommentId { get; set; }
        public InquiryType? InquiryType { get; set; }
    }
}
