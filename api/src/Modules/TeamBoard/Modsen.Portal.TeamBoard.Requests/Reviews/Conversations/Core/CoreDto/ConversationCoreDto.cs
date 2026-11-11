
namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.CoreDto
{

    internal record ConversationCoreDto(
        int Id,
        DateOnly? Date,
        string ConversationType,
        string? ConversationResult,
        string? Theses,
        string? EmployeeTheses,
        string? Agreements,
        DateTime? PerformanceReviewDate,
        string? PerformanceReviewStatus,
        int SupervisorId,
        int UserId,
        int? UserPerformanceReviewId,
        IReadOnlyCollection<ConversationFileCoreDto> Files,
        IReadOnlyCollection<ConversationDocumentLinkCoreDto> DocumentLinks);
}
