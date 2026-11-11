namespace Modsen.Portal.Notification.UseCases.Dto;

public class UserNotificationBodyDto
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string AvatarFileId { get; set; }
    public required string AvatarUri { get; set; }
}
