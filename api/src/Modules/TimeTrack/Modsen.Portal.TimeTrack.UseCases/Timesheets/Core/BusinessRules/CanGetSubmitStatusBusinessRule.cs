using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface ICanGetSubmitStatusBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal class CanGetSubmitStatusBusinessRule : ICanGetSubmitStatusBusinessRule
{
    private readonly IGetTimeTrackUserDbQuery _getTimeTrackUserDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanGetSubmitStatusBusinessRule(IAuthenticatedUserContext authenticatedUserContext,
        IGetTimeTrackUserDbQuery getTimeTrackUserDbQuery)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getTimeTrackUserDbQuery = getTimeTrackUserDbQuery;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var user = await _getTimeTrackUserDbQuery.ExecuteAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            return false;
        }

        return _authenticatedUserContext.GetRoles() switch
        {
            var roles when roles.IsCto() || roles.IsCeo() || roles.IsHeadOfDepartment() || roles.IsGroupManager()
                => true,

            var roles when (roles.IsEmployee() || roles.IsSystemAdministrator()) &&
                _authenticatedUserContext.UserId == userId => true,

            _ => false
        };
    }
}
