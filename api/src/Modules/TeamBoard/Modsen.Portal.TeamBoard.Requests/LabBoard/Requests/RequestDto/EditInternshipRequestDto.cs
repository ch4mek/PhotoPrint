using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;

internal record EditInternshipRequestDto(
    int? Id,
    string Specialization,
    InternshipType Type,
    int EngineerStage,
    int? CompletedPoints,
    int? ApprovedPoints,
    InternshipFinalResult? FinalResult,
    DateOnly StartDate,
    DateOnly PlannedEndDate    
);
