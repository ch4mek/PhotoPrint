using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface ICanUserUpdateTimesheetBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal sealed class CanUserUpdateTimesheetBusinessRule : ICanUserUpdateTimesheetBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimeTrackUserDbQuery _getUserDbQuery;
    private readonly IGetUserActiveStatusDbQuery _getUserActiveStatusDbQuery;

    public CanUserUpdateTimesheetBusinessRule(IGetTimeTrackUserDbQuery getUserDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetUserActiveStatusDbQuery getUserActiveStatusDbQuery)
    {
        _getUserDbQuery = getUserDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _getUserActiveStatusDbQuery = getUserActiveStatusDbQuery;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var userDetails = await _getUserDbQuery.ExecuteAsync(userId, cancellationToken);

        if (userDetails is null)
        {
            throw new InvalidDataAppException(PortalMessages.UserNotFound);
        }

        var userActiveStatus = await _getUserActiveStatusDbQuery.ExecuteAsync(
            userId,
            cancellationToken);

        if (!userActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetOfInactiveUser);
        }

        if (!_authenticatedUserContext.IsManager)
        {
            if (userId != _authenticatedUserContext.UserId)
            {
                return false;
            }
        }
        else
        {
            var currentRoles = _authenticatedUserContext.GetRoles();
            var currentDepartmentId = _authenticatedUserContext.DepartmentId;
            var currentResourceGroupId = _authenticatedUserContext.ResourceGroupId;

            if (currentRoles.IsGroupManager() && userDetails.ResourceGroupId != currentResourceGroupId)
            {
                return false;
            }

            if (currentRoles.IsHeadOfDepartment() && userDetails.DepartmentId != currentDepartmentId)
            {
                return false;
            }
        }

        return true;
    }
}
