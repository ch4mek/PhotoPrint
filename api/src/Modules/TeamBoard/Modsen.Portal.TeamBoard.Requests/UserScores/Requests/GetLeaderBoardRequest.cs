using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.UserScores.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserScores.Requests;

internal sealed class GetLeaderBoardRequest : IQueryHandler<GetLeaderBoardRequest.Query, List<UserScoreDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private const int LeadersCount = 10;

    public GetLeaderBoardRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<UserScoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var players = await _teamBoardDbContext.UserScores
            .OrderByDescending(x => x.Score)
            .Take(LeadersCount)
            .Select(x => new UserScoreDto
            {
                FirstName = x.User!.FirstName,
                LastName = x.User.LastName,
                FirstNameRu = x.User!.FirstNameRu,
                LastNameRu = x.User.LastNameRu,
                Score = x.Score,
                AvatarId = x.User.AvatarFileId,
                DepartmentName = x.User.Department == null ? null : x.User.Department.FullName,
            }).ToListAsync(cancellationToken);

        return players;
    }

    internal sealed record Query() : IQuery<List<UserScoreDto>>;
}
