using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal record EditInternshipInput
{
    public int? Id { get; set; }
    public required string Specialization { get; init; }
    public required InternshipType Type { get; init; }
    public required int EngineerStage { get; init; }
    public int? CompletedPoints { get; init; }
    public int? ApprovedPoints { get; init; }
    public InternshipFinalResult? FinalResult { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly PlannedEndDate { get; init; }
    public List<InternshipSpecializationInput> InternshipSpecializations { get; init; } = [];
}
