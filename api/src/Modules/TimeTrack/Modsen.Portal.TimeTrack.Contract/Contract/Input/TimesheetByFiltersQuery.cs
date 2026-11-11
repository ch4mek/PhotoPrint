namespace Modsen.Portal.TimeTrack.Contract.Contract.Input;

public record TimesheetByFiltersQuery(int? StartYear, int? EndYear, string? DepartmentId,
    IReadOnlyCollection<int>? Projects, IReadOnlyCollection<int>? Clients, bool? IsBillable);
