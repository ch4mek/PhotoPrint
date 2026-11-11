using System.Text.Json.Serialization;
using Modsen.Portal.Core.Api.Converters;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal class CreateConversationInput
{

    public DateOnly Date { get; set; }
    public ConversationType ConversationType { get; set; }
    public ConversationResult? ConversationResult { get; set; }
    public string? Theses { get; set; }
    public string? EmployeeTheses { get; set; }

    [JsonConverter(typeof(SideTrimmingConverter))]
    public string? Agreements { get; set; }
    public DateTime? PerformanceReviewDate { get; set; }
    public bool? Passed { get; set; }
    public int? UserPerformanceReviewId { get; set; }
    public int UserId { get; set; }
    public IReadOnlyCollection<CreateConversationFileDto> Files { get; set; } = [];
    public IReadOnlyCollection<string> DocumentLinks { get; set; } = [];
    public IReadOnlyCollection<UserSalaryFullCoreDto> Salaries { get; set; } = [];
    public IReadOnlyCollection<DevelopmentPlanFullInput> DevelopmentPlans { get; set; } = [];

    public static CreateConversationDto Of(CreateConversationInput input)
    {
        return new CreateConversationDto
        (
            Date: input.Date,
            ConversationType: input.ConversationType,
            ConversationResult: input.ConversationResult,
            Theses: input.Theses,
            EmployeeTheses: input.EmployeeTheses,
            Agreements: input.Agreements,
            PerformanceReviewDate: input.PerformanceReviewDate,
            UserPerformanceReviewId: input.UserPerformanceReviewId,
            Passed: input.Passed,
            UserId: input.UserId,
            Files: input.Files,
            DocumentLinks: input.DocumentLinks,
            Salaries: input.Salaries,
            DevelopmentPlans: input.DevelopmentPlans.Select(DevelopmentPlanFullInput.Of).ToList()
        );
    }
};
