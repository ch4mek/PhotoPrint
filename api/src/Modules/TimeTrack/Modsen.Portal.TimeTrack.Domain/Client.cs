namespace Modsen.Portal.TimeTrack.Domain;

internal class Client
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntity { get; set; }
    public string? Color { get; set; }
    public string? Comments { get; set; }
    public required bool IsActive { get; set; }
    public int FactTrackedTimeInMinutes { get; set; }
    public int BillableTrackedTimeInMinutes { get; set; }
    public bool IsProjectExist { get; set; }
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId  { get; set; }

    public required IReadOnlyCollection<int> Projects { get; set; } = [];
}
