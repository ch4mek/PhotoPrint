namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ClientProjectOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required bool IsActive { get; set; }
    public required int? FactTrackedTime { get; set; }
    public required int? BillableTrackedTime { get; set; }
}
