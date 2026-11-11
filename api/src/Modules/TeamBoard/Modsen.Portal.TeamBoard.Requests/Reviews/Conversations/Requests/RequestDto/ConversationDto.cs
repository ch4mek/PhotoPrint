using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

internal record ConversationDto(
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
    bool? Passed,
    int? UserPerformanceReviewId,
    IReadOnlyCollection<ConversationFileDto> Files,
    IReadOnlyCollection<ConversationDocumentLinkDto> DocumentLinks,
    IReadOnlyCollection<UserSalaryFullCoreDto> Salaries,
    IReadOnlyCollection<DevelopmentPlanFullCoreDto> DevelopmentPlans)
{
    public static ConversationDto? Of(ConversationCoreDto? source, bool? isPassed)
    {
        if (source == null)
        {
            return null;
        }

        return new ConversationDto(
            source.Id,
            source.Date,
            source.ConversationType,
            source.ConversationResult,
            source.Theses,
            source.EmployeeTheses,
            source.Agreements,
            source.PerformanceReviewDate,
            source.PerformanceReviewStatus,
            source.SupervisorId,
            source.UserId,
            isPassed,
            source.UserPerformanceReviewId,
            source.Files.Select(x => new ConversationFileDto(x.Id, x.FileId, x.FileName)).ToList(),
            source.DocumentLinks.Select(x => new ConversationDocumentLinkDto(x.Id, x.Link)).ToList(),
            [],
            []);
    }
};
