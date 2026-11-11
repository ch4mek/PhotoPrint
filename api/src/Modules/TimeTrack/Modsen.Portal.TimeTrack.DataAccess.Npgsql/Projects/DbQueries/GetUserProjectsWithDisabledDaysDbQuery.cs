using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.Domain;
using Project = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;

internal class GetUserProjectsWithDisabledDaysDbQuery : IGetUserProjectsWithDisabledDaysDbQuery
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IEnvironmentFeatureContext _environmentFeatureContext;

    public GetUserProjectsWithDisabledDaysDbQuery(ITimeTrackDbContext timeTrackDbContext,
        IEnvironmentFeatureContext environmentFeatureContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _environmentFeatureContext = environmentFeatureContext;
    }

    public async Task<List<ProjectWithDisabledDays>> ExecuteAsync(GetUserProjectsWithDisabledDaysDbRequest request, CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Projects
            .Include(x => x.ProjectUsers)
            .AsNoTracking();

        if (request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value)
        {
            query = query.Where(x => x.ManagerUserId == request.CurrentUserId);
        }

        var checkAssignDates = await _environmentFeatureContext.IsCheckAssignDatesFeatureEnabledAsync();

        if (checkAssignDates)
        {
            var projects = await query
                    .Where(x => x.ProjectUsers.Any(y => y.UserId == request.UserId &&
                        (y.UserOnProjectTo == null || y.UserOnProjectTo >= request.TrackIntervalFrom) && y.UserOnProjectFrom <= request.TrackIntervalTo)
                            && !x.IsPublic && x.IsActive)
                    .Select(x => new Project
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Client = x.Client,
                        IsBillable = x.IsBillable,
                        Color = x.Color,
                        ProjectUsers = x.ProjectUsers
                        .Where(y => y.UserId == request.UserId &&
                            (y.UserOnProjectTo == null || y.UserOnProjectTo >= request.TrackIntervalFrom) && y.UserOnProjectFrom <= request.TrackIntervalTo)
                        .ToList()
                    })
                    .OrderBy(x => x.Id)
                    .ToListAsync(cancellationToken);

            return GetProjectsWithDisabledDays(projects, request.TrackIntervalFrom, request.TrackIntervalTo);
        }

        return await query
                    .Where(x => x.ProjectUsers.Any(y => y.UserId == request.UserId) && !x.IsPublic && x.IsActive)
                    .Select(x => new ProjectWithDisabledDays
                    {
                        ProjectLookup = new ProjectLookup
                        {
                            Id = x.Id,
                            Name = x.Name,
                            ClientName = x.Client!.Name,
                            IsBillable = x.IsBillable,
                            Color = x.Color
                        },
                        DisabledDates = Array.Empty<DateOnly>(),
                        BillableDates = Array.Empty<BillableDate>()
                    })
                    .OrderBy(x => x.ProjectLookup!.Id)
                    .ToListAsync(cancellationToken);
    }

    private static List<ProjectWithDisabledDays> GetProjectsWithDisabledDays(List<Project> projects, DateOnly trackIntervalFrom, DateOnly trackIntervalTo)
    {
        var result = new List<ProjectWithDisabledDays>();

        var allDays = Enumerable.Range(0, trackIntervalTo.DayNumber - trackIntervalFrom.DayNumber + 1)
            .Select(trackIntervalFrom.AddDays)
            .ToList();

        foreach (var project in projects)
        {
            var projectId = project.Id;
            var excludedDays = new HashSet<DateOnly>();
            var billableDays = new List<BillableDate>();
            
            foreach (var pu in project.ProjectUsers)
            {
                var projectStart = pu.UserOnProjectFrom > trackIntervalFrom ? pu.UserOnProjectFrom : trackIntervalFrom;
                var projectEnd = pu.UserOnProjectTo < trackIntervalTo ? pu.UserOnProjectTo : trackIntervalTo;

                var assignedDays = Enumerable.Range(0, projectEnd.Value.DayNumber - projectStart.DayNumber + 1)
                .Select(d => projectStart.AddDays(d));

                excludedDays.UnionWith(Enumerable.Range(0, projectEnd.Value.DayNumber - projectStart.DayNumber + 1)
                    .Select(d => projectStart.AddDays(d)));

                billableDays.AddRange(assignedDays.Select(day => new BillableDate
                {
                    Date = day,
                    IsBillable = pu.IsTimeOnProjectBillable
                }));
            }

            var missingDays = allDays.Where(day => !excludedDays.Contains(day)).ToArray();
            result.Add(new ProjectWithDisabledDays
            {
                ProjectLookup = new ProjectLookup
                {
                    Id = projectId,
                    Name = project.Name,
                    ClientName = project.Client!.Name,
                    IsBillable = project.IsBillable,
                    Color = project.Color
                },
                DisabledDates = missingDays,
                BillableDates = billableDays.ToArray()
            });
        }

        return result;
    }
}
