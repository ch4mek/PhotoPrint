using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.BusinessRules;

internal interface IIsPerformanceReviewForTodayExistingBusinessRule
    : IBusinessRuleWithFailReason<IsPerformanceReviewForTodayExistingBusinessRuleRequestModel>;

internal record IsPerformanceReviewForTodayExistingBusinessRuleRequestModel(
    int UserId, DateTime? PerformanceReviewDate);

internal class IsPerformanceReviewForTodayExistingBusinessRule : IIsPerformanceReviewForTodayExistingBusinessRule
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public IsPerformanceReviewForTodayExistingBusinessRule(
        ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(
        IsPerformanceReviewForTodayExistingBusinessRuleRequestModel? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);
        var isPerformanceReviewForTodayExisting = await _teamBoardDbContext.UserPerformanceReviews
            .AnyAsync(x => x.UserId == model.UserId &&
            x.InterviewDateFromUtc!.Value.Date == model.PerformanceReviewDate!.Value.Date,
            cancellationToken);

        if (isPerformanceReviewForTodayExisting)
        {
            return BusinessRuleResult.Fail("Performance review for the same date already exists");
        }

        return BusinessRuleResult.Success();
    }
}
