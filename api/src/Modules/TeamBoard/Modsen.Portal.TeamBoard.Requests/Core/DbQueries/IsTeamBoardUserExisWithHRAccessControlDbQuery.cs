using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IIsTeamBoardUserExistWithHRAccessControlDbQuery
{
    Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken);
}

internal class IsTeamBoardUserExistWithHRAccessControlDbQuery
    : IIsTeamBoardUserExistWithHRAccessControlDbQuery
{
    private readonly TeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsTeamBoardUserExistWithHRAccessControlDbQuery(TeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<bool> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            if (!_authenticatedUserContext.IsManager)
            {
                var currentUserId = _authenticatedUserContext.UserId;
                return _teamBoardDbContext.TeamBoardUsers.AnyAsync(user =>
                        userId == currentUserId &&
                        user.UserId == currentUserId &&
                        user.IsActive,
                    cancellationToken);
            }

            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
            {
                return _teamBoardDbContext.TeamBoardUsers.AnyAsync(user =>
                        user.UserId == userId &&
                        user.IsActive &&
                        user.DepartmentId == _authenticatedUserContext.DepartmentId,
                    cancellationToken);
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager())
            {
                return _teamBoardDbContext.TeamBoardUsers.AnyAsync(user =>
                        user.UserId == userId &&
                        user.IsActive &&
                        user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId,
                    cancellationToken);
            }
        }

        return _teamBoardDbContext.TeamBoardUsers.AnyAsync(user =>
            user.UserId == userId &&
            user.IsActive,
            cancellationToken);
    }
}
