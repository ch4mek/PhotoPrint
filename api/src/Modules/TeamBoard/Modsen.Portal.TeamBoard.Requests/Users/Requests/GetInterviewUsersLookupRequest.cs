using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests;

internal sealed class GetInterviewUsersLookupRequest : IQueryHandler<GetInterviewUsersLookupRequest.Query, List<TeamBoardUserRequestDto>>
{
    private readonly TeamBoardDbContext _dbContext;
    private readonly IInterviewsAuthenticationFilter _interviewsAuthenticationFilter;

    public GetInterviewUsersLookupRequest(TeamBoardDbContext dbContext,
        IInterviewsAuthenticationFilter interviewsAuthenticationFilter)
    {
        _dbContext = dbContext;
        _interviewsAuthenticationFilter = interviewsAuthenticationFilter;
    }

    public async Task<List<TeamBoardUserRequestDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _dbContext.TeamBoardUsers
            .AsNoTracking()
            .Where(x => x.Position != PortalRoleNames.Intern);

        query = await  _interviewsAuthenticationFilter.Apply(query, cancellationToken);

        if (request.OnlyActive == true)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query.Select(x => new TeamBoardUserRequestDto
        {
            Id = x.UserId,
            FirstName = x.FirstName,
            LastName = x.LastName,
            FirstNameRu = x.FirstNameRu,
            LastNameRu = x.LastNameRu
        }).OrderBy(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(bool? OnlyActive) : IQuery<List<TeamBoardUserRequestDto>>;
}
