using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal record UpdateUserSubmitStatusDto(
    int UserId,
    TimesheetSubmitStatusType Status,
    int Month,
    int Year);
