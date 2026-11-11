namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpTeamBoardUserDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public DateOnly? CareerStartedAt { get; set; }
    public int ProfExperienceInMonths { get; set; }

    public string? EnglishLevelId { get; set; }
    public string? EnglishLevelName { get; set; }
    public int? EnglishLevelOrderWeight { get; set; }

    public string? SeniorityLevelId { get; set; }
    public string? SeniorityLevel { get; set; }
    public int? SeniorityLevelOrderWeight { get; set; }

    public string? GeneralSeniorityLevel { get; set; }

    public int? PrimarySpecializationId { get; set; }
    public string? PrimarySpecializationName { get; set; }

    public string? DepartmentId { get; set; }
    public string? DepartmentFullName { get; set; }
    public int? DepartmentOrderWeight { get; set; }

    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }

    public string? Position { get; set; }
    public int? PositionOrderWeight { get; set; }
    public string? Location { get; set; }
    public string? RegisteredLocation { get; set; }
    public string? CurrentLocation { get; init; }
    public string? Comments { get; set; }

    public bool IsActive { get; set; }

    public IReadOnlyCollection<OdpNonPrimarySpecializationDto> OdpNonPrimarySpecializations { get; set; } = [];
}
