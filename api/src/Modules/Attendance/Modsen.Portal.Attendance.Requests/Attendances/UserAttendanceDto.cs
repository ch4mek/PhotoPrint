using Modsen.Portal.Core.Attendance;

namespace Modsen.Portal.Attendance.Requests.Attendances;

public class UserAttendanceDto
{
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public DirectionType Direction { get; set; }
    public required string Door { get; set; }
    public required string Location { get; set; }
}
