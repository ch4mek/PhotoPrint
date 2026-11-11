using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules.Models;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;

internal interface IIsUserHasPermissionsToViewContractsBusinessRule : IBusinessRuleWithFailReason<IsUserHasPermissionBusinessModel>;

internal sealed class IsUserHasPermissionsToViewContractsBusinessRule : IIsUserHasPermissionsToViewContractsBusinessRule
{
    private readonly IIsUserHasPermissionsToMutateContractsBusinessRule _isUserHasPermissionsToMutateContactsBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsUserHasPermissionsToViewContractsBusinessRule(
        IIsUserHasPermissionsToMutateContractsBusinessRule isUserHasPermissionsToMutateContactsBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _isUserHasPermissionsToMutateContactsBusinessRule = isUserHasPermissionsToMutateContactsBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BusinessRuleResult> IsCorrespondsToBusiness(IsUserHasPermissionBusinessModel? model, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            return Task.FromResult(BusinessRuleResult.Success());
        }

        return _isUserHasPermissionsToMutateContactsBusinessRule.IsCorrespondsToBusiness(model, cancellationToken);
    }
}
