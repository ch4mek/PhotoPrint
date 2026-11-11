using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

internal sealed class IsTimesheetUserFromDepartmentDbQuery
    (ITimeTrackDbContext dbContext) : IIsTimesheetUserFromDepartmentDbQuery
{
    public Task<bool> IsTimesheetUserFromDepartmentAsync(int timesheetId, string departmentId,
        CancellationToken cancellationToken)
    {
        return dbContext.Timesheets
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.Id == timesheetId)
            .AnyAsync(x => x.User!.DepartmentId == departmentId, cancellationToken);
    }
}
