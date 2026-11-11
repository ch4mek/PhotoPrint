using System.ComponentModel.DataAnnotations;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.OData;

public class OdpAttendanceTotal
{
    [Key]
    public int UserId { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }
    public string? UserFirstNameRu { get; set; }
    public string? UserLastNameRu { get; set; }
    public string? UserFullName { get; set; }
    public string? UserFullNameRu { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserResourceGroupName { get; set; }
    public bool IsUserActive { get; set; }
    public DateOnly Date { get; set; }
    public int TotalTimeInMinutes { get; set; }

    internal static OdpAttendanceTotal Of(AttendanceEntity attendance)
    {
        return  new OdpAttendanceTotal
        {
            UserId = attendance.UserId,
            UserFirstName = attendance.User!.FirstName,
            UserLastName = attendance.User.LastName,
            UserFirstNameRu = attendance.User.FirstNameRu,
            UserLastNameRu = attendance.User.LastNameRu,
            UserFullName = ToFullName.Reverse(attendance.User.FirstName, attendance.User.LastName),

            UserFullNameRu = attendance.User.LastNameRu == null && attendance.User.FirstNameRu == null
                ? null
                : (attendance.User.LastNameRu + " " + attendance.User.FirstNameRu).Trim(),
                
            UserDepartmentId = attendance.User.DepartmentId,
            UserResourceGroupId = attendance.User.ResourceGroupId,
            UserResourceGroupName = attendance.User.ResourceGroup?.Name,
            IsUserActive = attendance.User.IsActive
        };
    }
        
}
