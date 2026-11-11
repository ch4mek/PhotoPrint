using System.Linq.Expressions;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Attendance;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.OData;

public class OdpAttendance
{
    public int Id { get; set; }
    public required string UserFullName { get; set; }
    public string? UserFullNameRu { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserResourceGroupName { get; set; }
    public bool IsUserActive { get; set; }
    public DirectionType Direction { get; set; }
    public required string Door { get; set; }
    public required string Location { get; set; }
    public DateTime CheckedAtUtc { get; set; }

    internal static readonly Expression<Func<AttendanceEntity, OdpAttendance>> Map = attendance => new OdpAttendance
    {
        Id = attendance.Id,
        UserFullName = attendance.User!.LastName + " " + attendance.User!.FirstName,

        UserFullNameRu = attendance.User.LastNameRu == null && attendance.User.FirstNameRu == null
                ? null
                : (attendance.User.LastNameRu + " " + attendance.User.FirstNameRu).Trim(),

        UserDepartmentId = attendance.User!.DepartmentId,
        UserResourceGroupId = attendance.User!.ResourceGroupId,
        UserResourceGroupName = attendance.User!.ResourceGroupId != null ? attendance.User!.ResourceGroup!.Name : null,
        IsUserActive = attendance.User.IsActive,
        Direction = attendance.Direction,
        Door = attendance.Door,
        Location = attendance.Location.ToString(),
        CheckedAtUtc = attendance.CheckedAtUtc,
    };
}
