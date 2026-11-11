namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

internal class LeaveDto
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Position { get; set; }
    public string? AvatarFieldId { get; set; }
    public IReadOnlyCollection<LeaveDayDto> LeaveDays { get; set; } = [];
}
