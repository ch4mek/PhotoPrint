using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

internal interface IIsNewGroupManagerValidBusinessRule : IBusinessRuleWithFailReason<IsNewGroupManagerValidBusinessRule.Request>;

internal sealed class IsNewGroupManagerValidBusinessRule : IIsNewGroupManagerValidBusinessRule
{
    private readonly IIdentityDbContext _dbContext;

    public IsNewGroupManagerValidBusinessRule(IIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var groupManager = await _dbContext.Users
            .Include(user => user.ResourceGroup)
            .Where(u =>
                u.Id == model.GroupManagerId &&
                u.IsActive &&
                u.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.GroupManager))
            .FirstOrDefaultAsync(cancellationToken);

        if (groupManager == null)
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }

        if (groupManager.ResourceGroupId != null && groupManager.ResourceGroup!.IsActive)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupManagerAlreadyHasActiveGroup);
        }

        if (groupManager.DepartmentId != model.DepartmentId)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupManagerIsFromAnotherDepartment);
        }

        return BusinessRuleResult.Success();
    }

    internal record Request(int GroupManagerId, string DepartmentId);
}


