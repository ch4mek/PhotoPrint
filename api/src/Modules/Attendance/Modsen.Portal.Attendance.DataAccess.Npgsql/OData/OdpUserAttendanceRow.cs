using Modsen.Portal.Core.Attendance;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.OData;

public sealed class OdpUserAttendanceRow
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int Time { get; set; }
    public DirectionType Direction { get; set; }
    public required string Door { get; set; }
    public required string Location { get; set; }
}
