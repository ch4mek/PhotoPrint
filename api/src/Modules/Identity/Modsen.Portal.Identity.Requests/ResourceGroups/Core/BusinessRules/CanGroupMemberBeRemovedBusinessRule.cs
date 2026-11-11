using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

internal interface ICanGroupMemberBeRemovedBusinessRule
    : IBusinessRuleWithFailReason<CanGroupMemberBeRemovedBusinessRule.Request>;

internal sealed class CanGroupMemberBeRemovedBusinessRule : ICanGroupMemberBeRemovedBusinessRule
{
    private readonly ICanUserUpdateGroupBusinessRule _canUserUpdateGroupBusinessRule;
    private readonly IIdentityDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanGroupMemberBeRemovedBusinessRule(
        ICanUserUpdateGroupBusinessRule canUserUpdateGroupBusinessRule,
        IIdentityDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canUserUpdateGroupBusinessRule = canUserUpdateGroupBusinessRule;
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(user => user.Id == model!.UserId && user.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var groupDepartmentId = await _dbContext.ResourceGroups
            .Where(group => group.Id == model!.GroupId)
            .Select(group => group.DepartmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }

        if (groupDepartmentId == null)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupNotFound);
        }

        if (user.ResourceGroupId != model!.GroupId)
        {
            return BusinessRuleResult.Fail(PortalMessages.UserIsNotGroupMember);
        }

        var isUserEmployee = user.UserRoles.First().Role!.Name.IsEmployee();
        if (!isUserEmployee)
        {
            return BusinessRuleResult.Fail(PortalMessages.OnlyEmployeeGroupMemberCanBeUpdated);
        }

        var canHeadUpdateGroupFromAnotherDept = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(
            new CanUserUpdateGroupBusinessRule.Request(groupDepartmentId),
            cancellationToken);

        if (!canHeadUpdateGroupFromAnotherDept.IsSuccess)
        {
            return canHeadUpdateGroupFromAnotherDept;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            return user.DepartmentId != _authenticatedUserContext.DepartmentId
                ? BusinessRuleResult.Fail(PortalMessages.HeadCannotRemoveUserFromAnotherDepartmentFromGroup)
                : BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Success();
    }

    internal sealed record Request(int UserId, int GroupId);
}
