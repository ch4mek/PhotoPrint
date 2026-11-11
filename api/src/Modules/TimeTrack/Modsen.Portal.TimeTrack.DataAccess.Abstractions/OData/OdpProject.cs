namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

public class OdpProject
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public int ClientId { get; set; }
    public required string ClientName { get; set; }
    public int FactTrackedTime { get; set; }
    public int BillableTrackedTime { get; set; }
    public bool IsPublic { get; set; }
    public bool IsBillable { get; set; }
    public bool IsActive { get; set; }
    public IReadOnlyCollection<OdpProjectUser> ProjectUsers { get; set; } = [];
}
