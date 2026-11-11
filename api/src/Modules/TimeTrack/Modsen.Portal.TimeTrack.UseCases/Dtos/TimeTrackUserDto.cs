namespace Modsen.Portal.TimeTrack.UseCases.Dtos;

internal class TimeTrackUserDto
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
}
