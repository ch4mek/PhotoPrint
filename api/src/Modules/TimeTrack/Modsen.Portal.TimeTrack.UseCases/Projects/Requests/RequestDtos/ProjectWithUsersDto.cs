using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class ProjectWithUsersDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required int ClientId { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsPublic { get; set; }
    public int? ManagerId { get; set; }
    public string? ManagerFullName { get; set; }
    public IReadOnlyCollection<ProjectUserInfoDto> ProjectUsers { get; set; } = [];

    public static ProjectWithUsersDto Of(Project source)
    {
        return new ProjectWithUsersDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            IsPublic = source.IsPublic,
            ManagerId = source.ManagerId,
            ProjectUsers = source.ProjectUsers.ConvertAll(x => new ProjectUserInfoDto { Id = x })
        };
    }
}
