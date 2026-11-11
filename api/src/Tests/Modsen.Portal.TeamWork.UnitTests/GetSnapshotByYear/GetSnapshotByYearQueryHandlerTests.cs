using System.Globalization;
using Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.TeamWork.UnitTests.GetSnapshotByYear;

public class GetSnapshotByYearQueryHandlerTests
{
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();
    private readonly Mock<ITeamWorkPortalSettingsAccessor> _settingsAccessorMock = new();

    private readonly GetSnapshotByYearQueryHandler _hendler;
    private readonly CancellationToken _token = CancellationToken.None;

    public GetSnapshotByYearQueryHandlerTests()
    {
        _hendler = new GetSnapshotByYearQueryHandler(_timeTrackContractMock.Object, _settingsAccessorMock.Object);

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TeamWorkPortalSettingsDto
            {
                MonthDayYearFormat = "MM.dd.yyyy",
                MonthYearFormat = "MMM yyyy"
            });
    }

    [Fact]
    public async Task GetSnapshotByYearWhenDataDoesNotExistReturnEmptyModel()
    {
        //Arrange
        var projects = new List<int>();
        var clients = new List<int>();
        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients, true);
        _timeTrackContractMock.Setup(q => q.GetTimesheetByFilters(getTimesheetByFiltersRequest, _token)).ReturnsAsync(
            []);

        var expectedResult = new GetSnapshotByYearResponse<UserChild>
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
        var result = await _hendler.Handle(new GetSnapshotByYearQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetSnapshotByYearWhenDataExistReturnExpectedModel()
    {
        //Arrange
        var projects = new List<int>();
        var clients = new List<int>();

        var groupedTimesheets = new List<TimesheetByFiltersOutput>
        {
            new ()
            {
                Duration = 360,
                Month = DateTime.UtcNow.Month,
                Year = DateTime.UtcNow.Year,
                Users = new List<TimesheetUserOutput>
                {
                    new ()
                    {
                        Count = 6,
                        Duration = 360,
                        FirstName = "Test",
                        Id = 6,
                        LastName = "Employee"
                    }
                }
            },
            new ()
            {
                Duration = 30,
                Month = DateTime.UtcNow.AddMonths(1).Month,
                Year = DateTime.UtcNow.AddMonths(1).Year,
                Users = new List<TimesheetUserOutput>
                {
                    new ()
                    {
                        Count = 2,
                        Duration = 30,
                        FirstName = "Test1",
                        Id = 17,
                        LastName = "Employee1"
                    }
                }
            }
        };

        var getTimesheetByFiltersRequest = new TimesheetByFiltersQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients, true);

        _timeTrackContractMock.Setup(q => q.GetTimesheetByFilters(getTimesheetByFiltersRequest, _token)).ReturnsAsync(groupedTimesheets);

        var settings = await _settingsAccessorMock.Object.GetSettings(CancellationToken.None);

        var expectedResult = new GetSnapshotByYearResponse<UserChild>
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
                new Group<UserChild>
                {
                    Amounts = [],
                    Duration = 21600,
                    Id = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new UserChild
                        {
                            Amounts = [],
                            Duration = 21600,
                            HasMembership = true,
                            Id = 6,
                            Name = "Test Employee",
                            NameLowerCase = "test employee"
                        }
                    ]
                },
                new Group<UserChild>
                {
                    Amounts = [],
                    Duration = 1800,
                    Id = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(settings.MonthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new UserChild
                        {
                            Amounts = [],
                            Duration = 1800,
                            HasMembership = true,
                            Id = 17,
                            Name = "Test1 Employee1",
                            NameLowerCase = "test1 employee1"
                        }
                    ]
                }
            ]
        };

        //Act
        var result = await _hendler.Handle(new GetSnapshotByYearQuery(DateTime.UtcNow.Year, DateTime.UtcNow.Year + 1,
            DepartmentId: null, projects, clients), _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
