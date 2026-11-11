using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.DbQueries;

internal interface IGetPerformanceReviewByConversationIdDbQuery
{
    Task<PerformanceReviewCoreDto?> ExecuteAsync(int conversationId, CancellationToken cancellationToken);
}

internal class GetUserPerformanceReviewByConversationIdDbQuery : IGetPerformanceReviewByConversationIdDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetUserPerformanceReviewByConversationIdDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PerformanceReviewCoreDto?> ExecuteAsync(int conversationId, CancellationToken cancellationToken)
    {
        var performanceReview = await _dbContext.UserPerformanceReviews.FirstOrDefaultAsync(x => x.ConversationId == conversationId, cancellationToken);

        if (performanceReview == null)
        {
            return null;
        }


        return new PerformanceReviewCoreDto(
            performanceReview.Id,
            performanceReview.InterviewDateFromUtc,
            performanceReview.Status.ToString()
        );
    }
}
