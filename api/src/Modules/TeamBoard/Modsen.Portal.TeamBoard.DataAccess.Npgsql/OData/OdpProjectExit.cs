using System.Linq.Expressions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpProjectExit
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? UserResourceGroupId { get; set; }
    public string? UserResourceGroupName { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = null!;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public DateOnly ClosingDate { get; set; }
    public int ClosingReasonId { get; set; }
    public string ClosingReasonName { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsUserActive { get; set; }
    public string? UserPosition { get; set; }

    internal static readonly Expression<Func<ProjectExit, OdpProjectExit>> Map =
        source => new OdpProjectExit
        {
            Id = source.Id,
            UserId = source.UserId,
            FullName = source.User!.LastNameRu == null && source.User.FirstNameRu == null
                ? source.User.LastName + " " + source.User.FirstName
                : (source.User.LastNameRu + " " + source.User.FirstNameRu + " (" + source.User.LastName + " " + source.User.FirstName + ")").Trim(),
            DepartmentId = source.User.DepartmentId,
            DepartmentName = source.User.Department!.FullName,
            UserResourceGroupId = source.User.ResourceGroupId,
            UserResourceGroupName = source.User.ResourceGroup!.ResourceGroupName,
            ClientId = source.ClientId,
            ClientName = source.Client!.Name,
            ProjectId = source.ProjectId,
            ProjectName = source.Project!.ProjectName,
            ClosingDate = source.ClosingDate,
            ClosingReasonId = source.ClosingReasonId,
            ClosingReasonName = source.ClosingReason!.Name,
            IsActive = source.IsActive,
            IsUserActive = source.User.IsActive,
            UserPosition = source.User.Position
        };
}
