using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Models;

namespace Modsen.Portal.Identity.Contract;

public interface IIdentityContract
{
    // TimeTrack:
    Task<PermissionAssignmentDto?> GetPermissionAssignment(int userId, string resourceId,
        PermissionResourceType resourceType, PermissionType permissionType, CancellationToken cancellationToken);
    Task<bool> CanUserBeProjectManagerByDepartment(int userId, CancellationToken cancellationToken);
    Task<List<ProjectManagerLookupDto>> GetProjectManagersLookup(CancellationToken cancellationToken);
    Task<List<DepartmentWithNameOutput>> GetDepartmentNames(CancellationToken cancellationToken);

    // TimeTrack, Equipment
    public Task<List<int>> GetManagerIds(CancellationToken cancellationToken);
    public Task<bool> IsUserManager(int userId, CancellationToken cancellationToken);
    public Task<bool> IsActive(int userId, CancellationToken cancellationToken);

    // Certificate, HOST API
    Task<string[]> UserRoles(int userId, CancellationToken cancellationToken);
    Task<List<DepartmentClassificationDto>> GetDepartmentsClassificator(CancellationToken cancellationToken);
    public Task<List<RoleClassificationDto>> GetRolesClassificator(CancellationToken cancellationToken);
    Task<List<DepartmentModsenApiValue>> GetDepartmentModsenApiValues(CancellationToken cancellationToken);
    Task<string?> GetDepartmentModsenApiValue(string departmentId, CancellationToken cancellationToken);

    // TimeTrack
    Task<List<RecourceGroupUserOutput>> GetUsersFromResourceGroupById(int resourceGroupId, CancellationToken cancellationToken);
    Task<List<int>> GetUsersByClaim(string claimName, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, DateRangeDto[]>?> GetUsersActivePeriodsInDepartments(IReadOnlyCollection<int> userIds, IReadOnlyCollection<string>? departmentIds, CancellationToken cancellationToken);
    Task<List<int>> UsersWorkInDepartment(int year, IReadOnlyCollection<string> departmentIds, CancellationToken cancellationToken, int? month = null);
    Task<IReadOnlyDictionary<int, string>> UsersHistoricalDepartment(int year, int month, IReadOnlyCollection<int> userIds, CancellationToken cancellationToken);

    // Inquiries
    Task<int[]> GetHigherManagerIdsIncludingChiefs(int userId, CancellationToken cancellationToken);
    Task<int[]> GetHigherManagerIds(int userId, CancellationToken cancellationToken);
    Task<int?> GetUserHeadId(int userId, CancellationToken cancellationToken);
    Task<int?> GetUserGroupManagerId(int userId, CancellationToken cancellationToken);
    Task<int?> GetUserChiefIdAsync(int userId, CancellationToken cancellationToken);
    Task<List<UserWithManagersDto>> GetUsersWithManagersAsync(int[]? userIds, CancellationToken cancellationToken);

    // API
    Task SaveThreadId(string userEmail, string? threadId, string assistantId, CancellationToken cancellationToken);
    Task<string?> GetThreadId(string userEmail, string assistantId, CancellationToken cancellationToken);


    // Recruitment
    Task<UserShortDto[]> GetUserShortInfo(CancellationToken cancellationToken);
    Task<List<ClaimDto>> GetUserClaimsByGroup(string groupId, int userId, CancellationToken cancellationToken);

    // Storage
    Task<List<string>> GetUserAvatarFileIds(CancellationToken cancellationToken);

    //Finance
    Task<List<int>> GetUserAndSubordinatesIdsAsync(int year, int month, CancellationToken cancellationToken, bool includeSelf = false);
    Task<List<int>> GetCeoCtoIds(CancellationToken cancellationToken);

}
