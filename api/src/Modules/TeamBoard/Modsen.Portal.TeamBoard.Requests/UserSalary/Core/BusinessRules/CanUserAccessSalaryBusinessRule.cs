using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;

internal interface ICanUserAccessSalaryBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken);
}

internal sealed class CanUserAccessSalaryBusinessRule : ICanUserAccessSalaryBusinessRule
{
    private readonly IGetTeamBoardUsersWithNoIncludesDbQuery _getUsersDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserAccessSalaryBusinessRule(IGetTeamBoardUsersWithNoIncludesDbQuery getUsersDbQuery,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _getUsersDbQuery = getUsersDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var user = await _getUsersDbQuery.ExecuteAsync(userId, cancellationToken);

        if (user!.IsActive.HasValue && !user.IsActive.Value)
        {
            return false;
        }

        if (user.UserId == _authenticatedUserContext.UserId)
        {
            return true;
        }

        if (_authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsIntern() ||
            _authenticatedUserContext.GetRoles().IsGroupManager())
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            && user.DepartmentId == _authenticatedUserContext.DepartmentId)
        {
            return true;
        }

        if (_authenticatedUserContext.GetRoles().IsCto() && user.Position.IsCeo())
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsCeo() && user.Position.IsCto())
        {
            return false;
        }

        if (_authenticatedUserContext.GetRoles().IsCto() || _authenticatedUserContext.GetRoles().IsCeo())
        {
            return true;
        }

        return false;
    }
}
