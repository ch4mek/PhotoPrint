using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Composers
{
    internal interface IUpdatePerformanceReviewComposer
    {
        Task<PerformanceReviewCoreDto?> Compose(
            UpdatePerformanceReviewCoreDto request,
            CancellationToken cancellationToken);
    }

    internal class UpdatePerformanceReviewComposer : IUpdatePerformanceReviewComposer
    {
        private readonly ITeamBoardDbContext _dbContext;

        public UpdatePerformanceReviewComposer(ITeamBoardDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PerformanceReviewCoreDto?> Compose(UpdatePerformanceReviewCoreDto request, CancellationToken cancellationToken)
        {
            var performanceReviewToUpdate = await _dbContext.UserPerformanceReviews.FirstOrDefaultAsync(x => x.Id == request.UserPerformanceReviewId,
                cancellationToken);

            if (performanceReviewToUpdate == null)
            {
                return null;
            }

            performanceReviewToUpdate.InterviewDateFromUtc = request.Date;
            performanceReviewToUpdate.InterviewDateToUtc = request.Date?.AddHours(1);
            performanceReviewToUpdate.Status = (request.Passed ?? false) ? UserPerformanceReviewStatus.Passed : UserPerformanceReviewStatus.Pending;
            performanceReviewToUpdate.ResponsibleUserFeedback = request.Theses;
            performanceReviewToUpdate.Agreements = request.Agreements;
            performanceReviewToUpdate.ConversationId = request.ConversationId;
            performanceReviewToUpdate.EmployeeFeedback = request.EmployeeTheses;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new PerformanceReviewCoreDto(
                performanceReviewToUpdate.Id,
                performanceReviewToUpdate.InterviewDateFromUtc,
                performanceReviewToUpdate.Status.GetDescription());
        }
    }
}
