using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface ICanGetUserTimesheetBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, bool? onlyManagedProjects, CancellationToken cancellationToken);
}

internal class CanGetUserTimesheetBusinessRule : ICanGetUserTimesheetBusinessRule
{
    private readonly IIsUserManagerOfAnyPublicProjectDbQuery _isUserManagerOfAnyPublicProjectDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimeTrackUserDbQuery _getUserDbQuery;
    private readonly ITimeTrackDbContext _dbContext;

    public CanGetUserTimesheetBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IIsUserManagerOfAnyPublicProjectDbQuery isUserManagerOfAnyPublicProjectDbQuery,
        IGetTimeTrackUserDbQuery getUserDbQuery,
        ITimeTrackDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _isUserManagerOfAnyPublicProjectDbQuery = isUserManagerOfAnyPublicProjectDbQuery;
        _getUserDbQuery = getUserDbQuery;
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, bool? onlyManagedProjects,
        CancellationToken cancellationToken)
    {
        var roles = _authenticatedUserContext.GetRoles();
        var user = await _getUserDbQuery.ExecuteAsync(userId, cancellationToken);
        if (user is null)
        {
            return false;
        }

        if (roles.IsGroupManager())
        {
            return IsGroupManagerAllowed(userId, onlyManagedProjects, user);
        }

        if (IsHeadOrEmployeeRequestingManagedProjects(roles, onlyManagedProjects))
        {
            return await IsHeadOrEmployeeAllowedAsync(userId, roles, user, cancellationToken);
        }

        return IsDefaultRoleAllowed(roles, userId);
    }

    private bool IsGroupManagerAllowed(int userId, bool? onlyManagedProjects, TimeTrackUser user)
    {
        if (_authenticatedUserContext.UserId == userId)
        {
            return true;
        }

        if (onlyManagedProjects == true)
        {
            return false;
        }

        return _authenticatedUserContext.ResourceGroupId == user.ResourceGroupId;
    }

    private static bool IsHeadOrEmployeeRequestingManagedProjects(IReadOnlyCollection<string> roles, bool? onlyManagedProjects)
    {
        return roles.IsHeadOfDepartment() || (roles.IsEmployee() && onlyManagedProjects == true);
    }

    private async Task<bool> IsHeadOrEmployeeAllowedAsync(int userId, IReadOnlyCollection<string> roles, TimeTrackUser user,
        CancellationToken cancellationToken)
    {
        var isCurrentUserManagerOfAnyUserProjects = await _dbContext.Projects.AnyAsync(project =>
            project.IsActive &&
            project.ManagerUserId == _authenticatedUserContext.UserId &&
            project.ProjectUsers.Any(projectUser => projectUser.UserId == userId),
            cancellationToken);

        var isCurrentUserManagerOfAnyUserTrackedTimeProjects = await _dbContext.Projects.AnyAsync(project =>
            project.IsActive &&
            project.ManagerUserId == _authenticatedUserContext.UserId &&
            project.Timesheets.Any(timesheet => timesheet.UserId == userId),
            cancellationToken);

        var isUserManagerOfAnyPublicProject = await _isUserManagerOfAnyPublicProjectDbQuery.ExecuteAsync(
            _authenticatedUserContext.UserId,
            cancellationToken);

        var isUserFromDepartment = IsUserFromDepartment(roles, user);
        return isUserFromDepartment ||
            isCurrentUserManagerOfAnyUserProjects ||
            isCurrentUserManagerOfAnyUserTrackedTimeProjects ||
            isUserManagerOfAnyPublicProject;
    }

    private bool IsUserFromDepartment(IReadOnlyCollection<string> roles, TimeTrackUser user)
    {
        return roles.IsHeadOfDepartment() && user.DepartmentId == _authenticatedUserContext.DepartmentId;
    }

    private bool IsDefaultRoleAllowed(IReadOnlyCollection<string> roles, int userId)
    {
        if (roles.IsCto() || roles.IsCeo())
        {
            return true;
        }

        return (roles.IsEmployee() || roles.IsSystemAdministrator()) && _authenticatedUserContext.UserId == userId;
    }
}
