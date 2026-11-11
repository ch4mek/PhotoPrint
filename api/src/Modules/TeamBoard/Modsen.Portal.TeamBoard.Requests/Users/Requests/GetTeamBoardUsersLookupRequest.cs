using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests;

internal sealed class GetTeamBoardUsersLookupRequest : IQueryHandler<GetTeamBoardUsersLookupRequest.Query, List<TeamBoardUserRequestDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetTeamBoardUsersLookupRequest(ITeamBoardDbContext teamBoardDbContext, IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<TeamBoardUserRequestDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var teamBoardUsers = await GetUsersAsync(request.IncludeInactive, cancellationToken);

        return teamBoardUsers;
    }

    private Task<List<TeamBoardUserRequestDto>> GetUsersAsync(bool includeInactive,
        CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.TeamBoardUsers.AsNoTracking()
            .OrderBy(user => user.UserId)
            .Where(user =>
                user.Position != PortalRoleNames.Intern &&
                user.Position != PortalRoleNames.Ceo &&
                user.Position != PortalRoleNames.Cto);

        if (!includeInactive)
        {
            query = query.Where(user => user.IsActive);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(user => user.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if(_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(user=> user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return query.Select(user => new TeamBoardUserRequestDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FirstNameRu = user.FirstNameRu,
                LastNameRu = user.LastNameRu,
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(bool IncludeInactive) : IQuery<List<TeamBoardUserRequestDto>>;
}
