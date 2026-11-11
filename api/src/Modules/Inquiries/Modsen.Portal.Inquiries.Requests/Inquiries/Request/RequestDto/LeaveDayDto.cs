namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

internal class LeaveDayDto
{
    public DateOnly Date { get; set; }
    public required string LeaveType { get; set; }
}
