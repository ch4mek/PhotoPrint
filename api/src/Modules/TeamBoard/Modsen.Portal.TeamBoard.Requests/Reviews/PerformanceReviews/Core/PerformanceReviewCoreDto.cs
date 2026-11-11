namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core;

public record PerformanceReviewCoreDto(
    int Id,
    DateTime? PerformanceReviewDate,
    string? Status);
