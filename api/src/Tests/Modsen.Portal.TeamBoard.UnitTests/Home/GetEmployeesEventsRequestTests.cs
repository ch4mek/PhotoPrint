using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Home;

public class GetEmployeesEventsRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IPortalSettingsAccessor> _portalSettingsAccessorMock = new();

    private readonly GetEmployeesEventsRequest _employeesEventsRequestHandler;

    public GetEmployeesEventsRequestTests()
    {
        const string employeesEventsDaysSpan = "7";

        _portalSettingsAccessorMock
            .Setup(x => x.GetStringValueByName(
                PortalSettingNames.EmployeesEventsDaysSpan,
                It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(employeesEventsDaysSpan);

        _employeesEventsRequestHandler = new GetEmployeesEventsRequest(
            _authenticatedUserContextMock.Object,
            _teamBoardDbContextMock.Object,
            _clockServiceMock.Object,
            _portalSettingsAccessorMock.Object);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsDataFromTheirGroup_Success()
    {
        const string sameAsGroupManagerDepartmentId = "SameAsGroupManagerDepartmentId";
        const int sameAsGroupManagerResourceGroupId = 17;
        var todayUtc = new DateOnly(2026, 2, 15);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsGroupManagerDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Fits by dep., group and date
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsGroupManagerDepartmentId,
                    ResourceGroupId = sameAsGroupManagerResourceGroupId,
                    DateOfBirth = todayUtc.AddDays(5).AddYears(-20),
                    IsActive = true,
                },
                new()
                { // Fits by department and group, but hire date is to early
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = sameAsGroupManagerDepartmentId,
                    ResourceGroupId = sameAsGroupManagerResourceGroupId,
                    HireDate = todayUtc.AddDays(-1).AddYears(-2),
                    IsActive = true,
                },
                new()
                { // Does not fit by dep and group
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    FirstNameRu = "Имя3",
                    LastNameRu = "Фамилия3",
                    Email = "Email3",
                    DepartmentId = Guid.CreateVersion7().ToString(),
                    ResourceGroupId = 3,
                    DateOfBirth = todayUtc.AddYears(-25),
                    IsActive = true,
                },
                new()
                { // Does not fit by date
                    UserId = 4,
                    FirstName = "FName4",
                    LastName = "LName4",
                    FirstNameRu = "Имя4",
                    LastNameRu = "Фамилия4",
                    Email = "Email4",
                    DepartmentId = sameAsGroupManagerDepartmentId,
                    ResourceGroupId = sameAsGroupManagerResourceGroupId,
                    DateOfBirth = todayUtc.AddYears(-25).AddDays(10),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = sameAsGroupManagerDepartmentId,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                resourceGroupId: sameAsGroupManagerResourceGroupId,
                eventCelebrationDate: todayUtc.AddDays(5),
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                departmentId: sameAsGroupManagerDepartmentId,
                originalEventDate: todayUtc.AddDays(5).AddYears(-20),
                position: null
            ),
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsDataFromAnotherDepartment_WithException()
    {
        const string groupManagerDepartmentId = "GroupManagerDepartmentId";
        const string anotherDepartmentId = "AnotherDepartmentId";

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(groupManagerDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        var action = () => _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = anotherDepartmentId,
        }, CancellationToken.None);

        await action.Should().ThrowExactlyAsync<ActionNotAllowedException>();
    }

    [Fact]
    // When events spans from December to January and have different years. Plus, one user produces events of 2 types.
    public async Task HandleWhenManagerRequestsDataWithEventsFromDifferentYears_Success()
    {
        const string sameAsHeadOfDepartmentDepartmentId = "SameAsHeadOfDepartmentDepartmentId";
        var todayUtc = new DateOnly(2026, 12, 30);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDepartmentDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Will result 2 Events: Birthday in next year and hiring anniversary in current year
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    DateOfBirth = todayUtc.AddDays(5).AddYears(-50),
                    HireDate = todayUtc.AddYears(-20),
                    IsActive = true,
                },
                new()
                { // Fits by dep id, and hiring date
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    HireDate = todayUtc.AddYears(-2),
                    IsActive = true,
                },
                new()
                { // Fits by birthday, BUT does not fit by dep id
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    FirstNameRu = "Имя3",
                    LastNameRu = "Фамилия3",
                    Email = "Email3",
                    DepartmentId = Guid.CreateVersion7().ToString(),
                    DateOfBirth = todayUtc.AddYears(-25),
                    IsActive = true,
                },
                new()
                { // Does not fit by date
                    UserId = 4,
                    FirstName = "FName4",
                    LastName = "LName4",
                    FirstNameRu = "Имя4",
                    LastNameRu = "Фамилия4",
                    Email = "Email4",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    DateOfBirth = todayUtc.AddDays(10).AddYears(-25),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = sameAsHeadOfDepartmentDepartmentId,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                resourceGroupId: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.EmploymentAnniversary,
                userId: 1,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-20),
                position: null
            ),
            new(
                fullNameEng: "LName2 FName2",
                fullNameRu: "Фамилия2 Имя2",
                resourceGroupId: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.EmploymentAnniversary,
                userId: 2,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-2),
                position: null
            ),
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                resourceGroupId: null,
                eventCelebrationDate: todayUtc.AddDays(5),
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddDays(5).AddYears(-50),
                position: null
            ),
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenManagerRequestsDataWithEventsFromDifferentMonths_Success()
    {
        const string sameAsHeadOfDepartmentDepartmentId = "SameAsHeadOfDepartmentDepartmentId";
        var todayUtc = new DateOnly(2026, 10, 30);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDepartmentDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Will result 2 Events: Birthday in next year and hiring anniversary in current year
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    DateOfBirth = todayUtc.AddDays(5).AddYears(-20),
                    HireDate = todayUtc.AddYears(-20),
                    IsActive = true,
                },
                new()
                { // Fits by dep id, and hiring date
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    HireDate = todayUtc.AddYears(-2),
                    IsActive = true,
                },
                new()
                { // Fits by birthday, BUT does not fit by dep id
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    FirstNameRu = "Имя3",
                    LastNameRu = "Фамилия3",
                    Email = "Email3",
                    DepartmentId = Guid.CreateVersion7().ToString(),
                    DateOfBirth = todayUtc.AddYears(-25),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = sameAsHeadOfDepartmentDepartmentId,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                resourceGroupId: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.EmploymentAnniversary,
                userId: 1,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-20),
                position: null
            ),
            new(
                fullNameEng: "LName2 FName2",
                fullNameRu: "Фамилия2 Имя2",
                resourceGroupId: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.EmploymentAnniversary,
                userId: 2,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-2),
                position: null
            ),
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                resourceGroupId: null,
                eventCelebrationDate: todayUtc.AddDays(5),
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddDays(5).AddYears(-20),
                position: null
            )
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData("NET")]
    [InlineData("Java")]
    [InlineData((string)null!)]
    public async Task HandleWhenCEORequestsDataFromAllDepartments_Success(string? departmentId)
    {
        var todayUtc = new DateOnly(2026, 2, 15);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Ceo]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = "NET",
                    DateOfBirth = todayUtc.AddDays(5).AddYears(-20),
                    IsActive = true,
                },
                new()
                {
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = "Java",
                    HireDate = todayUtc.AddDays(1).AddYears(-2),
                    IsActive = true,
                },
                new()
                {
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    FirstNameRu = "Имя3",
                    LastNameRu = "Фамилия3",
                    Email = "Email3",
                    DepartmentId = Guid.CreateVersion7().ToString(), // random
                    ResourceGroupId = 3,
                    DateOfBirth = todayUtc.AddYears(-25),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = departmentId,
        }, CancellationToken.None);

        result.Should().NotBeEmpty();
        result.Should().HaveCount(departmentId.IsNullOrWhiteSpace() ? 3 : 1);
        result.Should().OnlyContain(x => departmentId.IsNullOrWhiteSpace() || x.DepartmentId == departmentId);
    }

    [Fact]
    public async Task HandleWhenManagerRequestsDataFromUserWithLeapYearBirthday_Success()
    {
        const string sameAsGroupManagerDepartmentId = "SameAsGroupManagerDepartmentId";
        const int sameAsGroupManagerResourceGroupId = 17;
        var todayUtc = new DateOnly(2026, 2, 25);
        var originalLeapYearBirthday = new DateOnly(2012, 2, 29);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsGroupManagerDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Fits by dep., group and date
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsGroupManagerDepartmentId,
                    ResourceGroupId = sameAsGroupManagerResourceGroupId,
                    DateOfBirth = originalLeapYearBirthday,
                    IsActive = true,
                },
                new()
                { // Does not fit
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    FirstNameRu = "Имя3",
                    LastNameRu = "Фамилия3",
                    Email = "Email3",
                    DepartmentId = Guid.CreateVersion7().ToString(),
                    ResourceGroupId = 3,
                    DateOfBirth = todayUtc.AddYears(-25),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = sameAsGroupManagerDepartmentId,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                eventCelebrationDate: new DateOnly(todayUtc.Year, 3, 1),
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                resourceGroupId: sameAsGroupManagerResourceGroupId,
                departmentId: sameAsGroupManagerDepartmentId,
                originalEventDate: originalLeapYearBirthday,
                position: null
            )
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentRequestsDataWithNullDepartmentId_Success()
    {
        const string sameAsHeadOfDepartmentDepartmentId = "SameAsHeadOfDepartmentDepartmentId";
        var todayUtc = new DateOnly(2026, 2, 15);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDepartmentDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Fits by dep., group and date
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    DateOfBirth = todayUtc.AddDays(5).AddYears(-20),
                    IsActive = true,
                },
                new()
                { // Another department
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = Guid.CreateVersion7().ToString(),
                    HireDate = todayUtc.AddDays(5).AddYears(-21),
                    IsActive = true,
                },
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = null,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                eventCelebrationDate: todayUtc.AddDays(5),
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                resourceGroupId: null,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddDays(5).AddYears(-20),
                position: null
            )
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenManagerRequestsDataWhereEmployeeHasBeenHiredToday_ShowOnlyEmployeesWithMoreThanYearAnniversary()
    {
        const string sameAsHeadOfDepartmentDepartmentId = "SameAsHeadOfDepartmentDepartmentId";
        var todayUtc = new DateOnly(2026, 2, 15);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDepartmentDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                { // Fits by date, 3 years hiring anniversary
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    HireDate = todayUtc.AddDays(5).AddYears(-3),
                    IsActive = true,
                },
                new()
                { // Was hired yesterday -> 0 years anniversary -> skip them
                    UserId = 2,
                    FirstName = "FName2",
                    LastName = "LName2",
                    FirstNameRu = "Имя2",
                    LastNameRu = "Фамилия2",
                    Email = "Email2",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    HireDate = todayUtc.AddDays(-1),
                    IsActive = true,
                },
                new()
                { // Fits by date, birthday
                    UserId = 3,
                    FirstName = "FName3",
                    LastName = "LName3",
                    Email = "Email3",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    DateOfBirth = todayUtc.AddYears(-20),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = null,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName3 FName3",
                fullNameRu: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.Birthday,
                userId: 3,
                resourceGroupId: null,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-20),
                position: null
            ),
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                eventCelebrationDate: todayUtc.AddDays(5),
                eventType: EmployeeEventType.EmploymentAnniversary,
                userId: 1,
                resourceGroupId: null,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddDays(5).AddYears(-3),
                position: null
            )
        };

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenManagerRequestsDataWhereEmployeeHasSuitableBirthDateButNotSuitableHireDate_Success()
    {
        const string sameAsHeadOfDepartmentDepartmentId = "SameAsHeadOfDepartmentDepartmentId";
        var todayUtc = new DateOnly(2026, 2, 15);

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDepartmentDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(new List<TeamBoardUser>
            {
                new()
                {
                    UserId = 1,
                    FirstName = "FName1",
                    LastName = "LName1",
                    Email = "Email1",
                    DepartmentId = sameAsHeadOfDepartmentDepartmentId,
                    HireDate = todayUtc,
                    DateOfBirth = todayUtc.AddYears(-20),
                    IsActive = true,
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(todayUtc);

        var result = await _employeesEventsRequestHandler.Handle(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = null,
        }, CancellationToken.None);

        var expectedResult = new EmployeeEventDto[]
        {
            new(
                fullNameEng: "LName1 FName1",
                fullNameRu: null,
                eventCelebrationDate: todayUtc,
                eventType: EmployeeEventType.Birthday,
                userId: 1,
                resourceGroupId: null,
                departmentId: sameAsHeadOfDepartmentDepartmentId,
                originalEventDate: todayUtc.AddYears(-20),
                position: null
            )
        };

        result.Should().BeEquivalentTo(expectedResult);
    }
}
