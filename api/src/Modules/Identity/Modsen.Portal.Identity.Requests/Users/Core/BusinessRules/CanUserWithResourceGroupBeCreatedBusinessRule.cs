using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanUserWithResourceGroupBeCreatedBusinessRule  : IBusinessRuleWithFailReason<CanUserWithResourceGroupBeCreatedBusinessRule.Request>;

internal class CanUserWithResourceGroupBeCreatedBusinessRule : ICanUserWithResourceGroupBeCreatedBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public CanUserWithResourceGroupBeCreatedBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var group = await _identityDbContext.ResourceGroups
            .Where(group => group.Id == model.ResourceGroupId)
            .Where(group => group.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var groupManager = await _identityDbContext.Users
            .Where(user => user.ResourceGroupId == model.ResourceGroupId)
            .Where(user => user.IsActive)
            .Where(user => user.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
            .FirstOrDefaultAsync(cancellationToken);

        if (group == null)
        {
            throw new EntityNotFoundException($"Resource group with id {model.ResourceGroupId} not found");
        }

        if (model.RoleName.IsGroupManager() && groupManager?.Id != null)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupAlreadyHasManager);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            _authenticatedUserContext.DepartmentId != group.DepartmentId &&
            !_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return BusinessRuleResult.Fail(PortalMessages.HeadCannotAssignUserToGroupFromAnotherDepartment);
        }

        return model.DepartmentId != group.DepartmentId
            ? BusinessRuleResult.Fail(PortalMessages.ResourceGroupMustBeFromDepartment)
            : BusinessRuleResult.Success();
    }

    internal record Request(string? DepartmentId, int ResourceGroupId, string RoleName);
}
