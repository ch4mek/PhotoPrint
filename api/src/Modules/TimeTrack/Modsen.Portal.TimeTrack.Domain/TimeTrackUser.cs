namespace Modsen.Portal.TimeTrack.Domain;

internal class TimeTrackUser
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public bool IsActive { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }
    public int? CreatedById { get; set; }
}
