namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;

internal class FilterParams
{
    public int? UserId { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public int? ClientId { get; set; }
    public int? ProjectId { get; set; }
    public int? ManagerId { get; set; }
    public bool? IsActive { get; set; }
    public bool? OnlyManagedUsers { get; set; }
    public DateOnly? ReportForDateFrom { get; set; }
    public DateOnly? ReportForDateTo { get; set; }
}
