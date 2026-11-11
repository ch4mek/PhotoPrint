using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

internal record ProjectExperienceCoreDto
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public string? TeamSize { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? Description { get; set; }
    public string? Responsibilities { get; set; }
    public string? Environment { get; set; }
    public int OrderWeight { get; set; }
    public int CvId { get; set; }

    public static ProjectExperienceCoreDto Of(ProjectExperienceEntity entity)
    {
        return new ProjectExperienceCoreDto
        {
            Id = entity.Id,
            Position = entity.Position,
            TeamSize = entity.TeamSize,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsCurrent = entity.IsCurrent,
            Description = entity.Description,
            Responsibilities = entity.Responsibilities,
            Environment = entity.Environment,
            OrderWeight = entity.OrderWeight,
            CvId = entity.CvId,
        };
    }
}
