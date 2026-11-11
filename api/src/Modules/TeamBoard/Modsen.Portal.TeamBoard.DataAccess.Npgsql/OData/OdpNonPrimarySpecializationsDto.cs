namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpNonPrimarySpecializationDto
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Name { get; set; }
    public int? OrderWeight { get; set; }

    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevel { get; set; }

    public bool? IsVisibleSpecialization { get; set; }
    public bool? IsActive { get; set; }

    public DateTime? CreatedAtUtc { get; set; }
}
