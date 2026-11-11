using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Departments.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Departments;

public class GetUserAndSubordinatesIdsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly GetUserAndSubordinatesIdsRequest _request;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserAndSubordinatesIdsRequestTests()
    {
        _request = new GetUserAndSubordinatesIdsRequest(
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object,
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndIncludeSelfReturnsSubordinatesIds()
    {
        // Arrange
        var departmentId = "Dep_1";
        var year = 2025;
        var month = 3;

        var departmentUsersIds = new List<int> { 1, 2, 3 };

        SetupAuthenticatedUser(1, departmentId: departmentId, roles: [PortalRoleNames.HeadOfDepartment]);

        _identityContractMock
            .Setup(x => x.UsersWorkInDepartment(year, new List<string> { departmentId }, _cancellationToken, month))
            .ReturnsAsync(departmentUsersIds);

        // Act
        var result = await _request.Handle(GetQuery(year: year, month: month), _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(departmentUsersIds);

        _identityContractMock.Verify(
            x => x.UsersWorkInDepartment(year, new List<string> { departmentId }, _cancellationToken, month),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndDoesNotIncludeSelfRemovesInitiatorId()
    {
        // Arrange
        var initiatorId = 1;
        var departmentId = "Dep_1";
        var year = 2025;
        var month = 3;

        var departmentUsersIds = new List<int> { 1, 2, 3 };

        SetupAuthenticatedUser(initiatorId, departmentId: departmentId, roles: [PortalRoleNames.HeadOfDepartment]);

        _identityContractMock
            .Setup(x => x.UsersWorkInDepartment(year, new List<string> { departmentId }, _cancellationToken, month))
            .ReturnsAsync(departmentUsersIds);

        // Act
        var result = await _request.Handle(GetQuery(year: year, month: month, includeSelf: false), _cancellationToken);

        // Assert
        result.Should().NotContain(initiatorId);
        result.Should().BeEquivalentTo(departmentUsersIds.Except([initiatorId]));

        _identityContractMock.Verify(
            x => x.UsersWorkInDepartment(year, new List<string> { departmentId }, _cancellationToken, month),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerAndIncludeSelfReturnsGroupMembers()
    {
        // Arrange
        var resourceGroupId = 1;
        var userIds = new List<int> { 1, 2, 3, 4 };

        SetupAuthenticatedUser(
            userIds[0],
            resourceGroupId: resourceGroupId,
            roles: [PortalRoleNames.GroupManager]);

        var users = new List<User>
        {
            GetUser(userIds[0], resourceGroupId),
            GetUser(userIds[1], resourceGroupId),
            GetUser(userIds[2], resourceGroupId),
            GetUser(userIds[3], resourceGroupId + 1),
        };

        _identityDbContextMock.SetupGet(x => x.Users).Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(GetQuery(), _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo([userIds[0], userIds[1], userIds[2]]);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerAndDoesNotIncludeSelfRemovesInitiatorId()
    {
        // Arrange
        var resourceGroupId = 1;
        var userIds = new List<int> { 1, 2, 3, 4 };
        var initiatorId = userIds[0];

        SetupAuthenticatedUser(
            userIds[0],
            resourceGroupId: resourceGroupId,
            roles: [PortalRoleNames.GroupManager]);

        var users = new List<User>
        {
            GetUser(userIds[0], resourceGroupId),
            GetUser(userIds[1], resourceGroupId),
            GetUser(userIds[2], resourceGroupId),
            GetUser(userIds[3], resourceGroupId + 1),
        };

        _identityDbContextMock.SetupGet(x => x.Users).Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _request.Handle(GetQuery(includeSelf: false), _cancellationToken);

        // Assert
        result.Should().NotContain(initiatorId);
        result.Should().BeEquivalentTo([userIds[1], userIds[2]]);
    }

    [Fact]
    public async Task HandleWhenUserHasNoSpecialRoleAndIncludeSelfReturnsOnlyInitiatorId()
    {
        // Arrange
        const int initiatorId = 1;

        SetupAuthenticatedUser(initiatorId, roles: [PortalRoleNames.Employee]);

        var query = new GetUserAndSubordinatesIdsRequest.Query(
            2025,
            3,
            IncludeSelf: true);

        // Act
        var result = await _request.Handle(GetQuery(), _cancellationToken);

        // Assert
        result.Should().ContainSingle().Which.Should().Be(initiatorId);
    }

    [Fact]
    public async Task HandleWhenUserHasNoSpecialRoleAndDoesNotIncludeSelfReturnsEmptyList()
    {
        // Arrange
        const int initiatorId = 1;

        SetupAuthenticatedUser(initiatorId, roles: [PortalRoleNames.Employee]);

        // Act
        var result = await _request.Handle(GetQuery(includeSelf: false), _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    private GetUserAndSubordinatesIdsRequest.Query GetQuery(int year = 2026, int month = 6, bool includeSelf = true)
    {
        return new GetUserAndSubordinatesIdsRequest.Query(year, month, includeSelf);
    }

    private void SetupAuthenticatedUser(int initiatorId, string? departmentId = null, int? resourceGroupId = null,
        List<string>? roles = null)
    {
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(initiatorId);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _authenticatedUserContextMock.SetupGet(x => x.ResourceGroupId).Returns(resourceGroupId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(roles ?? []);
    }

    private User GetUser(int id, int resourceGroupId)
    {
        return new User
        {
            Id = id,
            FirstName = Guid.CreateVersion7().ToString(),
            LastName = Guid.CreateVersion7().ToString(),
            ResourceGroupId = resourceGroupId
        };
    }
}
