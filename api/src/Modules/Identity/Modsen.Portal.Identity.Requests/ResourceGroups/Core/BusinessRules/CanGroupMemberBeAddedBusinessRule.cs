using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

internal interface ICanGroupMemberBeAddedBusinessRule
    : IBusinessRuleWithFailReason<CanGroupMemberBeAddedBusinessRule.Request>;

internal sealed class CanGroupMemberBeAddedBusinessRule : ICanGroupMemberBeAddedBusinessRule
{
    private readonly ICanUserUpdateGroupBusinessRule _canUserUpdateGroupBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanGroupMemberBeAddedBusinessRule(
        ICanUserUpdateGroupBusinessRule canUserUpdateGroupBusinessRule,
        IIdentityDbContext identityDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canUserUpdateGroupBusinessRule = canUserUpdateGroupBusinessRule;
        _identityDbContext = identityDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        var userDetails = await _identityDbContext.Users
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .Where(user => user.Id == model!.UserId && user.IsActive)            
            .Select(user => new
            {
                Position = user.UserRoles.First().Role!.Name,
                user.ResourceGroupId,
                user.Email,
                user.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userDetails is null)
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }

        var group = await _identityDbContext.ResourceGroups
            .Where(resourceGroup => resourceGroup.Id == model!.GroupId)
            .Select(group => new
            {
                group.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupNotFound);
        }

        if (!userDetails.Position.IsEmployee())
        {
            return BusinessRuleResult.Fail(PortalMessages.OnlyEmployeeGroupMemberCanBeUpdated);
        }

        var isUserAlreadyInActiveGroup = userDetails.ResourceGroupId == model!.GroupId || userDetails.ResourceGroupId != null;

        if (isUserAlreadyInActiveGroup)
        {
            return BusinessRuleResult.Fail($"User {userDetails.Email} is already in the group");
        }

        var isUserCabBeAddedInGroup = userDetails.DepartmentId == group.DepartmentId;

        if (!isUserCabBeAddedInGroup)
        {
            return BusinessRuleResult.Fail("It shouldn't be possible to assign User to group from another department ");
        }

        var canHeadUpdateGroupFromAnotherDept = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(
            new CanUserUpdateGroupBusinessRule.Request(group.DepartmentId),
            cancellationToken);

        if (!canHeadUpdateGroupFromAnotherDept.IsSuccess)
        {
            return canHeadUpdateGroupFromAnotherDept;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            return userDetails.DepartmentId != _authenticatedUserContext.DepartmentId
                ? BusinessRuleResult.Fail(PortalMessages.HeadCannotAssignUserFromAnotherDepartmentToGroup)
                : BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Success();
    }

    internal sealed record Request(int UserId, int GroupId);
}
