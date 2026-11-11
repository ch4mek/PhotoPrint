using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetDashboardOverview;

public class GetDashboardOverviewQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<IDashboardOverviewLoader> _getDashboardOverviewLoaderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetDashboardOverviewRequest _handler;

    public GetDashboardOverviewQueryHandlerTests()
    {
        _handler = new GetDashboardOverviewRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _getDashboardOverviewLoaderMock.Object);
    }

    [Fact]
    public async Task GetDashboardUsersWithValidRequestReturnsDashboardManagerPagingUserViewModel()
    {
        // Arrange
        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");

        var monthOfYear = new[] { 11, 12 };

        _getDashboardOverviewLoaderMock
            .Setup(q => q.LoadAsync(2023, monthOfYear, 1, "Net", null, _token))
            .ReturnsAsync(new DashboardOverviewCoreDto
            {
                TotalBillableTime = 18000,
                TotalFactTime = 24000,
                TopBillableProject = "Modsen App",
                TopFactProject = "Modsen App",
                TopBillableClient = "Modsen",
                TopFactClient = "Modsen",
                TopBillableProjectId = 1,
                TopFactProjectId = 1,
                TopBillableClientId = 1,
                TopFactClientId = 1
            });

        var expectedResult = new DashboardOverviewCoreDto
        {
            TotalBillableTime = 18000,
            TotalFactTime = 24000,
            TopBillableProject = "Modsen App",
            TopFactProject = "Modsen App",
            TopBillableClient = "Modsen",
            TopFactClient = "Modsen",
            TopBillableProjectId = 1,
            TopFactProjectId = 1,
            TopBillableClientId = 1,
            TopFactClientId = 1
        };

        // Act
        var result = await _handler.Handle(new GetDashboardOverviewRequest.Query(2023, monthOfYear, 1, "Net"), _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
