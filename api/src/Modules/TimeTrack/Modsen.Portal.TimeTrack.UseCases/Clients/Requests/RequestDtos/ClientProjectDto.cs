using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

internal class ClientProjectDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required bool IsActive { get; set; }
    public int? FactTrackedTime { get; set; }
    public int? BillableTrackedTime { get; set; }

    public static ClientProjectDto Of(Project project)
    {
        return new ClientProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Color = project.Color,
            IsActive = project.IsActive,
            FactTrackedTime = project.FactTrackedTimeInMinutes,
            BillableTrackedTime = project.BillableTrackedTimeInMinutes
        };
    }
}
