using System.Linq.Expressions;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

public class ProjectPermissionAssignmentCoreDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string UserFullName { get; set; }
    public string? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int PermissionId { get; set; }
    public required string PermissionName { get; set; }
    public string? UserDepartmentId { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserColor { get; set; }

    internal static readonly Expression<Func<PermissionAssignment, ProjectPermissionAssignmentCoreDto>> MapPermissionAssign =
        x => new ProjectPermissionAssignmentCoreDto
        {
            Id = x.Id,
            ProjectId = x.ResourceId,
            ProjectName = x.ResourceName,

            UserId = x.UserId,
            UserDepartmentId = x.User!.DepartmentId,
            UserResourceGroupId = x.User.ResourceGroupId,
            UserColor = x.User!.Color,
            UserFullName = x.User.LastName + " " + x.User.FirstName,

            PermissionId = x.PermissionDefinitionId,
            PermissionName = x.Permission!.Name,
        };
}
