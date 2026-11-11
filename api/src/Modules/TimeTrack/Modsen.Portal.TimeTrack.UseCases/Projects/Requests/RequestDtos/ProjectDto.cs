using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class ProjectDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required int ClientId { get; set; }
    public required int? ManagerId { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsPublic { get; set; }

    public static ProjectDto Of(Project source)
    {
        return new ProjectDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ManagerId = source.ManagerId,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive
        };
    }

    public static ProjectDto Of(DataAccess.Npgsql.DatabaseEntities.Project source)
    {
        return new ProjectDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ManagerId = source.ManagerUserId,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive
        };
    }
}
