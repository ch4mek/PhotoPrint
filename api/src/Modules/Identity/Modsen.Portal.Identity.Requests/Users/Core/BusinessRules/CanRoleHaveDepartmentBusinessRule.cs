using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanRoleHaveDepartmentBusinessRule : IBusinessRuleWithFailReason<CanRoleHaveDepartmentBusinessRule.Request>;

internal class CanRoleHaveDepartmentBusinessRule : ICanRoleHaveDepartmentBusinessRule
{
    private readonly IIdentityDbContext _identityDbContext;

    public CanRoleHaveDepartmentBusinessRule(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        if (model is null)
        {
            return BusinessRuleResult.Fail(PortalMessages.ModelIsNull);
        }

        var (position, departmentId) = model;

        var isRoleDefined = await _identityDbContext.Roles
            .AnyAsync(r => position == r.Name, cancellationToken);

        if (!isRoleDefined)
        {
            return BusinessRuleResult.Fail(PortalMessages.UnsupportedRoleTypes);
        }

        return position switch
        {
            var role when (role.IsCeo() || role.IsCto()) && departmentId is not null => 
                BusinessRuleResult.Fail(PortalMessages.CtoAndCeoCannotHaveDepartment),

            var role when (role.IsSystemAdministrator() || role.IsHeadOfDepartment() || role.IsEmployee() || role.IsGroupManager() || role.IsIntern())
                && departmentId is null =>
                BusinessRuleResult.Fail(PortalMessages.SysAdminHeadGmEmployeeAndInternMustHaveDepartment),

            _ => BusinessRuleResult.Success()
        };
    }

    internal record Request(string? Position, string? DepartmentId);
}
