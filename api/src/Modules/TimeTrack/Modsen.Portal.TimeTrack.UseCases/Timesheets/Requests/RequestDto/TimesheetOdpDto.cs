using System.Linq.Expressions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class TimesheetOdpDto
{
    public int Id { get; set; }
    public required string TaskName { get; set; }
    public int FactTrackedInMinutes { get; set; }
    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }
    public int ReportYear { get; set; }
    public int ReportMonthOfYear { get; set; }
    public int ReportWeekOfYear { get; set; }
    public int ReportDayOfYear { get; set; }

    public int UserId { get; set; }
    public string? UserDepartmentId { get; set; }
    public required string UserFullName { get; set; }
    public string? UserColor { get; set; }

    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? ProjectColor { get; set; }
    public bool IsProjectActive { get; set; }

    public int ClientId { get; set; }

    internal static readonly Expression<Func<Timesheet, TimesheetOdpDto>> Map = source => new TimesheetOdpDto
    {
        Id = source.Id,
        TaskName = source.TaskName,
        FactTrackedInMinutes = source.FactTrackedInMinutes,
        BillableTrackedInMinutes = source.BillableTrackedInMinutes,
        ReportForDate = source.ReportForDate,
        ReportYear = source.ReportYear,
        ReportMonthOfYear = source.ReportMonthOfYear,
        ReportWeekOfYear = source.ReportWeekOfYear,
        ReportDayOfYear = source.ReportDayOfYear,
        UserId = source.UserId,
        UserDepartmentId = source.User!.DepartmentId,
        UserFullName = $"{source.User!.FirstName} {source.User.LastName}",
        UserColor = source.User!.Color,
        ProjectId = source.ProjectId,
        ProjectName = source.Project!.Name,
        ProjectColor = source.Project!.Color,
        IsProjectActive = source.Project!.IsActive,
        ClientId = source.Project!.ClientId
    };
}
