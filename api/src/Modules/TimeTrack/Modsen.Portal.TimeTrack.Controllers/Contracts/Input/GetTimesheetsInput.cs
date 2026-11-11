namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

public record GetTimesheetsInput(
    int? Top,
    int? Skip,
    DateOnly ReportForDateFrom,
    DateOnly ReportForDateTo,
    string? DepartmentId,
    int? ResourceGroupId,
    int? ClientId,
    int? ProjectId,
    int? UserId,
    bool? OnlyManagedProjects,
    bool? AssignedForSelectedDate);
