namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.Lookup;

public record ProjectsLookupDbQueryModel(
    int Id,
    string Name,
    string ClientName,
    bool IsBillable,
    string? Color);
