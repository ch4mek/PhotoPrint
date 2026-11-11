using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class GetTimesheetsByProjectIdsRequest : IQueryHandler<GetTimesheetsByProjectIdsRequest.Query,
    List<GetTimesheetsByProjectsIdsDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetTimesheetsByProjectIdsRequest(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<GetTimesheetsByProjectsIdsDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var timesheetProjects = await _timeTrackDbContext.Timesheets
            .Where(timesheet => request.ProjectsIds.Contains(timesheet.ProjectId))
            .GroupBy(timesheet => timesheet.ProjectId)
            .Select(grouping => new GetTimesheetsByProjectsIdsDto
            (
                grouping.Key,
                grouping.Sum(timesheet => timesheet.FactTrackedInMinutes),
                grouping.Sum(timesheet => timesheet.BillableTrackedInMinutes)
            ))
            .ToListAsync(cancellationToken);

        var existingProjectIds = timesheetProjects.Select(timesheetResponse => timesheetResponse.ProjectId)
            .ToHashSet();

        var missingProjects = await _timeTrackDbContext.Projects
            .Where(project => request.ProjectsIds.Contains(project.Id) && !existingProjectIds.Contains(project.Id))
            .Select(project => new GetTimesheetsByProjectsIdsDto
            (
                project.Id,
                0,
                0
            ))
            .ToListAsync(cancellationToken);

        timesheetProjects.AddRange(missingProjects);

        return timesheetProjects;
    }

    internal sealed record Query(IReadOnlyCollection<int> ProjectsIds) : IQuery<List<GetTimesheetsByProjectsIdsDto>>;
}
