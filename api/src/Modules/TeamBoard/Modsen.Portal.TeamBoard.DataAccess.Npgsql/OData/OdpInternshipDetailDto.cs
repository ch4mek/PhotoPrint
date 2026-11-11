using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpInternshipDetailDto
{
    public int Id { get; init; }

    public int UserId { get; init; }

    public required string Specialization { get; init; }
    public int? ApprovedPoints { get; init; }
    public int? CompletedPoints { get; init; }
    public int EngineerStage { get; init; }

    public DateOnly StartDate { get; init; }
    public DateOnly PlannedEndDate { get; init; }

    public InternshipType Type { get; init; }
    public int TypeOrderWeight { get; init; }

    public InternshipFinalResult? FinalResult { get; init; }
    public int FinalResultOrderWeight { get; init; }
}
