using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardUsers;

public class GetDashboardUsersQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<IDashboardUsersLoader> _dashboardUsersLoader = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardUsersRequest _handler;

    public GetDashboardUsersQueryHandlerTests()
    {
        _handler = new GetDashboardUsersRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _dashboardUsersLoader.Object);
    }

    [Fact]
    public async Task GetDashboardUsersWithValidRequestReturnsDashboardManagerPagingUserViewModel()
    {
        // Arrange
        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");

        var monthOfYear = new[] { 11, 12 };

        var users = new List<DashboardUser>
        {
            new ()
            {
                UserId = 1,
                Color = "#ABC4E",
                CountOfProjects = 1,
                TotalTrackedInMinutes = 180,
                UserName = "Ivan Ivanov"
            },
            new ()
            {
                UserId = 2,
                Color = "#ABC4E",
                CountOfProjects = 1,
                TotalTrackedInMinutes = 270,
                UserName = "Anna Anonymous"
            }
        };

        _dashboardUsersLoader
            .Setup(q => q.LoadAsync(1, 2, 2023, monthOfYear, "Net", null, _token))
            .ReturnsAsync(new DashboardUsersCoreDto
            {
                Page = 1,
                PageSize = 2,
                TotalCount = 4,
                Items = users
            });

        // Act
        var result = await _handler.Handle(new GetDashboardUsersRequest.Query(2023, monthOfYear, 1, 2, "Net"), _token);

        // Assert
        result.TotalCount.Should().Be(4);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.Items.Should().BeEquivalentTo(users);
    }
}
