using System.Linq.Expressions;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpProjectExitDto
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
    public bool IsManagedByRole { get; set; }
    public string? UserPosition { get; set; }

    internal static readonly Expression<Func<OdpProjectExit, OdpProjectExitDto>> Map =
        source => new OdpProjectExitDto()
        {
            Id = source.Id,
            UserId = source.UserId,
            FullName = source.FullName,
            DepartmentId = source.DepartmentId,
            DepartmentName = source.DepartmentName,
            UserResourceGroupId = source.UserResourceGroupId,
            UserResourceGroupName = source.UserResourceGroupName,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            ProjectId = source.ProjectId,
            ProjectName = source.ProjectName,
            ClosingDate = source.ClosingDate,
            ClosingReasonId = source.ClosingReasonId,
            ClosingReasonName = source.ClosingReasonName,
            IsActive = source.IsActive,
            IsUserActive = source.IsUserActive,
            UserPosition = source.UserPosition
        };
}
