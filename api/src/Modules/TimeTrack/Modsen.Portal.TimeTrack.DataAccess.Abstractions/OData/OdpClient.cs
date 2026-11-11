namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

public class OdpClient
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntity { get; set; }
    public string? Color { get; set; }
    public string? Comments { get; set; }
    public bool IsActive { get; set; }
    public int FactTrackedTimeInMinutes { get; set; }
    public int BillableTrackedTimeInMinutes { get; set; }
    public bool IsProjectExist { get; set; }
    public bool HasActiveProject { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
    public string? SalesManagerFullName { get; set; }
}
