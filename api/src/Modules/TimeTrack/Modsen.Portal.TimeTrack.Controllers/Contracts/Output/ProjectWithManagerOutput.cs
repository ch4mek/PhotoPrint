using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectWithManagerOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Color { get; set; }
    public required int ClientId { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsPublic { get; set; }
    public bool? IsSelfCreatedProject { get; set; }
    public int? ManagerId { get; set; }
    public string? ManagerFullName { get; set; }

    internal static ProjectWithManagerOutput Of(ProjectWithManagerDto source)
    {
        return new ProjectWithManagerOutput
        {
            Id = source.Id,
            Name = source.Name,
            ClientId = source.ClientId,
            IsSelfCreatedProject = source.IsSelfCreatedProject,
            Color = source.Color,
            IsActive = source.IsActive,
            IsBillable = source.IsBillable,
            IsPublic = source.IsPublic,
            ManagerId = source.ManagerId,
            ManagerFullName = source.ManagerFullName
        };
    }
}
