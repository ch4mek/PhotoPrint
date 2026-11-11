namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class OdpUserPerformanceReviewDto
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? DepartmentId { get; set; }
    public IReadOnlyCollection<PerformanceReviewDateDto>? Dates { get; set; } = [];
}
