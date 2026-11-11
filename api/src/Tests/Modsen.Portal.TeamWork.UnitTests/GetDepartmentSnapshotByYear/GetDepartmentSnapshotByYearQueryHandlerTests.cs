using System.Globalization;
using Modsen.Portal.TeamWork.UseCases.GetDepartmentSnapshotByYear;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.TeamWork.UnitTests.GetDepartmentSnapshotByYear;

public class GetDepartmentSnapshotByYearQueryHandlerTests
{
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();

    private readonly GetDepartmentSnapshotByYearQueryHandler _hendler;
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly Mock<ITeamWorkPortalSettingsAccessor> _settingsAccessorMock = new();

    public GetDepartmentSnapshotByYearQueryHandlerTests()
    {
        _hendler = new GetDepartmentSnapshotByYearQueryHandler(
            _timeTrackContractMock.Object,
            _settingsAccessorMock.Object);

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamWorkPortalSettingsDto
            {
                MonthDayYearFormat = "MM.dd.yyyy",
                MonthYearFormat = "MMM yyyy"
            });
    }

    [Fact]
    public async Task GetDepartmentSnapshotByYearWhenDataDoesNotExistReturnEmptyModel()
    {
        //Arrange
        var projects = new List<int>();
        var clients = new List<int>();
        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients, true);

        _timeTrackContractMock.Setup(q => q.GetDepartmentTimesheetByFilters(getTimesheetByFiltersRequest, _token)).ReturnsAsync(
            []);

        var expectedResult = new GetSnapshotByYearResponse<DepartmentChild>
        {
            Totals =
            [
                new Total
                {
                    Id = string.Empty,
                    EntriesCount = 0,
                    TotalTime = 0,
                    TotalAmount = null,
                    Amounts = []
                }
            ]
        };

        //Act
        var result = await _hendler.Handle(new GetDepartmentSnapshotByYearQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetDepartmentSnapshotByYearWhenDataExistReturnExpectedModel()
    {
        //Arrange
        var projects = new List<int>();
        var clients = new List<int>();

        var groupedTimesheets = new List<DepartmentTimesheetsByFiltersOutput>
        {
            new ()
            {
                Duration = 360,
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year,
                Departments = new List<TimesheetDepartmentOutput>
                {
                    new ()
                    {
                        Count = 6,
                        Duration = 360,
                        DepartmentId = "Net"
                    }
                }
            },
            new ()
            {
                Duration = 30,
                Month = DateTime.UtcNow.AddMonths(1).Month,
                Year = DateTime.UtcNow.AddMonths(1).Year,
                Departments = new List<TimesheetDepartmentOutput>
                {
                    new ()
                    {
                        Count = 2,
                        Duration = 30,
                        DepartmentId = "Java"
                    }
                }
            }
        };

        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients, true);

        _timeTrackContractMock.Setup(q => q.GetDepartmentTimesheetByFilters(getTimesheetByFiltersRequest, _token))
            .ReturnsAsync(groupedTimesheets);

        var settings = await _settingsAccessorMock.Object.GetSettings(CancellationToken.None);

        var expectedResult = new GetSnapshotByYearResponse<DepartmentChild>
        {
            Totals =
            [
                new Total
                {
                    Amounts = [],
                    EntriesCount = 8,
                    Id = "",
                    TotalAmount = null,
                    TotalTime = 23400
                }
            ],
            GroupOne =
            [
                new Group<DepartmentChild>
                {
                    Amounts = [],
                    Duration = 21600,
                    Id = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new DepartmentChild { Amounts = [], Duration = 21600, Ids = ["Net"], Name = "Net" }
                    ]
                },
                new Group<DepartmentChild>
                {
                    Amounts = [],
                    Duration = 1800,
                    Id = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new DepartmentChild { Amounts = [], Duration = 1800, Ids = ["Java"], Name = "Java" }
                    ]
                }
            ]
        };

        //Act
        var result = await _hendler.Handle(new GetDepartmentSnapshotByYearQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
