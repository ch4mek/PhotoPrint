using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpInternshipDetail
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public required string Specialization { get; set; }
    public int? ApprovedPoints { get; set; }
    public int? CompletedPoints { get; set; }
    public int EngineerStage { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly PlannedEndDate { get; set; }

    public InternshipType Type { get; set; }
    public int TypeOrderWeight { get; set; }

    public InternshipFinalResult? FinalResult { get; set; }
    public int FinalResultOrderWeight { get; set; }
}
