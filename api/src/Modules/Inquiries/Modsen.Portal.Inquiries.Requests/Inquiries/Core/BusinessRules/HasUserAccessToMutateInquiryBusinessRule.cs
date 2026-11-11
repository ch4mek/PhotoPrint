using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;

internal interface IHasUserAccessToMutateInquiryBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal class HasUserAccessToMutateInquiryBusinessRule : IHasUserAccessToMutateInquiryBusinessRule
{
    private readonly IIdentityContract _identityContract;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public HasUserAccessToMutateInquiryBusinessRule(IIdentityContract identityContract,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _identityContract = identityContract;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        if (userId == _authenticatedUserContext.GetRequiredUserId() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.GetRoles().IsCeo())
        {
            return true;
        }

        var higherManagerIds = await _identityContract.GetHigherManagerIds(userId, cancellationToken);

        return higherManagerIds.Contains(_authenticatedUserContext.GetRequiredUserId());
    }
}
