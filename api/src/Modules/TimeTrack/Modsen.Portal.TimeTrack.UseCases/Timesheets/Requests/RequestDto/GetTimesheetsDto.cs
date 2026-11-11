namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal record GetTimesheetsDto
{
    public required int Top { get; set; }

    public required int Skip { get; set; }

    public required DateOnly ReportForDateFrom { get; set; }
    public required DateOnly ReportForDateTo { get; set; }

    public string? DepartmentId { get; set; }

    public int? ResourceGroupId { get; set; }

    public int? ClientId { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public int? ManagerId { get; set; }

    public bool? OnlyManagedProjects { get; set; }
    public bool? AssignedForSelectedDate { get; set; }

    internal static GetTimesheetsDto Of(GetTimesheetsRequest.Query source)
    {
        return new GetTimesheetsDto
        {
            Top = source.SearchFilters.Top,
            Skip = source.SearchFilters.Skip,
            ReportForDateFrom = source.SearchFilters.ReportForDateFrom,
            ReportForDateTo = source.SearchFilters.ReportForDateTo,
            DepartmentId = source.SearchFilters.DepartmentId,
            ClientId = source.SearchFilters.ClientId,
            ProjectId = source.SearchFilters.ProjectId,
            UserId = source.SearchFilters.UserId,
            OnlyManagedProjects = source.SearchFilters.OnlyManagedProjects,
            ResourceGroupId = null,
            ManagerId = null
        };
    }
}
