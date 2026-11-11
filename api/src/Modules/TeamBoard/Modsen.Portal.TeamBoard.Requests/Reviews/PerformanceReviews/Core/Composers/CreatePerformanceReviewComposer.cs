using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Composers;

internal interface ICreatePerformanceReviewComposer
{
    Task<PerformanceReviewCoreDto> Compose(CreatePerformanceReviewCoreDto request,
        CancellationToken cancellationToken);
}

internal class CreatePerformanceReviewComposer : ICreatePerformanceReviewComposer
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;

    public CreatePerformanceReviewComposer(ITeamBoardDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext, IClockService clockService)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
    }

    public async Task<PerformanceReviewCoreDto> Compose(CreatePerformanceReviewCoreDto request,
        CancellationToken cancellationToken)
    {
        var userPerformanceReview = new UserPerformanceReview
        {
            InterviewDateFromUtc = request.Date,
            InterviewDateToUtc = request.Date?.AddHours(1),
            UserId = request.UserId,
            Type = UserPerformanceReviewType.ScheduledPerformanceReview,
            Status = (request.Passed ?? false) ? UserPerformanceReviewStatus.Passed : UserPerformanceReviewStatus.Finished,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId,
            Agreements = request.Agreements,
            EmployeeFeedback = request.EmployeeTheses,
            ResponsibleUserFeedback = request.Theses,
            ConversationId = request.ConversationId
        };

        var response = await _dbContext.UserPerformanceReviews.AddAsync(userPerformanceReview, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (request.ConversationId != null)
        {
            var performanceReviewAssignedToConversation = await _dbContext.UserPerformanceReviews.FirstOrDefaultAsync(x => x.ConversationId == request.ConversationId, cancellationToken);

            if (performanceReviewAssignedToConversation != null)
            {
                await _dbContext.Conversations.FirstOrDefaultAsync(x => x.Id == request.ConversationId, cancellationToken);
                performanceReviewAssignedToConversation.ConversationId = request.ConversationId;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var result = new PerformanceReviewCoreDto(
            response.Entity.Id,
            response.Entity.InterviewDateFromUtc,
            response.Entity.Status.GetDescription());

        return result;
    }
}
