using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;

internal class InternshipDto
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public required string Specialization { get; set; }
    public required InternshipType Type { get; set; }
    public required int EngineerStage { get; set; }
    public int? CompletedPoints { get; set; }
    public int? ApprovedPoints { get; set; }
    public InternshipFinalResult? FinalResult { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly PlannedEndDate { get; set; }
    public List<InternshipSpecializationDto> UserInternshipSpecializations { get; set; } = [];


    internal static InternshipDto Of(InternshipDetail source)
    {
        return new InternshipDto
        {
            Id = source.Id,
            UserId = source.UserId,
            Specialization = source.Specialization,
            Type = source.Type,
            EngineerStage = source.EngineerStage,
            CompletedPoints = source.CompletedPoints,
            ApprovedPoints = source.ApprovedPoints,
            FinalResult = source.FinalResult,
            StartDate = source.StartDate,
            PlannedEndDate = source.PlannedEndDate,
            UserInternshipSpecializations = source.InternshipSpecializations
                .Select(s => InternshipSpecializationDto.Of(s))
                .ToList()
        };
    }
}
