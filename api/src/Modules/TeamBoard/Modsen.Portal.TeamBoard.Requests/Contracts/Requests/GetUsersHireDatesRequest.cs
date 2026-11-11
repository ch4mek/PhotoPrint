using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests.RequestDtos;

namespace Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

internal sealed class GetUsersHireDatesRequest : IQueryHandler<GetUsersHireDatesRequest.Query, List<UserHireDateDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetUsersHireDatesRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<List<UserHireDateDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.TeamBoardUsers
           .Where(x => x.IsActive)
           .Where(x => x.HireDate != null)
           .Select(x => new UserHireDateDto
           {
               UserId = x.UserId,
               HireDate = x.HireDate!.Value,

           })
           .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<UserHireDateDto>>;
}
