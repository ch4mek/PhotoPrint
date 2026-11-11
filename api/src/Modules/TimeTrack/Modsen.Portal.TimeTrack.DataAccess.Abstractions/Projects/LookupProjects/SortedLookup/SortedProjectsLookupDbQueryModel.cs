namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.SortedLookup;

public record SortedProjectsLookupDbQueryModel(
    int Id,
    string Name,
    string? Color,
    bool IsPublic,
    int? CreatedById,
    string Type,
    string Client,
    IReadOnlyCollection<ProjectUserInfoModel> ProjectUsers);
