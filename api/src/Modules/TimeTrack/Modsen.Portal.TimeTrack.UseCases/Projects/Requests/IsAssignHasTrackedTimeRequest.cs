using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class IsAssignHasTrackedTimeRequest(
    ITimeTrackDbContext timeTrackDbContext) : IQueryHandler<IsAssignHasTrackedTimeRequest.Query, bool>
{
    public Task<bool> Handle(Query request, CancellationToken cancellationToken)
    {
        return timeTrackDbContext.Timesheets
            .Where(timesheet => timesheet.UserId == request.UserId)
            .Where(timesheet => timesheet.ProjectId == request.ProjectId)
            .Where(timesheet => timesheet.ReportForDate >= request.UserProjectFrom)
            .Where(timesheet => request.UserProjectTo == null || timesheet.ReportForDate <= request.UserProjectTo)
            .AnyAsync(cancellationToken);
    }

    internal sealed record Query(int UserId, int ProjectId, DateOnly UserProjectFrom, DateOnly? UserProjectTo) : IQuery<bool>;
}
