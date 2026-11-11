using System.Text.Json.Serialization;
using Modsen.Portal.Core.Api.Converters;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record UpdateConversationInput
{
    public int Id { get; set; }
    public ConversationType ConversationType { get; set; }
    public ConversationResult? ConversationResult { get; set; }
    public string? Theses { get; set; }
    public string? EmployeeTheses { get; set; }
    [JsonConverter(typeof(SideTrimmingConverter))]
    public string? Agreements { get; set; }
    public DateTime? PerformanceReviewDate { get; set; }
    public bool? Passed { get; set; }
    public int UserId { get; set; }
    public IReadOnlyCollection<CreateConversationFileDto> Files { get; set; } = [];
    public IReadOnlyCollection<string> DocumentLinks { get; set; } = [];
    public IReadOnlyCollection<UserSalaryFullCoreDto> Salaries { get; set; } = [];
    public IReadOnlyCollection<DevelopmentPlanFullInput> DevelopmentPlans { get; set; } = [];

    public static UpdateConversationDto Of(UpdateConversationInput input)
    {
        return new UpdateConversationDto(
                input.Id,
                input.ConversationType,
                input.ConversationResult,
                input.Theses,
                input.EmployeeTheses,
                input.Agreements,
                input.PerformanceReviewDate,
                input.Passed,
                input.UserId,
                input.Files,
                input.DocumentLinks,
                input.Salaries,
                input.DevelopmentPlans.Select(DevelopmentPlanFullInput.Of).ToList()
        );
    }
}
