using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class UserTimesheetDto
{
    public int Id { get; set; }

    public required string ProjectName { get; set; }

    public required string TaskName { get; set; }


    [JsonPropertyName("ftm")]
    public int FactTrackedInMinutes { get; set; }


    [JsonPropertyName("btm")]
    public int? BillableTrackedInMinutes { get; set; }

    public DateOnly ReportForDate { get; set; }

    public int ProjectId { get; set; }

    public bool IsProjectActive { get; set; }

    public static readonly Expression<Func<TimesheetOdpDto, UserTimesheetDto>> Map = source => new UserTimesheetDto
    {
        Id = source.Id,
        ProjectName = source.ProjectName!,
        TaskName = source.TaskName,
        FactTrackedInMinutes = source.FactTrackedInMinutes,
        BillableTrackedInMinutes = source.BillableTrackedInMinutes,
        ReportForDate = source.ReportForDate,
        ProjectId = source.ProjectId,
        IsProjectActive = source.IsProjectActive
    };
}
