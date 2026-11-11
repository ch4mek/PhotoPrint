using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

internal record SubmitStatusCoreDto(
    int UserId,
    TimesheetSubmitStatusType Status,
    int Year,
    int Month,
    int FactTrackedInMinutesSum,
    int BillableTrackedInMinutesSum,
    string Snapshot);
