using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal class LeaveDayOutput
{
    public DateOnly Date { get; set; }
    public string? LeaveType { get; set; }

    public static LeaveDayOutput Of(LeaveDayDto leaveDayDto)
    {
        return new LeaveDayOutput
        {
            Date = leaveDayDto.Date,
            LeaveType = leaveDayDto.LeaveType
        };
    }
}
