namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

public record GetTimesheetsByProjectsIdsDto(
    int ProjectId,
    int FactTrackedSum,
    int BillableTrackedSum);
