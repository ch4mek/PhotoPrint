using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Dto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.BusinessRules;

internal interface ICanAccessUserAssignsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(
        int userId,
        CancellationToken cancellationToken);

    Task<bool> IsCorrespondsToBusiness(
        int userId,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);
}

internal sealed class CanAccessUserAssignsBusinessRule : ICanAccessUserAssignsBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;
    private readonly ITimeTrackContract _timeTrackContract;

    public CanAccessUserAssignsBusinessRule(IAuthenticatedUserContext authenticatedUserContext,
        IUserLoader userLoader,
        ITimeTrackContract timeTrackContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _userLoader = userLoader;
        _timeTrackContract = timeTrackContract;
    }

    public Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        const AccessOptions defaultAccessOptions = AccessOptions.Manager;

        return IsCorrespondsToBusiness(userId, defaultAccessOptions, cancellationToken);
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        var user = await _userLoader.GetUserByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        if (_authenticatedUserContext.UserId == userId)
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Hr)
            && _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
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
            var currentUserId = _authenticatedUserContext.GetRequiredUserId();
            if (user.Id == currentUserId)
            {
                return true;
            }

            var managedUserIds = await _timeTrackContract.GetUserIdsByProjectManagerAsync(currentUserId, cancellationToken);
            if (managedUserIds.Contains(userId))
            {
                return true;
            }
        }

        var roles = _authenticatedUserContext.GetRoles();
        if (roles.IsCeo() || roles.IsCto())
        {
            return true;
        }

        return IsCorrespondsToBusinessForManager(accessOptions, user);
    }

    private bool IsCorrespondsToBusinessForManager(AccessOptions accessOptions, InquiriesUserDto user)
    {
        return accessOptions.HasFlag(AccessOptions.Manager) && (
            _authenticatedUserContext.IsHeadOfDepartmentFrom(user.DepartmentId) ||
            _authenticatedUserContext.IsGroupManagerFrom(user.ResourceGroupId));
    }
}
