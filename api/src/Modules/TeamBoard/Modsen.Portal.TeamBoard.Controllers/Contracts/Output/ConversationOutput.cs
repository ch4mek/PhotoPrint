using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record ConversationOutput(
    int Id,
    DateOnly? Date,
    string ConversationType,
    string? ConversationResult,
    string? Theses,
    string? EmployeeTheses,
    string? Agreements,
    int SupervisorId,
    int UserId,
    DateTime? PerformanceReviewDate,
    bool? Passed,
    int? UserPerformanceReviewId,
    string? Status,
    IReadOnlyCollection<ConversationFileDto>? Files,
    IReadOnlyCollection<ConversationDocumentLinkDto>? DocumentLinks,
    IReadOnlyCollection<UserSalaryFullCoreDto> Salaries,
    IReadOnlyCollection<DevelopmentPlanFullCoreDto> DevelopmentPlans)
{
    public static ConversationOutput Of(ConversationDto source)
    {
        return new ConversationOutput(
            source.Id,
            source.Date,
            source.ConversationType,
            source.ConversationResult,
            source.Theses,
            source.EmployeeTheses,
            source.Agreements,
            source.SupervisorId,
            source.UserId,
            source.PerformanceReviewDate,
            source.Passed,
            source.UserPerformanceReviewId,
            source.PerformanceReviewStatus,
            source.Files,
            source.DocumentLinks,
            source.Salaries,
            source.DevelopmentPlans);
    }
};
