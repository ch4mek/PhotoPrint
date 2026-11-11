using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Interviews.Requests;

public class GetEmployeesInterviewsTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly GetEmployeesInterviewsRequest _getEmployeesInterviewsRequestHandler;

    private readonly DateOnly _mockTodayDate = new(2026, 2, 15);

    public GetEmployeesInterviewsTests()
    {
        _getEmployeesInterviewsRequestHandler = new GetEmployeesInterviewsRequest(
            _authenticatedUserContextMock.Object,
            _teamBoardDbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsDataFromTheirGroupSuccess()
    {
        const string sameAsGroupManagerDepartmentId = "SameAsGroupManagerDepartmentId";
        const int sameAsGroupManagerResourceGroupId = 17;

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsGroupManagerDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        var testUsers = new List<TeamBoardUser>
        {
            new()
            { // Fits
                UserId = 1,
                FirstName = "FName1",
                LastName = "LName1",
                FirstNameRu = "Имя1",
                LastNameRu = "Фамилия1",
                Email = "Email1",
                DepartmentId = sameAsGroupManagerDepartmentId,
                ResourceGroupId = sameAsGroupManagerResourceGroupId,
                IsActive = true,
                Position = "Employee",
            },
            new()
            { // Fits by group & dept
                UserId = 2,
                FirstName = "FName2",
                LastName = "LName2",
                FirstNameRu = "Имя2",
                LastNameRu = "Фамилия2",
                Email = "Email2",
                DepartmentId = sameAsGroupManagerDepartmentId,
                ResourceGroupId = sameAsGroupManagerResourceGroupId,
                IsActive = true,
                Position = "Employee",
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
                IsActive = true,
                Position = "Employee",
            },
            new()
            { // Does not fit
                UserId = 4,
                FirstName = "Intern",
                LastName = "Intern",
                FirstNameRu = "Стажёр",
                LastNameRu = "Стажёр",
                Email = "Intern1",
                DepartmentId = sameAsGroupManagerDepartmentId,
                ResourceGroupId = sameAsGroupManagerResourceGroupId,
                IsActive = true,
                Position = PortalRoleNames.Intern,
            },
        };

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(testUsers.BuildMockDbSet().Object);

        _teamBoardDbContextMock
            .Setup(x => x.UserInterviews)
            .Returns(new List<UserInterview>
            {
                new()
                { // Fits
                    Id = 11,
                    UserId = 1,
                    User = testUsers[0],
                    Specialization = "TestSpecialization1",
                    UserInterviewDate = _mockTodayDate,
                    UserInterviewFromUtc = new TimeOnly(10, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                },
                new()
                { // Does not fit: tomorrow date
                    Id = 22,
                    UserId = 2,
                    User = testUsers[1],
                    Specialization = "TestSpecialization2",
                    UserInterviewDate = _mockTodayDate.AddDays(1),
                    UserInterviewFromUtc = new TimeOnly(10, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                },
                new()
                { // Does not fits (user has another dep)
                    Id = 33,
                    UserId = 3,
                    User = testUsers[2],
                    Specialization = "TestSpecialization3",
                    UserInterviewDate = _mockTodayDate,
                    UserInterviewFromUtc = new TimeOnly(11, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                },
                new()
                { // Does not fits (user is intern)
                    Id = 44,
                    UserId = 4,
                    User = testUsers[3],
                    Specialization = "TestSpecialization4",
                    UserInterviewDate = _mockTodayDate,
                    UserInterviewFromUtc = new TimeOnly(10, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                },
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(_mockTodayDate);

        var expectedResult = new EmployeeInterviewDto(
            id: 11,
            userId: 1,
            fullNameRu: "Фамилия1 Имя1",
            fullNameEng: "LName1 FName1",
            departmentId: sameAsGroupManagerDepartmentId,
            resourceGroupId: sameAsGroupManagerResourceGroupId,
            specialization: "TestSpecialization1",
            interviewDate: _mockTodayDate,
            interviewFromUtc: new TimeOnly(10, 00),
            status: UserInterviewStatus.NotStarted,
            userPosition: "Employee"
        );

        var result = await _getEmployeesInterviewsRequestHandler.Handle(
            new GetEmployeesInterviewsRequest.Query
            {
                DepartmentId = sameAsGroupManagerDepartmentId
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo([expectedResult]);
    }

    [Fact]
    public async Task HandleWhenHeadOfDeptRequestsDataFromAnotherDeptExceptionIsThrown()
    {
        const string sameAsHeadOfDeptDepartmentId = "SameAsHeadOfDeptDepartmentId";

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDeptDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        var action = () => _getEmployeesInterviewsRequestHandler.Handle(
            new GetEmployeesInterviewsRequest.Query
            {
                DepartmentId = Guid.CreateVersion7().ToString()
            },
            CancellationToken.None);

        await action.Should().ThrowExactlyAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(".NET")]
    [InlineData("Jaba")]
    [InlineData("JS")]
    public async Task HandleWhenCtoRequestsDataSuccess(string? departmentId)
    {
        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        var testUsers = new List<TeamBoardUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "FName1",
                LastName = "LName1",
                Email = "Email1",
                DepartmentId = ".NET",
                IsActive = true,
                Position = "Employee",
            },
            new()
            {
                UserId = 2,
                FirstName = "FName2",
                LastName = "LName2",
                FirstNameRu = "Имя2",
                LastNameRu = "Фамилия2",
                Email = "Email2",
                DepartmentId = ".NET",
                IsActive = true,
                Position = "Employee",
            },
            new()
            {
                UserId = 3,
                FirstName = "FName3",
                LastName = "LName3",
                FirstNameRu = "Имя3",
                LastNameRu = "Фамилия3",
                Email = "Email3",
                DepartmentId = ".NET",
                IsActive = true,
                Position = "Employee",
            },
            new()
            {
                UserId = 4,
                FirstName = "FName4",
                LastName = "LName4",
                FirstNameRu = "Имя4",
                LastNameRu = "Фамилия4",
                Email = "Email4",
                DepartmentId = "Jaba",
                IsActive = true,
                Position = "Employee",
            },
            new()
            {
                UserId = 5,
                FirstName = "FName5",
                LastName = "LName5",
                FirstNameRu = "Имя5",
                LastNameRu = "Фамилия5",
                Email = "Email5",
                DepartmentId = "Jaba",
                IsActive = true,
                Position = "Employee",
            },
            new()
            {
                UserId = 6,
                FirstName = "FName6",
                LastName = "LName6",
                FirstNameRu = "Имя6",
                LastNameRu = "Фамилия6",
                Email = "Email6",
                DepartmentId = "JS",
                IsActive = true,
                Position = "Employee",
            }
        };

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(testUsers.BuildMockDbSet().Object);

        var testInterviews = new List<UserInterview>
        {
            new()
            {
                Id = 11,
                UserId = 1,
                User = testUsers[0],
                Specialization = ".NET Backend",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(14, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new()
            {
                Id = 22,
                UserId = 2,
                User = testUsers[1],
                Specialization = ".NET Backend",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(15, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new()
            {
                Id = 33,
                UserId = 3,
                User = testUsers[2],
                Specialization = ".NET Fullstack",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(16, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new()
            {
                Id = 44,
                UserId = 4,
                User = testUsers[3],
                Specialization = "Jaba Backend",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(10, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new()
            {
                Id = 55,
                UserId = 5,
                User = testUsers[4],
                Specialization = "Jaba Backend",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(11, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new()
            {
                Id = 66,
                UserId = 6,
                User = testUsers[5],
                Specialization = "JS",
                UserInterviewDate = _mockTodayDate,
                UserInterviewFromUtc = new TimeOnly(10, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            },
            new() // tomorrow
            {
                Id = 66,
                UserId = 6,
                User = testUsers[5],
                Specialization = "JS",
                UserInterviewDate = _mockTodayDate.AddDays(1),
                UserInterviewFromUtc = new TimeOnly(10, 00),
                Status = UserInterviewStatus.NotStarted,
                IsActive = true
            }
        };

        _teamBoardDbContextMock
            .Setup(x => x.UserInterviews)
            .Returns(testInterviews.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(_mockTodayDate);

        var expectedResult = testInterviews
            .Where(x => departmentId.IsNullOrWhiteSpace() || x.User!.DepartmentId == departmentId)
            .Where(x => x.UserInterviewDate == _mockTodayDate)
            .Select(x => new EmployeeInterviewDto(
                id: x.Id,
                userId: x.UserId,
                fullNameRu: x.User!.FirstNameRu.IsNullOrWhiteSpace() && x.User.LastNameRu.IsNullOrWhiteSpace()
                    ? null
                    : (x.User.LastNameRu + " " + x.User.FirstNameRu).Trim(),
                fullNameEng: x.User!.FirstName.IsNullOrWhiteSpace() && x.User.LastName.IsNullOrWhiteSpace()
                    ? null
                    : (x.User.LastName + " " + x.User.FirstName).Trim(),
                departmentId: x.User.DepartmentId,
                resourceGroupId: x.User.ResourceGroupId,
                specialization: x.Specialization,
                interviewDate: x.UserInterviewDate,
                interviewFromUtc: x.UserInterviewFromUtc,
                status: x.Status,
                userPosition: x.User!.Position
            ))
            .OrderBy(x => x.UserInterviewDate).ThenBy(x => x.UserFullNameRu);

        var result = await _getEmployeesInterviewsRequestHandler.Handle(
            new GetEmployeesInterviewsRequest.Query
            {
                DepartmentId = departmentId
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentRequestsDataWithNullDepartmentIdSuccess()
    {
        const string sameAsHeadOfDeptDepartmentId = "SameAsHeadOfDeptDepartmentId";

        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(true);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns(sameAsHeadOfDeptDepartmentId);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        var testUsers = new List<TeamBoardUser>
        {
            new()
            { // Fits
                UserId = 1,
                FirstName = "FName1",
                LastName = "LName1",
                FirstNameRu = "Имя1",
                LastNameRu = "Фамилия1",
                Email = "Email1",
                DepartmentId = sameAsHeadOfDeptDepartmentId,
                IsActive = true,
                Position = "Employee",
            },
            new()
            { // Another dept
                UserId = 2,
                FirstName = "FName2",
                LastName = "LName2",
                FirstNameRu = "Имя2",
                LastNameRu = "Фамилия2",
                Email = "Email2",
                DepartmentId = Guid.CreateVersion7().ToString(),
                IsActive = true,
                Position = "Employee",
            },
        };

        _teamBoardDbContextMock
            .Setup(x => x.TeamBoardUsers)
            .Returns(testUsers.BuildMockDbSet().Object);

        _teamBoardDbContextMock
            .Setup(x => x.UserInterviews)
            .Returns(new List<UserInterview>
            {
                new()
                { // Fits
                    Id = 11,
                    UserId = 1,
                    User = testUsers[0],
                    Specialization = "TestSpecialization1",
                    UserInterviewDate = _mockTodayDate,
                    UserInterviewFromUtc = new TimeOnly(10, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                },
                new()
                { // Fits
                    Id = 22,
                    UserId = 2,
                    User = testUsers[1],
                    Specialization = "TestSpecialization2",
                    UserInterviewDate = _mockTodayDate,
                    UserInterviewFromUtc = new TimeOnly(10, 00),
                    Status = UserInterviewStatus.NotStarted,
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(_mockTodayDate);

        var expectedResult = new EmployeeInterviewDto(
            id: 11,
            userId: 1,
            fullNameRu: "Фамилия1 Имя1",
            fullNameEng: "LName1 FName1",
            departmentId: sameAsHeadOfDeptDepartmentId,
            resourceGroupId: null,
            specialization: "TestSpecialization1",
            interviewDate: _mockTodayDate,
            interviewFromUtc: new TimeOnly(10, 00),
            status: UserInterviewStatus.NotStarted,
            userPosition: "Employee"
        );

        var result = await _getEmployeesInterviewsRequestHandler.Handle(
            new GetEmployeesInterviewsRequest.Query
            {
                DepartmentId = null
            },
            CancellationToken.None);

        result.Should().BeEquivalentTo([expectedResult]);
    }
}
