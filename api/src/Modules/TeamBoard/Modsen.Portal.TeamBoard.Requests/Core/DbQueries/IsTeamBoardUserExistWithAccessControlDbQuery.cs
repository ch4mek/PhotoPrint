using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IIsTeamBoardUserExistWithAccessControlDbQuery
{
    Task<bool> ExecuteAsync(int userId, bool includeInterns = false, CancellationToken cancellationToken = default);
}

internal class IsTeamBoardUserExistWithAccessControlDbQuery
    : IIsTeamBoardUserExistWithAccessControlDbQuery
{
    private readonly TeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsTeamBoardUserExistWithAccessControlDbQuery(TeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<bool> ExecuteAsync(int userId, bool includeInterns = false, CancellationToken cancellationToken = default)
    {
        var currentUserId = _authenticatedUserContext.UserId;

        var query = _teamBoardDbContext.TeamBoardUsers.AsQueryable();
        query = includeInterns ? query : query.Where(x => x.Position != PortalRoleNames.Intern);

        if (!_authenticatedUserContext.IsManager)
        {            
            return query.AnyAsync(user =>
                userId == currentUserId &&
                user.UserId == currentUserId &&
                user.IsActive,
                cancellationToken);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            return query.AnyAsync(user =>
                user.UserId == userId &&
                user.IsActive &&
                user.DepartmentId == _authenticatedUserContext.DepartmentId,
                cancellationToken);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            return query.AnyAsync(user =>
                user.UserId == userId &&
                user.IsActive &&
                (user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                    || userId == currentUserId), //for gm without group
                cancellationToken);
        }

        return query.AnyAsync(user =>
            user.UserId == userId &&
            user.IsActive,
            cancellationToken);
    }
}
