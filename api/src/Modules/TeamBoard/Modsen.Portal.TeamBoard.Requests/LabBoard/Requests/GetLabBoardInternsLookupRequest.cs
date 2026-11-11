using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;

internal sealed class GetLabBoardInternsLookupRequest : IQueryHandler<GetLabBoardInternsLookupRequest.Query, List<LabBoardInternRequestDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetLabBoardInternsLookupRequest(ITeamBoardDbContext teamBoardDbContext, IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<LabBoardInternRequestDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var labBoardInterns = await GetUsersAsync(request.IncludeInactive, cancellationToken);

        return labBoardInterns;
    }

    private Task<List<LabBoardInternRequestDto>> GetUsersAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.TeamBoardUsers
            .AsNoTracking()
            .OrderBy(user => user.UserId)
            .Where(user => user.Position == PortalRoleNames.Intern);

        query = GetFilteredQuery(query, includeInactive);

        return query.Select(user => new LabBoardInternRequestDto
        {
            Id = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FirstNameRu = user.FirstNameRu,
            LastNameRu = user.LastNameRu,
        })
            .ToListAsync(cancellationToken);
    }

    private IQueryable<TeamBoardUser> GetFilteredQuery(IQueryable<TeamBoardUser> query, bool includeInactive)
    {
        if (!includeInactive)
        {
            query = query.Where(user => user.IsActive);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            !_authenticatedUserContext.GetUserClaimsNames().Contains(LabBoardClaimTypeNames.ManageAllInterns.Name))
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return query;
    }

    internal sealed record Query(bool IncludeInactive) : IQuery<List<LabBoardInternRequestDto>>;
}
