using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpProjectUser
{
    public int Id { get; set; }

    public int ProjectId { get; set; }
    public required string ProjectName { get; set; }

    public int UserId { get; set; }
    public required string UserFullName { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public bool? IsTimeOnProjectBillable { get; set; }
    public string? ClientName { get; set; }
    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsActive { get; set; }

    internal static readonly Expression<Func<ProjectUser, OdpProjectUser>> MapProjectUser =
        x => new OdpProjectUser
        {
            Id = x.Id,
            ProjectId = x.ProjectId,
            ProjectName = x.Project!.ProjectName,
            UserId = x.UserId,
            UserFullName = x.User!.LastName + ' ' + x.User.FirstName,
            DepartmentId = x.User.DepartmentId,
            ResourceGroupId = x.User.ResourceGroupId,
            IsTimeOnProjectBillable = x.IsTimeOnProjectBillable,
            ClientName = x.Project.ClientName,
            UserOnProjectFrom = x.UserOnProjectFrom,
            UserOnProjectTo = x.UserOnProjectTo,
            IsActive = x.UserOnProjectTo == null || DateOnly.FromDateTime(DateTime.UtcNow) <= x.UserOnProjectFrom
                || x.UserOnProjectFrom <= DateOnly.FromDateTime(DateTime.UtcNow) && DateOnly.FromDateTime(DateTime.UtcNow) <= x.UserOnProjectTo
        };
}

