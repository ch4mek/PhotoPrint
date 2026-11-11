using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

internal interface ICanUserUpdateGroupBusinessRule
    : IBusinessRuleWithFailReason<CanUserUpdateGroupBusinessRule.Request>;

internal sealed class CanUserUpdateGroupBusinessRule : ICanUserUpdateGroupBusinessRule
{
    private readonly IIdentityDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserUpdateGroupBusinessRule(IIdentityDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }
    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var isDepartmentExists = await _dbContext.Departments.AnyAsync(
            department => department.Id == model.DepartmentId,
            cancellationToken);

        if (!isDepartmentExists)
        {
            return BusinessRuleResult.Fail(PortalMessages.DepartmentNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            return _authenticatedUserContext.DepartmentId != model.DepartmentId
                ? BusinessRuleResult.Fail(PortalMessages.HeadCannotUpdateGroupFromAnotherDepartment)
                : BusinessRuleResult.Success();
        }

        return BusinessRuleResult.Success();
    }

    internal record Request(string DepartmentId);

}
