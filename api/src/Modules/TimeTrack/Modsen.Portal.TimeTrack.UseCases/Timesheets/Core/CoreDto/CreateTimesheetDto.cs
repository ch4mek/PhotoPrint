using System.Text.Json.Serialization;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal class CreateTimesheetDto
{
    public required string TaskName { get; set; }

    [JsonPropertyName("ftm")]
    public int FactTrackedInMinutes { get; set; }

    [JsonPropertyName("btm")]
    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public bool AllowAutomatic { get; set; }

    public Timesheet ToDatabaseEntity(int userId, int? createdById, DateTime createdAt)
    {
        return new Timesheet
        {
            TaskName = TaskName,
            FactTrackedInMinutes = FactTrackedInMinutes,
            BillableTrackedInMinutes = BillableTrackedInMinutes ?? 0,
            ReportForDate = ReportForDate,
            ProjectId = ProjectId,
            UserId = userId,
            ReportYear = ReportForDate.Year,
            ReportMonthOfYear = ReportForDate.Month,
            ReportDayOfYear = ReportForDate.DayOfYear,
            ReportWeekOfYear = ReportForDate.GetWeekOfYear(),
            CreatedAtUtc = createdAt,
            CreatedById = createdById
        };
    }
}
