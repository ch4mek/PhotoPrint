using System.Linq.Expressions;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core;
using Modsen.Portal.TimeTrack.UseCases.Reports.Core.Dto;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetClientsWithTotalTrackedTimeRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserMock = new();
    private readonly Mock<IClientsWithTotalTrackedTimeResolver> _resolverMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetClientsWithTotalTrackedTimeRequest _useCase;

    public GetClientsWithTotalTrackedTimeRequestTests()
    {
        _useCase = new GetClientsWithTotalTrackedTimeRequest(
            _authenticatedUserMock.Object,
            _resolverMock.Object
        );
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenUserIsCEOorCTOShouldReturnClientsAndCallGetTotalTrackedTimeDbQueryOnce(string userRole)
    {
        // Arrange
        const int clientId = 1;
        const int projectId = 1;

        var clients = new List<ClientWithTotalTrackedTimeCoreDto>
        {
            new()
            {
                Name = "Apple",
                Color = "#43sf12",
                TotalTimeInMinutes = 250
            },
            new()
            {
                Name = "Panasonic",
                Color = "#43sf20",
                TotalTimeInMinutes = 380
            }
        };

        var request = new GetClientsWithTotalTrackedTimeRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ResourceGroupId: null,
            clientId,
            projectId,
            UserId: null,
            TrackedTimeType.Fact);

        _authenticatedUserMock.Setup(x => x.GetRoles())
            .Returns([userRole]);

        Expression<Func<ClientsWithTotalTrackedTimeResolverRequest, bool>> requestMatch =
            request =>
                request.ClientId == clientId
                && request.ProjectId == projectId;

        _resolverMock
            .Setup(x => x.ResolveAsync(It.Is(requestMatch), _cancellationToken))
            .ReturnsAsync(clients);

        // Act
        var result = await _useCase.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(clients, opt => opt.WithStrictOrdering());

        _resolverMock
            .Verify(query => query.ResolveAsync(It.Is(requestMatch), _cancellationToken),
                Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentShouldReturnClientsAndCallGetTotalTrackedTimeDbQueryOnce()
    {
        // Arrange
        const int clientId = 1;
        const int projectId = 1;
        const string userRole = PortalRoleNames.HeadOfDepartment;
        const string departmentId = "Qa";

        var clients = new List<ClientWithTotalTrackedTimeCoreDto>
        {
            new()
            {
                Name = "Apple",
                Color = "#43sf12",
                TotalTimeInMinutes = 250
            },
            new()
            {
                Name = "Panasonic",
                Color = "#43sf20",
                TotalTimeInMinutes = 380
            }
        };

        var request = new GetClientsWithTotalTrackedTimeRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            departmentId,
            ResourceGroupId: null,
            clientId,
            projectId,
            UserId: null,
            TrackedTimeType: TrackedTimeType.Fact);

        _authenticatedUserMock.Setup(x => x.GetRoles())
            .Returns([userRole]);

        _authenticatedUserMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        Expression<Func<ClientsWithTotalTrackedTimeResolverRequest, bool>> dbRequestMatch =
            dbRequest =>
                dbRequest.DepartmentId == departmentId
                && dbRequest.ClientId == clientId
                && dbRequest.ProjectId == projectId;

        _resolverMock
            .Setup(x => x.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(clients);

        // Act
        var result = await _useCase.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(clients, opt => opt.WithStrictOrdering());

        _resolverMock
            .Verify(query => query.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken),
                Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerShouldReturnClientsAndCallGetTotalTrackedTimeDbQueryOnce()
    {
        // Arrange
        const int clientId = 1;
        const int projectId = 1;
        const string userRole = PortalRoleNames.GroupManager;
        const string departmentId = "Net";
        const int resourceGroupId = 1;

        var clients = new List<ClientWithTotalTrackedTimeCoreDto>
        {
            new()
            {
                Name = "Apple",
                Color = "#43sf12",
                TotalTimeInMinutes = 250
            },
            new()
            {
                Name = "Panasonic",
                Color = "#43sf20",
                TotalTimeInMinutes = 380
            }
        };

        var request = new GetClientsWithTotalTrackedTimeRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ResourceGroupId: null,
            clientId,
            projectId,
            UserId: null,
            TrackedTimeType: TrackedTimeType.Fact);

        _authenticatedUserMock.Setup(x => x.GetRoles())
            .Returns([userRole]);

        _authenticatedUserMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _authenticatedUserMock.Setup(x => x.ResourceGroupId)
            .Returns(resourceGroupId);

        Expression<Func<ClientsWithTotalTrackedTimeResolverRequest, bool>> dbRequestMatch =
            dbRequest =>
                dbRequest.ClientId == clientId
                && dbRequest.ProjectId == projectId
                && dbRequest.ResourceGroupId == resourceGroupId;

        _resolverMock
            .Setup(x => x.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(clients);

        // Act
        var result = await _useCase.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(clients, opt => opt.WithStrictOrdering());

        _resolverMock
            .Verify(query => query.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken),
                Times.Once);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenUserIsEmployeeOrSysAdminShouldReturnClientsAndCallGetTotalTrackedTimeDbQueryOnce(string userRole)
    {
        // Arrange
        const int clientId = 1;
        const int projectId = 1;
        const int userId = 1;

        var clients = new List<ClientWithTotalTrackedTimeCoreDto>
        {
            new()
            {
                Name = "Apple",
                Color = "#43sf12",
                TotalTimeInMinutes = 250
            },
            new()
            {
                Name = "Panasonic",
                Color = "#43sf20",
                TotalTimeInMinutes = 380
            }
        };

        var request = new GetClientsWithTotalTrackedTimeRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ResourceGroupId: null,
            clientId,
            projectId,
            UserId: null,
            TrackedTimeType: TrackedTimeType.Fact);

        _authenticatedUserMock.Setup(x => x.GetRoles())
            .Returns([userRole]);

        _authenticatedUserMock.Setup(x => x.UserId)
            .Returns(userId);

        Expression<Func<ClientsWithTotalTrackedTimeResolverRequest, bool>> dbRequestMatch =
            dbRequest =>
                dbRequest.UserId == userId
                && dbRequest.ClientId == clientId
                && dbRequest.ProjectId == projectId;

        _resolverMock
            .Setup(x => x.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(clients);

        // Act
        var result = await _useCase.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(clients, opt => opt.WithStrictOrdering());

        _resolverMock
            .Verify(query => query.ResolveAsync(It.Is(dbRequestMatch), _cancellationToken),
                Times.Once);
    }
}
