using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

public interface ICanIdentityUserBeActivatedBusinessRule : IBusinessRuleWithFailReason<UserDto>;

internal class CanIdentityUserBeActivatedBusinessRule : ICanIdentityUserBeActivatedBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLookupLoader _userLookupLoader;

    public CanIdentityUserBeActivatedBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityPortalSettingsAccessor identityPortalSettingsAccessor,
        IIdentityDbContext identityDbContext,
        IUserLookupLoader userLookupLoader)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
        _identityDbContext = identityDbContext;
        _userLookupLoader = userLookupLoader;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(UserDto? userToActivate, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userToActivate);

        var currentUserCanActivateByClaim = _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name);

        if (currentUserCanActivateByClaim)
        {
            return BusinessRuleResult.Success();
        }

        var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);
        var currentUserRoles = _authenticatedUserContext.GetRoles();

        if (currentUserRoles.IsCeo())
        {
            return await GetResultForCeo(userToActivate, settings, cancellationToken);
        }

        if (currentUserRoles.IsCto())
        {
            return await GetResultForCto(userToActivate, settings, cancellationToken);
        }

        if (currentUserRoles.IsHeadOfDepartment())
        {
            return GetResultForHeadOfDepartment(userToActivate);
        }

        if (currentUserRoles.IsSystemAdministrator())
        {
            return GetResultForSystemAdministrator(userToActivate);
        }

        if (currentUserRoles.IsGroupManager() || currentUserRoles.IsEmployee() || currentUserRoles.IsIntern())
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotActivateUser);
        }

        return BusinessRuleResult.Fail($"[CanIdentityUserBeActivatedBusinessRule] Unsupported role: {string.Join(" ", currentUserRoles)}");
    }

    private async Task<BusinessRuleResult> GetResultForCeo(UserDto userToActivate, IdentityPortalSettingsDto settings, CancellationToken cancellationToken)
    {
        if (userToActivate.Position.IsCto())
        {
            return await CheckCtoForActivation(settings, cancellationToken);
        }

        if (userToActivate.Position.IsHeadOfDepartment())
        {
            return await CheckHeadOfDepartmentForActivation(userToActivate.DepartmentId!, settings, cancellationToken);
        }

        if (userToActivate.Position.IsGroupManager())
        {
            return await CheckGroupManagerForActivation(userToActivate.ResourceGroupId, settings, cancellationToken);
        }

        if (userToActivate.Position.IsEmployee() || userToActivate.Position.IsSystemAdministrator() || userToActivate.Position.IsIntern())
        {
            return BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Fail($"[CanIdentityUserBeActivatedBusinessRule] Unsupported position for CEO: {userToActivate.Position}");
    }

    private async Task<BusinessRuleResult> GetResultForCto(UserDto userToActivate, IdentityPortalSettingsDto settings, CancellationToken cancellationToken)
    {
        if (userToActivate.Position.IsCeo() || userToActivate.Position.IsCto())
        {
            return BusinessRuleResult.Fail($"{PortalRoleNames.Cto} cannot activate {userToActivate.Position}");
        }

        if (userToActivate.Position.IsHeadOfDepartment())
        {
            return await CheckHeadOfDepartmentForActivation(userToActivate.DepartmentId!, settings, cancellationToken);
        }

        if (userToActivate.Position.IsGroupManager())
        {
            return await CheckGroupManagerForActivation(userToActivate.ResourceGroupId, settings, cancellationToken);
        }

        if (userToActivate.Position.IsEmployee() || userToActivate.Position.IsSystemAdministrator() || userToActivate.Position.IsIntern())
        {
            return BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Fail($"[CanIdentityUserBeActivatedBusinessRule] Unsupported position for CTO: {userToActivate.Position}");
    }

    private BusinessRuleResult GetResultForHeadOfDepartment(UserDto userToActivate)
    {
        var canActivateManagerRoles = userToActivate.Position.IsCeo() || userToActivate.Position.IsCto() || userToActivate.Position.IsHeadOfDepartment();
        if (canActivateManagerRoles)
        {
            return BusinessRuleResult.Fail($"Head of Department cannot activate {userToActivate.Position}");
        }

        var canActivateSubordinateRoles = userToActivate.Position.IsEmployee() || userToActivate.Position.IsSystemAdministrator() || userToActivate.Position.IsGroupManager() || userToActivate.Position.IsIntern();
        if (canActivateSubordinateRoles)
        {
            return _authenticatedUserContext.DepartmentId == userToActivate.DepartmentId
                ? BusinessRuleResult.Success()
                : BusinessRuleResult.Fail(PortalMessages.HeadCanActivateUsersOnlyFromHisDepartment);
        }

        return BusinessRuleResult.Fail($"[CanIdentityUserBeActivatedBusinessRule] Unsupported position for HoD: {userToActivate.Position}");
    }

    private static BusinessRuleResult GetResultForSystemAdministrator(UserDto userToActivate)
    {
        var cannotActivateRoles = userToActivate.Position.IsCeo() || userToActivate.Position.IsCto() ||
            userToActivate.Position.IsHeadOfDepartment() || userToActivate.Position.IsGroupManager() || userToActivate.Position.IsSystemAdministrator();
        if (cannotActivateRoles)
        {
            return BusinessRuleResult.Fail($"System administrator cannot activate {userToActivate.Position}");
        }

        if (userToActivate.Position.IsEmployee() || userToActivate.Position.IsIntern())
        {
            return BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Fail($"[CanIdentityUserBeActivatedBusinessRule] Unsupported position for SysAdmin: {userToActivate.Position}");
    }

    private async Task<BusinessRuleResult> CheckCtoForActivation(IdentityPortalSettingsDto settings, CancellationToken cancellationToken)
    {
        var activeCtoCount = await _identityDbContext.Users
            .CountAsync(user =>
                user.IsActive && user.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.Cto),
                cancellationToken);

        var result = activeCtoCount < settings.MaxCtoCount
            ? BusinessRuleResult.Success()
            : BusinessRuleResult.Fail($"{PortalRoleNames.Cto} is already exists in the system");

        return result;
    }

    private async Task<BusinessRuleResult> CheckHeadOfDepartmentForActivation(string departmentId, IdentityPortalSettingsDto settings, CancellationToken cancellationToken)
    {
        var lookupLoaderRequest = new CoreDto.UserLookupLoaderRequestDto
        {
            KeyWord = null,
            IsActive = true,
            DepartmentId = departmentId,
            ResourceGroupId = null,
            Roles = [PortalRoleNames.HeadOfDepartment],
        };

        var activeHeadsOfDepartments = await _userLookupLoader.LoadAsync(
            lookupLoaderRequest,
            cancellationToken);

        BusinessRuleResult result;

        if (activeHeadsOfDepartments.Count < settings.MaxHeadOfSpecificDepartmentCount)
        {
            result = BusinessRuleResult.Success();
        }
        else
        {
            var department = await _identityDbContext.Departments.FirstAsync(d => d.Id == departmentId, cancellationToken);
            result = BusinessRuleResult.Fail($"Head of {department.FullName} department is already exists in the system");
        }

        return result;
    }

    private async Task<BusinessRuleResult> CheckGroupManagerForActivation(int? resourceGroupId, IdentityPortalSettingsDto settings, CancellationToken cancellationToken)
    {
        var lookupLoaderRequest = new CoreDto.UserLookupLoaderRequestDto
        {
            KeyWord = null,
            IsActive = true,
            DepartmentId = null,
            ResourceGroupId = resourceGroupId,
            Roles = [PortalRoleNames.GroupManager],
        };

        var activeGroupManagers = await _userLookupLoader.LoadAsync(
            lookupLoaderRequest,
            cancellationToken);

        if (activeGroupManagers.Count < settings.MaxGmOfSpecificGroupCount || resourceGroupId is null)
        {
            return BusinessRuleResult.Success();
        }
        else
        {
            return BusinessRuleResult.Fail($"Group manager of group with id {resourceGroupId} is already exists in the system");
        }
    }
}
