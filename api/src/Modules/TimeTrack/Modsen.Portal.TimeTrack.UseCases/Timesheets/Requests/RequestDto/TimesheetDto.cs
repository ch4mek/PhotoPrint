using System.Text.Json.Serialization;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal class TimesheetDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string? TaskName { get; set; }
    public DateOnly ReportForDate { get; set; }

    [JsonPropertyName("ftm")]
    public int FactTrackedInMinutes { get; set; }

    [JsonPropertyName("btm")]
    public int BillableTrackedInMinutes { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Color { get; set; }

    public static TimesheetDto Of(Timesheet source)
    {
        return new TimesheetDto
        {
            Id = source.Id,
            UserId = source.UserId,
            UserName = source.User!.FirstName + " " + source.User.LastName,
            ProjectId = source.ProjectId,
            ProjectName = source.Project!.Name,
            TaskName = source.TaskName,
            Color = source.User.Color,
            FactTrackedInMinutes = source.FactTrackedInMinutes,
            BillableTrackedInMinutes = source.BillableTrackedInMinutes,
            ReportForDate = source.ReportForDate
        };
    }
}
