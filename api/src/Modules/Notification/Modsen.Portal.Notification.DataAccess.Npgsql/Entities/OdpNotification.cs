using System.Linq.Expressions;
using Modsen.Portal.Core.Notification;

namespace Modsen.Portal.Notification.DataAccess.Npgsql.Entities;

public class OdpNotification
{
    public int Id { get; set; }
    public required string OutputJson { get; set; }
    public int UserId { get; set; }

    public bool Sent { get; set; }
    public bool Read { get; set; }
    public bool IsDeleted { get; set; }

    public NotificationTypes NotificationType { get; set; }

    internal static readonly Expression<Func<NotificationEntity, OdpNotification>> Map =
        x => new OdpNotification
        {
            Id = x.Id,
            OutputJson = x.Arguments,
            IsDeleted = x.IsDeleted,
            NotificationType = x.NotificationType,
            Read = x.Read,
            Sent = x.Sent,
            UserId = x.UserId
        };
}
