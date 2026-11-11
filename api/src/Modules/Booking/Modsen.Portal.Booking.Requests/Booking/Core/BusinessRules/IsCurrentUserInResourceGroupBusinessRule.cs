using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;

internal interface IIsCurrentUserInResourceGroupBusinessRule
{
    bool IsCorrespondsToBusiness(int? resourceGroupId);
}

internal class IsCurrentUserInResourceGroupBusinessRule : IIsCurrentUserInResourceGroupBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsCurrentUserInResourceGroupBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public bool IsCorrespondsToBusiness(int? resourceGroupId)
    {
        return !_authenticatedUserContext.GetRoles().IsGroupManager() || _authenticatedUserContext.ResourceGroupId == resourceGroupId;
    }
}
