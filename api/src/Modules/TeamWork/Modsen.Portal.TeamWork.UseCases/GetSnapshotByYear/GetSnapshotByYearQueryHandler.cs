using System.Globalization;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;

namespace Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;

internal sealed class GetSnapshotByYearQueryHandler : IQueryHandler<GetSnapshotByYearQuery, GetSnapshotByYearResponse<UserChild>>
{
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly ITeamWorkPortalSettingsAccessor _teamWorkPortalSettingsAccessor;

    public GetSnapshotByYearQueryHandler(
        ITimeTrackContract timeTrackContract,
        ITeamWorkPortalSettingsAccessor teamWorkPortalSettingsAccessor)
    {
        _timeTrackContract = timeTrackContract;
        _teamWorkPortalSettingsAccessor = teamWorkPortalSettingsAccessor;
    }

    public async Task<GetSnapshotByYearResponse<UserChild>> Handle(GetSnapshotByYearQuery request, CancellationToken cancellationToken)
    {
        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(request.StartYear, request.EndYear, request.DepartmentId,
            request.Projects, request.Clients, IsBillable: true);

        var groupedTimesheets = await _timeTrackContract.GetTimesheetByFilters(getTimesheetByFiltersRequest, cancellationToken);
        var settings = await _teamWorkPortalSettingsAccessor.GetSettings(cancellationToken);

        var total = new Total
        {
            Id = string.Empty,
            TotalTime = 0,
            EntriesCount = 0,
            TotalAmount = null,
            Amounts = []
        };
        var timeEntryReportViewModel = new GetSnapshotByYearResponse<UserChild>
        {
            Totals = [total],
            GroupOne = groupedTimesheets.Select(groupedTimesheet =>
            {
                var group = new Group<UserChild>
                {
                    Duration = groupedTimesheet.Duration * 60,
                    Id = new DateTime(groupedTimesheet.Year, groupedTimesheet.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture)
                };

                group.Name = group.Id;

                group.Children = groupedTimesheet.Users
                    .Select(child => new UserChild
                    {
                        Duration = child.Duration * 60,
                        HasMembership = true,
                        Id = child.Id,
                        Name = $"{child.FirstName} {child.LastName}",
                        NameLowerCase = $"{child.FirstName} {child.LastName}".ToLower(CultureInfo.InvariantCulture),
                        Amounts = []
                    })
                    .ToArray();

                total.EntriesCount += groupedTimesheet.Users.Sum(child => child.Count);
                total.TotalTime += group.Duration;

                return group;
            }).ToArray()
        };

        return timeEntryReportViewModel;
    }
}
