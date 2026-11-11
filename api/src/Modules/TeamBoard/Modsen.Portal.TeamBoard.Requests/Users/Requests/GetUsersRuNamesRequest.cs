using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests;

internal sealed class GetUsersRuNamesRequest
    : IQueryHandler<GetUsersRuNamesRequest.Query, List<UserWithRuNameOutput>>
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetUsersRuNamesRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<UserWithRuNameOutput>> Handle(Query request, CancellationToken cancellationToken)
    {
        return _dbContext.TeamBoardUsers
            .Where(user => request.Ids.Contains(user.UserId))
            .Select(user => new UserWithRuNameOutput(user.UserId, user.FirstNameRu, user.LastNameRu))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int[] Ids) : IQuery<List<UserWithRuNameOutput>>;
}
