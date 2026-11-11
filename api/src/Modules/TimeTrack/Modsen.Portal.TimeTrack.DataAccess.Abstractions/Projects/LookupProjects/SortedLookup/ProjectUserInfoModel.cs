namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.SortedLookup;

public record ProjectUserInfoModel(
    int Id,
    string FirstName,
    string LastName,
    string? DepartmentId);
