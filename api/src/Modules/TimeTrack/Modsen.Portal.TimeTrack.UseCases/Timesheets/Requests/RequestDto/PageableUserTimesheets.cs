using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

internal record PageableUserTimesheets(PageableListOfItems<UserTimesheetSnapshotDto> UsersTimesheets);
