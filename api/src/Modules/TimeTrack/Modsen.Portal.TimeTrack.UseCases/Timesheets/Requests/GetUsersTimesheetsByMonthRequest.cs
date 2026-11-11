using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetUsersTimesheetsByMonthRequest : IQueryHandler<GetUsersTimesheetsByMonthRequest.Query, List<UserTimesheetByMonthDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetUsersTimesheetsByMonthRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<UserTimesheetByMonthDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var userIds = request.UsersTimesheetsDto.Select(y => y.UserId).Distinct().ToHashSet();
        var projectIds = request.UsersTimesheetsDto.Select(y => y.ProjectId).Distinct().ToHashSet();
        var years = request.UsersTimesheetsDto.Select(y => y.Year).Distinct().ToHashSet();
        var months = request.UsersTimesheetsDto.Select(y => y.Month).Distinct().ToHashSet();

        var timesheets = await _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .Where(x => userIds.Contains(x.UserId) &&
                        projectIds.Contains(x.ProjectId) &&
                        years.Contains(x.ReportYear) &&
                        months.Contains(x.ReportMonthOfYear))
            .ToListAsync(cancellationToken);

        return timesheets
            .Where(x => request.UsersTimesheetsDto
                .Any(y => y.UserId == x.UserId &&
                          y.ProjectId == x.ProjectId &&
                          y.Year == x.ReportYear &&
                          y.Month == x.ReportMonthOfYear))
            .Select(x => new UserTimesheetByMonthDto
            {
                UserId = x.UserId,
                ProjectId = x.ProjectId,
                BillableTrackedSum = x.BillableTrackedInMinutes,
                Year = x.ReportYear,
                Month = x.ReportMonthOfYear
            })
            .ToList();
    }

    internal sealed record Query(IReadOnlyCollection<GetUserTimesheetByMonthDto> UsersTimesheetsDto) : IQuery<List<UserTimesheetByMonthDto>>;
}
