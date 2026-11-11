using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;

internal interface IHasUserAccessToInquiryBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, AccessOptions accessOptions, CancellationToken cancellationToken);
}

internal class HasUserAccessToInquiryBusinessRule : IHasUserAccessToInquiryBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;
    private readonly ITimeTrackContract _timeTrackContract;

    public HasUserAccessToInquiryBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract,
        ITimeTrackContract timeTrackContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
        _timeTrackContract = timeTrackContract;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        if (userId == _authenticatedUserContext.GetRequiredUserId() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name))
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Hr) &&
            _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Accountant)
            && _authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Coordinator)
            && _authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name))
        {
            var projectUsers = await _timeTrackContract.GetUserIdsByProjectManagerAsync(
                _authenticatedUserContext.GetRequiredUserId(), cancellationToken);

            if (projectUsers.Contains(userId))
            {
                return true;
            }            
        }


        if (!accessOptions.HasFlag(AccessOptions.Manager))
        {
            return false;
        }

        var higherManagerIds = await _identityContract.GetHigherManagerIds(userId, cancellationToken);

        if (higherManagerIds.Contains(_authenticatedUserContext.GetRequiredUserId()))
        {
            return true;
        }

        return false;
    }
}
