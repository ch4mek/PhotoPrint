namespace Modsen.Portal.Inquiries.Requests.Core.Dto;

public class UserNotificationBodyDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string AvatarFileId { get; set; } = null!;
    public string AvatarUri { get; set; } = null!;
}
