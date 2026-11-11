using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal sealed record UserTimesheetsWithBlockedDaysDto(
    PageableListOfItems<UserTimesheetDto> PageableListOfItems,
    List<DateOnly> BlockedDays);
