
namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.CoreDto;

internal record CreatePerformanceReviewCoreDto(
    int UserId,
    DateTime? Date,
    bool? Passed,
    string? Theses,
    string? EmployeeTheses,
    string? Agreements,
    int? ConversationId);
