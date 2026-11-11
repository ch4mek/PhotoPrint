using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings.CoreDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.PerformanceReview;

public class GetPerformanceReviewLookupRequestTests
{
    private readonly GetPerformanceReviewLookupRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();
    private readonly Mock<IPerformanceReviewDateTypeCalculator> _performanceReviewDateTypeCalculator = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITeamBoardPortalSettingsAccessor> _teamBoardPortalSettingsAccessor = new();
    private readonly Mock<IClockService> _clockService = new();

    public GetPerformanceReviewLookupRequestTests()
    {
        _handler = new(_teamBoardDbContext.Object, _performanceReviewDateTypeCalculator.Object,
            _authenticatedUserContext.Object, _teamBoardPortalSettingsAccessor.Object,
            _clockService.Object);

        _clockService.Setup(x => x.TodayStartOfDayUtc()).Returns(DateTime.Today);
        _teamBoardPortalSettingsAccessor.Setup(x => x.GetSettings(_cancellationToken))
            .ReturnsAsync(new TeamBoardPortalSettingsDto
            {
                UpcomingIminentBorderDaysCount = 3
            });
    }

    [Theory]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenFilteringDepartmentIdSpecifiedUnderNotCeoCtoThrowActionNotAllowed(string position)
    {
        // Arrange
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([position]);
        var query = new GetPerformanceReviewLookupRequest.Query(7, 30, "Department");

        // Act
        var act = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenAuthAsNotManagerReturnEmpty(string position)
    {
        // Assert
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([position]);
        var query = new GetPerformanceReviewLookupRequest.Query(7, 30, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenAuthedAsHoDReturnOnlyHisDepartmentPerformanceReviews()
    {
        // Assert
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Department");

        var performanceReviews = new[]
        {
            new UserPerformanceReview
            {
                Id = 1,
                InterviewDateFromUtc = DateTime.Today.AddDays(2),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email",
                    IsActive = true,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DepartmentId = "Department"
                },
            },
            new UserPerformanceReview
            {
                Id = 2,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email2",
                    IsActive = true,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    DepartmentId = "Department"
                },
            },
            new UserPerformanceReview
            {
                Id = 3,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email3",
                    IsActive = true,
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                    DepartmentId = "Another Department"
                },
            },
        };

        _teamBoardDbContext.SetupGet(x => x.UserPerformanceReviews)
            .Returns(performanceReviews.BuildMockDbSet().Object);

        var dateType = "Future";
        _performanceReviewDateTypeCalculator.Setup(x => x.Calculate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(dateType);

        var query = new GetPerformanceReviewLookupRequest.Query(7, 30, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        var expected = new[]
        {
            new PerformanceReviewLookupDto
            {
                Id = 2,
                Date = DateTime.Today.AddDays(1),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName2",
                UserLastName = "LastName2"
            },
            new PerformanceReviewLookupDto
            {
                Id = 1,
                Date = DateTime.Today.AddDays(2),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName",
                UserLastName = "LastName"
            },
        };

        result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task HandleWhenAuthedAsChiefReturnOnlyHisDepartmentPerformanceReviews(string position)
    {
        // Assert
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([position]);
        var departmentId = "Department";

        var performanceReviews = new[]
        {
            new UserPerformanceReview
            {
                Id = 1,
                InterviewDateFromUtc = DateTime.Today.AddDays(2),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email",
                    IsActive = true,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    DepartmentId = "Department"
                },
            },
            new UserPerformanceReview
            {
                Id = 2,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email2",
                    IsActive = true,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    DepartmentId = "Department"
                },
            },
            new UserPerformanceReview
            {
                Id = 3,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email3",
                    IsActive = true,
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                    DepartmentId = "Another Department"
                },
            },
        };

        _teamBoardDbContext.SetupGet(x => x.UserPerformanceReviews)
            .Returns(performanceReviews.BuildMockDbSet().Object);

        var dateType = "Future";
        _performanceReviewDateTypeCalculator.Setup(x => x.Calculate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(dateType);

        var query = new GetPerformanceReviewLookupRequest.Query(7, 30, departmentId);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        var expected = new[]
        {
            new PerformanceReviewLookupDto
            {
                Id = 2,
                Date = DateTime.Today.AddDays(1),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName2",
                UserLastName = "LastName2"
            },
            new PerformanceReviewLookupDto
            {
                Id = 1,
                Date = DateTime.Today.AddDays(2),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName",
                UserLastName = "LastName"
            },
        };

        result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleWhenAuthedAsGmReturnOnlyHisResourceGroupPerformanceReviews()
    {
        // Assert
        var groupId = 25;
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContext.Setup(x => x.ResourceGroupId).Returns(groupId);

        var performanceReviews = new[]
        {
            new UserPerformanceReview
            {
                Id = 1,
                InterviewDateFromUtc = DateTime.Today.AddDays(2),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email",
                    IsActive = true,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    ResourceGroupId = groupId
                },
            },
            new UserPerformanceReview
            {
                Id = 2,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email2",
                    IsActive = true,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    ResourceGroupId = groupId
                },
            },
            new UserPerformanceReview
            {
                Id = 3,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email3",
                    IsActive = true,
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                    ResourceGroupId = 1234,
                },
            },
        };

