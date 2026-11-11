using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Core;
using Modsen.Portal.Identity.Requests.Departments.Requests;
using Modsen.Portal.Identity.Requests.Messages.Core.DbQueries;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Requests;

internal class IdentityContract : IIdentityContract
{
    private readonly ISender _sender;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IIsUserManagerBusinessRule _isUserManagerBusinessRule;
    private readonly ICanUserBeProjectManagerByDepartmentIdBusinessRule _canUserBeProjectManagerByDepartmentIdBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IHashService _hashService;
    private readonly IGetUserThreadIdDbQuery _getUserThreadIdDbQuery;
    private readonly IUserClaimsByGroupIdLoader _userClaimsByGroupIdLoader;

    public IdentityContract(
        ISender sender,
        IConnectionFactory connectionFactory,
        IIsUserManagerBusinessRule isUserManagerBusinessRule,
        ICanUserBeProjectManagerByDepartmentIdBusinessRule canUserBeProjectManagerByDepartmentIdBusinessRule,
        IIdentityDbContext identityDbContext,
        IHashService hashService,
        IGetUserThreadIdDbQuery getUserThreadIdDbQuery,
        IUserClaimsByGroupIdLoader userClaimsByGroupIdLoader)
    {
        _sender = sender;
        _connectionFactory = connectionFactory;
        _isUserManagerBusinessRule = isUserManagerBusinessRule;
        _canUserBeProjectManagerByDepartmentIdBusinessRule = canUserBeProjectManagerByDepartmentIdBusinessRule;
        _identityDbContext = identityDbContext;
        _hashService = hashService;
        _getUserThreadIdDbQuery = getUserThreadIdDbQuery;
        _userClaimsByGroupIdLoader = userClaimsByGroupIdLoader;
    }

    public async Task<List<DepartmentClassificationDto>> GetDepartmentsClassificator(CancellationToken cancellationToken)
    {
        var departments = await _sender.Send(new GetDepartmentsClassificationRequest.Query(), cancellationToken);
        return departments.ConvertAll(x => new DepartmentClassificationDto(x.Id, x.ShortName, x.FullName));
    }

