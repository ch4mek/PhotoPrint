using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanCurrentUserCreateUserWithRoleBusinessRule :
    IBusinessRuleWithFailReason<CanCurrentUserCreateUserWithRoleBusinessRule.Request>;

internal class CanCurrentUserCreateUserWithRoleBusinessRule : ICanCurrentUserCreateUserWithRoleBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanCurrentUserCreateUserWithRoleBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            && (!request.Position.IsEmployee() || _authenticatedUserContext.DepartmentId != request.DepartmentId)
            && (!request.Position.IsIntern() || _authenticatedUserContext.DepartmentId != request.DepartmentId)
            && !_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return Task.FromResult(BusinessRuleResult.Fail($"You can create only Employee(Intern) with {_authenticatedUserContext.DepartmentId} department"));
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager()
            && (!request.Position.IsEmployee() || _authenticatedUserContext.ResourceGroupId != request.ResourceGroupId)
            && (!request.Position.IsIntern() || _authenticatedUserContext.DepartmentId != request.DepartmentId)
            && !_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return Task.FromResult(BusinessRuleResult.Fail($"You can create only Employee(Intern) with resource group id {_authenticatedUserContext.ResourceGroupId}"));
        }

        return Task.FromResult(request.Position.IsCeo()
            ? BusinessRuleResult.Fail(PortalMessages.YouCannotCreateCeo)
            : BusinessRuleResult.Success());
    }

    internal record Request(string Position, string? DepartmentId, int? ResourceGroupId);
}
