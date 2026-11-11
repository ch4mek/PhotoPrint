
namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.CoreDto;

internal record UpdatePerformanceReviewCoreDto(
    int UserPerformanceReviewId,
    int? ConversationId,
    DateTime? Date,
    string? Theses,
    string? EmployeeTheses,
    string? Agreements,
    bool? Passed,
    int UserId);
