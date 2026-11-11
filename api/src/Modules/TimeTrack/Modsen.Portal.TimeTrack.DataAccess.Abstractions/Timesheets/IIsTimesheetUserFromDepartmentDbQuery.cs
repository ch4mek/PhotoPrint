namespace Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

internal interface IIsTimesheetUserFromDepartmentDbQuery
{
    Task<bool> IsTimesheetUserFromDepartmentAsync(int timesheetId, string departmentId, CancellationToken cancellationToken);
}
