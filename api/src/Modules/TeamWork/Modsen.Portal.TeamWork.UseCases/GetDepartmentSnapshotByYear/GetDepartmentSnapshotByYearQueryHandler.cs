using System.Globalization;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;

namespace Modsen.Portal.TeamWork.UseCases.GetDepartmentSnapshotByYear;

internal sealed class GetDepartmentSnapshotByYearQueryHandler : IQueryHandler<GetDepartmentSnapshotByYearQuery, GetSnapshotByYearResponse<DepartmentChild>>
{
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly ITeamWorkPortalSettingsAccessor _teamWorkPortalSettingsAccessor;

    public GetDepartmentSnapshotByYearQueryHandler(
        ITimeTrackContract timeTrackContract,
        ITeamWorkPortalSettingsAccessor teamWorkPortalSettingsAccessor)
    {
        _timeTrackContract = timeTrackContract;
        _teamWorkPortalSettingsAccessor = teamWorkPortalSettingsAccessor;
    }

    public async Task<GetSnapshotByYearResponse<DepartmentChild>> Handle(GetDepartmentSnapshotByYearQuery request, CancellationToken cancellationToken)
    {
        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(request.StartYear, request.EndYear, request.DepartmentId,
            request.Projects, request.Clients, IsBillable: true);

        var groupedTimesheets = await _timeTrackContract.GetDepartmentTimesheetByFilters(getTimesheetByFiltersRequest, cancellationToken);
        var settings = await _teamWorkPortalSettingsAccessor.GetSettings(cancellationToken);

        var total = new Total
        {
            Id = string.Empty,
            TotalTime = 0,
            EntriesCount = 0,
            TotalAmount = null,
            Amounts = []
        };

        var timeEntryReportViewModel = new GetSnapshotByYearResponse<DepartmentChild>
        {
            Totals = [total],
            GroupOne = groupedTimesheets.Select(groupedTimesheet =>
                {
                    var group = new Group<DepartmentChild>
                    {
                        Duration = groupedTimesheet.Duration * 60,
                        Id = new DateTime(groupedTimesheet.Year, groupedTimesheet.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                            .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture)
                    };

                    group.Name = group.Id;
                    group.Children = groupedTimesheet.Departments
                        .Select(child => new DepartmentChild
                        {
                            Duration = child.Duration * 60,
                            Name = child.DepartmentId,
                            Amounts = [],
                            Ids = child.DepartmentId != null ? [child.DepartmentId] : []
                        })
                        .ToArray();

                    total.EntriesCount += groupedTimesheet.Departments.Sum(child => child.Count);


                    total.TotalTime += group.Duration;

                    return group;
                })
                .ToArray()
        };

        return timeEntryReportViewModel;
    }
}
