namespace Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;

internal record IsUserHasPermissionBusinessModel
    (int UserId, string? Position, string? DepartmentId, int? ResourceGroupId, bool IsActive);