        _teamBoardDbContext.SetupGet(x => x.UserPerformanceReviews)
            .Returns(performanceReviews.BuildMockDbSet().Object);

        var dateType = "Future";
        _performanceReviewDateTypeCalculator.Setup(x => x.Calculate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(dateType);

        var query = new GetPerformanceReviewLookupRequest.Query(7, 30, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        var expected = new[]
        {
            new PerformanceReviewLookupDto
            {
                Id = 2,
                Date = DateTime.Today.AddDays(1),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName2",
                UserLastName = "LastName2"
            },
            new PerformanceReviewLookupDto
            {
                Id = 1,
                Date = DateTime.Today.AddDays(2),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName",
                UserLastName = "LastName"
            },
        };

        result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
    }

    [Fact]
    public async Task HandleWhenRequestIsValidReturnFilteredValid()
    {
        // Assert
        var overdueDays = 7;
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        var performanceReviews = new[]
        {
            new UserPerformanceReview
            {
                Id = 1,
                InterviewDateFromUtc = DateTime.Today.AddDays(2),
                Status = UserPerformanceReviewStatus.Passed,
                User = new TeamBoardUser
                {
                    Email = "Email",
                    IsActive = true,
                    FirstName = "FirstName",
                    LastName = "LastName",
                    Position = PortalRoleNames.Employee
                },
            },
            new UserPerformanceReview
            {
                Id = 2,
                InterviewDateFromUtc = DateTime.Today.AddDays(1 + overdueDays),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email2",
                    IsActive = true,
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    Position = PortalRoleNames.Employee
                },
            },
            new UserPerformanceReview
            {
                Id = 3,
                InterviewDateFromUtc = DateTime.Today.AddDays(1),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email3",
                    IsActive = false,
                    FirstName = "FirstName3",
                    LastName = "LastName3",
                    Position = PortalRoleNames.Employee
                },
            },
            new UserPerformanceReview
            {
                Id = 4,
                InterviewDateFromUtc = DateTime.Today.AddDays(overdueDays),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email4",
                    IsActive = true,
                    FirstName = "FirstName4",
                    LastName = "LastName4",
                    Position = PortalRoleNames.Employee
                },
            },
            new UserPerformanceReview
            {
                Id = 5,
                InterviewDateFromUtc = DateTime.Today.AddDays(-5 * overdueDays),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email5",
                    IsActive = true,
                    FirstName = "FirstName5",
                    LastName = "LastName5",
                    Position = PortalRoleNames.Employee
                },
            },
            new UserPerformanceReview
            {
                Id = 6,
                InterviewDateFromUtc = DateTime.Today.AddDays(-5 * overdueDays),
                Status = UserPerformanceReviewStatus.Pending,
                User = new TeamBoardUser
                {
                    Email = "Email5",
                    IsActive = true,
                    FirstName = "FirstName5",
                    LastName = "LastName5",
                    Position = PortalRoleNames.Intern
                },
            },
        };

        _teamBoardDbContext.SetupGet(x => x.UserPerformanceReviews)
            .Returns(performanceReviews.BuildMockDbSet().Object);

        var dateType = "Future";
        _performanceReviewDateTypeCalculator.Setup(x => x.Calculate(It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(dateType);

        var query = new GetPerformanceReviewLookupRequest.Query(overdueDays, 7, null);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        var expected = new[]
        {
            new PerformanceReviewLookupDto
            {
                Id = 5,
                Date = DateTime.Today.AddDays(-5 * overdueDays),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName5",
                UserLastName = "LastName5",
                UserPosition = PortalRoleNames.Employee
            },
            new PerformanceReviewLookupDto
            {
                Id = 4,
                Date = DateTime.Today.AddDays(overdueDays),
                PerformanceReviewDateType = dateType,
                UserFirstName = "FirstName4",
                UserLastName = "LastName4",
                UserPosition = PortalRoleNames.Employee
            }
        };

        result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
    }
}
