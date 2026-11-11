using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests
{
    internal sealed class GetStatusWithPreviousTasks : IQueryHandler<GetStatusWithPreviousTasks.Query, List<UserTimesheetCalendarDto>>
    {
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly ITimeTrackDbContext _timeTrackDbContext;
        public GetStatusWithPreviousTasks(
            IAuthenticatedUserContext authenticatedUserContext,
            ITimeTrackDbContext timeTrackDbContext)
        {
            _authenticatedUserContext = authenticatedUserContext;
            _timeTrackDbContext = timeTrackDbContext;
        }

        public async Task<List<UserTimesheetCalendarDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (_authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto())
            {
                return [];
            }
            var userId = _authenticatedUserContext.GetRequiredUserId();
            var month = request.ForDate.Month;
            var year = request.ForDate.Year;

            var result = await _timeTrackDbContext.Timesheets
                .Where(x => x.UserId == userId)
                .Where(x => x.ReportForDate.Year == year && x.ReportForDate.Month == month)
                .GroupBy(x => x.ReportForDate)
                .Select(g => new UserTimesheetCalendarDto
                {
                    Date = g.Key,
                    FactTrackedSum = g.Sum(x => x.FactTrackedInMinutes),
                    Tasks = g
                        .GroupBy(x => new { x.TaskName, x.Project!.Name, x.ProjectId })
                        .Select(t => new ProjectTaskDto
                        {
                            Id = t.Key.ProjectId,
                            TaskName = t.Key.TaskName,
                            ProjectName = t.Key.Name,
                            FactTrackedTime = t.Sum(y => y.FactTrackedInMinutes)
                        })
                        .ToList()
                })
                .Where(x => x.FactTrackedSum > 0)
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            return result;
        }

        internal sealed record Query(DateOnly ForDate) : IQuery<List<UserTimesheetCalendarDto>>;
    }
}
