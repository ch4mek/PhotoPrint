using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public int ClientId { get; set; }
    public int? ManagerId { get; set; }
    public bool IsBillable { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }

    internal static ProjectOutput? Of(ProjectDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            IsPublic = source.IsPublic,
            IsActive = source.IsActive,
            IsBillable = source.IsBillable,
            ManagerId = source.ManagerId
        };
    }
}
