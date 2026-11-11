namespace Modsen.Portal.Core.Communication.Notifications;

public sealed class NotificationOutput
{
    public int Id { get; set; }
    public required string OutputJson { get; set; }
    public int UserId { get; set; }
    public string? NotificationType { get; set; }
}
