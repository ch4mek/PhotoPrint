using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;

internal interface IIsUserHasPermissionsToMutateContractsBusinessRule : IBusinessRuleWithFailReason<IsUserHasPermissionBusinessModel>;

internal sealed class IsUserHasPermissionsToMutateContractsBusinessRule : IIsUserHasPermissionsToMutateContractsBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsUserHasPermissionsToMutateContractsBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BusinessRuleResult> IsCorrespondsToBusiness(IsUserHasPermissionBusinessModel? model, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRequiredUserId() == model?.UserId)
        {
            return Task.FromResult(BusinessRuleResult.Fail("You don’t have an access to mutate your contracts"));
        }

        if (_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return Task.FromResult(BusinessRuleResult.Success());
        }

        if (!model!.IsActive || (model.Position is not null && (model.Position.IsCeo() || model.Position.IsCto())))
        {
            return Task.FromResult(BusinessRuleResult.Fail($"There is no user with Id {model.UserId}"));
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && model.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            return Task.FromResult(BusinessRuleResult.Fail("You don’t have an access to users from other departments"));
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            if (model.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                return Task.FromResult(BusinessRuleResult.Fail("You don’t have an access to users from other departments"));
            }

            if (model.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                return Task.FromResult(BusinessRuleResult.Fail("You don’t have an access to users from other groups"));
            }
        }

        return Task.FromResult(BusinessRuleResult.Success());
    }
}
