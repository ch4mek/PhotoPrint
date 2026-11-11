using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;

internal interface ICanTeamBoardUserBeRetrievedBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(
        int userId,
        CancellationToken cancellationToken);

    Task<bool> IsCorrespondsToBusiness(
        int userId,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);

    /// <summary>
    /// Return a dictionary, where the key is user id and the value means that user can/can't be retrieved
    /// </summary>
    Task<Dictionary<int, bool>> IsCorrespondsToBusinessBatch(
        ICollection<int> usersIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);
}

internal class CanTeamBoardUserBeRetrievedBusinessRule : ICanTeamBoardUserBeRetrievedBusinessRule
{
    private readonly IGetTeamBoardUsersWithNoIncludesDbQuery _getTeamBoardUsersWithNoIncludesDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackContract _timeTrackContract;

    public CanTeamBoardUserBeRetrievedBusinessRule(
        IGetTeamBoardUsersWithNoIncludesDbQuery getTeamBoardUsersWithNoIncludesDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackContract timeTrackContract)
    {
        _getTeamBoardUsersWithNoIncludesDbQuery = getTeamBoardUsersWithNoIncludesDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackContract = timeTrackContract;
    }

    public Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        return IsCorrespondsToBusiness(userId, AccessOptions.Self | AccessOptions.Manager, cancellationToken);
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        if (accessOptions.HasFlag(AccessOptions.Self) && _authenticatedUserContext.UserId == userId)
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Hr)
            && _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Coordinator) &&
            _authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name))
        {
            var managedUserIds =
                await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(),
                    cancellationToken);
            if (managedUserIds.Contains(userId))
            {
                return true;
            }
        }

        var user = (await _getTeamBoardUsersWithNoIncludesDbQuery.ExecuteAsync(userId, cancellationToken))!;

        if (user.IsActive == false)
        {
            throw new EntityConflictException(PortalMessages.UserIsInactive);
        }

        if (accessOptions.HasFlag(AccessOptions.Accountant)
            && _authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            return true;
        }

        if (accessOptions.HasFlag(AccessOptions.Manager) && ValidateManager(user))
        {
            return true;
        }

        if (user.Position.IsIntern()
            && (_authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name)
                || _authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageInterns.Name)
                    && user.CreatedById == _authenticatedUserContext.UserId))
        {
            return true;
        }

        return false;
    }

    public async Task<Dictionary<int, bool>> IsCorrespondsToBusinessBatch(
        ICollection<int> usersIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        var managedUserIds =
            await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(),
                cancellationToken);
        var usersToCheck = await _getTeamBoardUsersWithNoIncludesDbQuery.ExecuteRangeAsync(usersIds, cancellationToken);

        return usersToCheck
            .Where(user => UserHasAccess(user, accessOptions, managedUserIds))
            .ToDictionary(x => x.UserId, _ => true);
    }

    private bool UserHasAccess(
        BasicTeamBoardUserCoreDto user,
        AccessOptions accessOptions,
        List<int> managedUserIds)
    {
        return
            ((accessOptions.HasFlag(AccessOptions.Self)
                    && _authenticatedUserContext.UserId == user.UserId)
                || (accessOptions.HasFlag(AccessOptions.Hr)
                    && _authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
                || (accessOptions.HasFlag(AccessOptions.Coordinator)
                    && _authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name)
                    && managedUserIds.Contains(user.UserId))
                || (accessOptions.HasFlag(AccessOptions.Accountant)
                    && _authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
                || (accessOptions.HasFlag(AccessOptions.Manager)
                    && !user.Position.IsCto()
                    && !user.Position.IsCeo()
                    && ValidateManager(user)))
            && (user.IsActive ?? false);
    }

    private bool ValidateManager(BasicTeamBoardUserCoreDto user)
    {
        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId ||
            user.UserId == _authenticatedUserContext.UserId)
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId ||
            user.UserId == _authenticatedUserContext.UserId)
        {
            return false;
        }

        if (user.Position.IsCto() || user.Position.IsCeo())
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsIntern())
        {
            return false;
        }

        return true;
    }
}
