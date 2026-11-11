using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.DbQueries;

internal interface IIsConversationExistingDbQuery
{
    Task<bool> ExecuteAsync(int conversationId, CancellationToken cancellationToken);
}


internal class IsConversationExistingDbQuery : IIsConversationExistingDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public IsConversationExistingDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<bool> ExecuteAsync(int conversationId, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.Conversations.AsNoTracking().AnyAsync(x => x.Id == conversationId,
            cancellationToken);
    }
}