    public Task<List<RoleClassificationDto>> GetRolesClassificator(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetRolesClassificatorRequest.Query(), cancellationToken);
    }

    public Task<List<DepartmentModsenApiValue>> GetDepartmentModsenApiValues(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetDepartmentsModsenApiValuesRequest.Query(), cancellationToken);
    }

    public Task<string?> GetDepartmentModsenApiValue(string departmentId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetDepartmentModsenApiValueRequest.Query(departmentId), cancellationToken);
    }

    public Task<bool> CanUserBeProjectManagerByDepartment(int userId, CancellationToken cancellationToken)
    {
        return _canUserBeProjectManagerByDepartmentIdBusinessRule.IsCorrespondsToBusiness(userId, cancellationToken);
    }

    public Task<List<ProjectManagerLookupDto>> GetProjectManagersLookup(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetProjectManagersLookupRequest.Query(), cancellationToken);
    }

    public Task<List<ClaimDto>> GetUserClaimsByGroup(string groupId, int userId, CancellationToken cancellationToken)
    {
        return _userClaimsByGroupIdLoader.LoadAsync(groupId, userId, cancellationToken);
    }

    public Task<List<string>> GetUserAvatarFileIds(CancellationToken cancellationToken)
    {
        return _identityDbContext.Users.Where(u => u.AvatarFileId != null).Select(user => user.AvatarFileId).ToListAsync(cancellationToken)!;
    }

    public async Task<PermissionAssignmentDto?> GetPermissionAssignment(
        int userId,
        string resourceId,
        PermissionResourceType resourceType,
        PermissionType permissionType,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            request: new GetUserPermissionAssignmentRequest.Query(userId, resourceId, resourceType, permissionType),
            cancellationToken);

        return result == null
            ? null
            : new PermissionAssignmentDto
            {
                Id = result.Id,
                ResourceId = result.ResourceId,
                ResourceType = result.ResourceType,
                UserId = result.UserId,
                PermissionType = result.PermissionType
            };
    }

    public async Task<List<RecourceGroupUserOutput>> GetUsersFromResourceGroupById(int resourceGroupId, CancellationToken cancellationToken)
    {
        var users = await _sender.Send(new GetUsersFromResourceGroupRequest.Query(resourceGroupId), cancellationToken);
        var output = users.ConvertAll(x => new RecourceGroupUserOutput
        (
            x.UserId,
            x.ResourceGroupId
        ));

        return output;
    }

    public async Task<IReadOnlyDictionary<int, DateRangeDto[]>?> GetUsersActivePeriodsInDepartments(
        IReadOnlyCollection<int> userIds, IReadOnlyCollection<string>? departmentIds,
        CancellationToken cancellationToken)
    {
        var users = await _sender.Send(new GetUsersActivePeriodsInDepartmentsRequest.Query(userIds, departmentIds), cancellationToken);
        return users;
    }

    public async Task<List<int>> UsersWorkInDepartment(int year, IReadOnlyCollection<string> departmentIds, CancellationToken cancellationToken, int? month = null)
    {
        return await _sender.Send(new GetUsersWorkInDepartmentRequest.Query(departmentIds, year, month), cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, string>> UsersHistoricalDepartment(int year, int month, IReadOnlyCollection<int> userIds, CancellationToken cancellationToken)
    {
        return await _sender.Send(new GetUsersHistoricalDepartmentQuery.Query(year, month, userIds), cancellationToken);
    }

    public Task<List<int>> GetManagerIds(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetManagersIdsRequest.Query(), cancellationToken);
    }

    public Task<bool> IsUserManager(int userId, CancellationToken cancellationToken)
    {
        return _isUserManagerBusinessRule.IsCorrespondsToBusiness(userId, cancellationToken);
    }

    public async Task<string[]> UserRoles(int userId, CancellationToken cancellationToken)
    {
        var user = await _sender.Send(new GetUserByIdRequest.Query(userId, null, null, [], []), cancellationToken);
        return user.RoleNames.ToArray();
    }

    public Task<int[]> GetHigherManagerIds(int userId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetHigherManagerIdsRequest.Query(userId), cancellationToken);
    }

    public Task<int[]> GetHigherManagerIdsIncludingChiefs(int userId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetHigherManagerIdsIncludingChiefsRequest.Query(userId), cancellationToken);
    }

    public Task<int?> GetUserHeadId(int userId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUserHesdIdRequest.Query(userId), cancellationToken);
    }

    public Task<int?> GetUserGroupManagerId(int userId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUserGroupManagerIdRequest.Query(userId), cancellationToken);
    }

    public Task<int?> GetUserChiefIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUserChiefIdRequest.Query(userId), cancellationToken);
    }

    public async Task SaveThreadId(string userEmail, string? threadId, string assistantId, CancellationToken cancellationToken)
    {
        if (userEmail.IsNullOrEmpty() || threadId.IsNullOrEmpty())
        {
            return;
        }

        await using var transaction = _connectionFactory.GetTransaction();

        _identityDbContext.CustomDetails.Add(new CustomDetails
        {
            CreatedAtUtc = DateTime.UtcNow,
            ReferenceType = "UserEmail",
            ReferenceId = GetReferenceId(userEmail, assistantId),
            MessageSource = MessageSource.Teams,
            Value = threadId
        });

        await _identityDbContext.SaveChanges(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public Task<string?> GetThreadId(string userEmail, string assistantId, CancellationToken cancellationToken)
    {
        if (userEmail.IsNullOrWhiteSpace())
        {
            return Task.FromResult(default(string?));
        }

        var referenceId = GetReferenceId(userEmail, assistantId);

        return _getUserThreadIdDbQuery.ExecuteAsync(referenceId, MessageSource.Teams, cancellationToken);
    }

    private string GetReferenceId(string userEmail, string assistantId)
    {
        var referenceId = $"{userEmail}+{assistantId}";
        return _hashService.GetSha256Hash(referenceId);
    }

    public async Task<UserShortDto[]> GetUserShortInfo(CancellationToken cancellationToken)
    {
        var departmentIds = new[] { "Recruitment", "LD" };
        var users = await _identityDbContext.Users
            .AsNoTracking()
            .Where(x => x.UserRoles.Any())
            .Where(x => departmentIds.Contains(x.DepartmentId) ||
                x.UserRoles.Any(ur => ur.Role!.Name == PortalRoleNames.HeadOfDepartment) ||
                x.Claims.Any(uc => uc.Claim != null && uc.Claim.GroupId == RecruitmentClaimTypeNames.GroupId))
            .AsSplitQuery()
            .Select(x => new UserShortDto
            {
                Email = x.Email!,
                UserId = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                AvatarFileId = x.AvatarFileId,
                IsActive = x.IsActive
            })
            .ToArrayAsync(CancellationToken.None);

        return users;
    }

    public Task<bool> IsActive(int userId, CancellationToken cancellationToken)
    {
        return _identityDbContext.Users
            .AnyAsync(x => x.Id == userId && x.IsActive, cancellationToken);
    }

    public Task<List<DepartmentWithNameOutput>> GetDepartmentNames(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetDepartmentNamesRequest.Query(), cancellationToken);
    }

    public Task<List<UserWithManagersDto>> GetUsersWithManagersAsync(int[]? userIds, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUsersWithManagersRequest.Query(userIds), cancellationToken);
    }

    public Task<List<int>> GetUsersByClaim(string claimName, CancellationToken cancellationToken)
    {
        return _identityDbContext.UserClaims
            .Where(x => x.ClaimType == claimName)
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task<List<int>> GetUserAndSubordinatesIdsAsync(int year, int month, CancellationToken cancellationToken, bool includeSelf = false)
    {
        return _sender.Send(new GetUserAndSubordinatesIdsRequest.Query(year, month, includeSelf), cancellationToken);
    }

    public Task<List<int>> GetCeoCtoIds(CancellationToken cancellationToken)
    {
        var roleIds = _identityDbContext.Roles
            .Where(x => x.Name == PortalRoleNames.Ceo || x.Name == PortalRoleNames.Cto)
            .Select(x => x.Id);

        return _identityDbContext.UserRoles
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }
}
