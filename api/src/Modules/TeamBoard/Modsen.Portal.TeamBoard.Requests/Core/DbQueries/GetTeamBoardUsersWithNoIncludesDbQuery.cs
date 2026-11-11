using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IGetTeamBoardUsersWithNoIncludesDbQuery
{
    Task<BasicTeamBoardUserCoreDto?> ExecuteAsync(int userId, CancellationToken cancellationToken);
    Task<ICollection<BasicTeamBoardUserCoreDto>> ExecuteRangeAsync(ICollection<int> usersIds, CancellationToken cancellationToken);
}

internal class GetTeamBoardUsersWithNoIncludesDbQuery : IGetTeamBoardUsersWithNoIncludesDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetTeamBoardUsersWithNoIncludesDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<BasicTeamBoardUserCoreDto?> ExecuteAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers.FindAsync([userId], cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        return BasicTeamBoardUserCoreDto.Of(user);
    }

    public async Task<ICollection<BasicTeamBoardUserCoreDto>> ExecuteRangeAsync(ICollection<int> usersIds, CancellationToken cancellationToken)
    {
        var uniqueUsersIds = usersIds.Distinct().ToArray();

        var users = await _teamBoardDbContext.TeamBoardUsers
            .Where(x => uniqueUsersIds.Contains(x.UserId))
            .Select(BasicTeamBoardUserCoreDto.ExpressionOf)
            .ToArrayAsync(cancellationToken: cancellationToken);

        if (users.Length != uniqueUsersIds.Length)
        {
            throw new EntityNotFoundException("One of the users wasn't found.");
        }

        return users;
    }
}
