using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.DbQueries;

internal interface IIsTheSamePerformanceReviewDbQuery
{
    Task<bool> ExecuteAsync(int performanceReviewId, int userId, DateTime date, CancellationToken cancellationToken);
}
internal class IsTheSamePerformanceReviewDbQuery : IIsTheSamePerformanceReviewDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public IsTheSamePerformanceReviewDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExecuteAsync(int performanceReviewId, int userId, DateTime date, CancellationToken cancellationToken)
    {
        var result = await _dbContext.UserPerformanceReviews.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.InterviewDateFromUtc != null && x.InterviewDateFromUtc.Value.Date == date,
            cancellationToken);

        if (result != null && performanceReviewId == result.Id)
        {
            return true;
        }

        if (result != null && performanceReviewId != result.Id)
        {
            return false;
        }

        return result == null;
    }
}
