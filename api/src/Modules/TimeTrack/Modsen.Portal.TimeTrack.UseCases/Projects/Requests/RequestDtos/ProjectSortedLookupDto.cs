using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.SortedLookup;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

internal class ProjectSortedLookupDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required bool IsPublic { get; set; }
    public required int? CreatedById { get; set; }
    public required string Type { get; set; }
    public required string ClientName { get; set; }

    public IReadOnlyCollection<ProjectUserInfoDto> ProjectUsers { get; set; } = [];

    public static ProjectSortedLookupDto Of(SortedProjectsLookupDbQueryModel source)
    {
        return new ProjectSortedLookupDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            IsPublic = source.IsPublic,
            CreatedById = source.CreatedById,
            Type = source.Type,
            ClientName = source.Client,
            ProjectUsers = source.ProjectUsers.Select(projectUser => new ProjectUserInfoDto
            {
                Id = projectUser.Id,
                LastName = projectUser.LastName,
                FirstName = projectUser.FirstName,
                DepartmentId = projectUser.DepartmentId
            }).ToList()
        };
    }
    public static ProjectSortedLookupDto Of(Project source)
    {
        return new ProjectSortedLookupDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            IsPublic = source.IsPublic,
            CreatedById = source.CreatedById,
            Type = source.GetProjectType(),
            ClientName = string.Empty,
            ProjectUsers = [],
        };
    }
}
