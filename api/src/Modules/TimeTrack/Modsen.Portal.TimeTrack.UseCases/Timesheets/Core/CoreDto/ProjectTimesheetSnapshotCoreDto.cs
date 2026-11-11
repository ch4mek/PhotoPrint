namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal sealed record ProjectTimesheetSnapshotCoreDto(
    int ProjectId,
    int FactTrackedInMinutes,
    int BillableTrackedInMinutes);
