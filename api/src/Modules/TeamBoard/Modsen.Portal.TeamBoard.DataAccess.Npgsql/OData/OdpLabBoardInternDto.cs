namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpLabBoardInternDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public OdpInternshipDetailDto? Internship { get; set; }

    public string? EnglishLevelId { get; set; }
    public string? EnglishLevelName { get; set; }
    public int? EnglishLevelOrderWeight { get; set; }

    public int? ResourceGroupId { get; set; }
    public string? ResourceGroupName { get; set; }

    public string? CurrentLocation { get; set; }
    public string? Comments { get; set; }

    public bool IsActive { get; set; }
}
