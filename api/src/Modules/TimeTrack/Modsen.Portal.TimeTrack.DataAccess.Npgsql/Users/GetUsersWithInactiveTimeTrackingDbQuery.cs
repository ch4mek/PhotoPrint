using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

internal class GetUsersWithInactiveTimeTrackingDbQuery : IGetUsersWithInactiveTimeTrackingDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;
    private readonly IIdentityContract _identityContract;
    public GetUsersWithInactiveTimeTrackingDbQuery(ITimeTrackDbContext dbContext,
        IIdentityContract identityContract)
    {
        _dbContext = dbContext;
        _identityContract = identityContract;
    }


    public async Task<List<GetUsersWithInactiveTimeTrackingDbResponse>> ExecuteAsync(
        GetUsersWithInactiveTimeTrackingDbRequest request, CancellationToken cancellationToken)
    {
        var managerIds = await _identityContract.GetManagerIds(cancellationToken);
        return await _dbContext.TimeTrackUsers
            .Where(user => !user.Timesheets.Any(timesheet =>
                request.StartDate <= timesheet.ReportForDate && timesheet.ReportForDate < request.EndDate)
                && !managerIds.Contains(user.UserId))
            .Select(user => new GetUsersWithInactiveTimeTrackingDbResponse(user.Email, user.FirstName))
            .ToListAsync(cancellationToken);
    }
}
