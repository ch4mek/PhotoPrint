namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public class CreateOrUpdateTrackedTimeDto
{
    public required string TaskName { get; set; }
    public DateOnly ReportForDate { get; set; }
    public int ProjectId { get; set; }
    public int? UserId { get; set; }
    public bool AllowAutomatic { get; set; }
}

public class UpsertBillableDto : CreateOrUpdateTrackedTimeDto
{
    public int BillableTrackedInMinutes { get; set; }
}

public class UpsertFactDto : CreateOrUpdateTrackedTimeDto
{
    public int FactTrackedInMinutes { get; set; }
}
