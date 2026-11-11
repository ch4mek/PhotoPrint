using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Identity.Contract;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Core.Builders;

public class FinanceClientsAndUsersQueryBuilderTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly FinanceClientsAndUsersQueryBuilder _builder;

    public FinanceClientsAndUsersQueryBuilderTests()
    {
        _builder = new FinanceClientsAndUsersQueryBuilder(
            _dbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _identityContractMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task BuildAvailableFinanceProjectUsersAsyncWhenAssignsAreNotBillableShouldReturnEmptyCollection()
    {
        // Arrange
        var currentUserId = 10;
        var managedClientId = 100;
        var anotherUserId = 888;

        var model = GetClientsAndProjectUsers(currentUserId, managedClientId, 200, anotherUserId, false);
        var query = Setup(model.ProjectUsers);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(Array.Empty<string>());

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
            .Returns(false);

        // Act
        var result = await _builder.BuildAvailableFinanceProjectUsersAsync(query, 2026, 7, _cancellationToken);

        // Assert
        var view = result.View.ToList();
        var manage = result.Manage.ToList();

        view.Should().HaveCount(0);
        manage.Should().HaveCount(0);
    }

    [Fact]
    public async Task BuildAvailableFinanceProjectUsersAsyncWhenUserHasNoGlobalRightsShouldReturnRestrictedQueries()
    {
        // Arrange
        var currentUserId = 10;
        var managedClientId = 100;
        var anotherUserId = 888;

        var model = GetClientsAndProjectUsers(currentUserId, managedClientId, 200, anotherUserId, true);
        var query = Setup(model.ProjectUsers);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(Array.Empty<string>());

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
            .Returns(false);

        // Act
        var result = await _builder.BuildAvailableFinanceProjectUsersAsync(query, 2026, 7, _cancellationToken);

        // Assert
        var view = result.View.ToList();
        var manage = result.Manage.ToList();

        view.Should().HaveCount(2);
        manage.Should().HaveCount(2);

        view.Should().Contain(x => x.UserId == currentUserId);
        view.Should().Contain(x => x.Project!.ClientId == managedClientId);

        view.Should().NotContain(x => x.UserId == anotherUserId);
    }

    [Fact]
    public async Task BuildAvailableFinanceProjectUsersAsyncWhenUserHasNoGlobalRightsAndHoDShouldReturnRestrictedQueries()
    {
        // Arrange
        var currentUserId = 10;
        var managedClientId = 100;
        var anotherUserId = 888;
        var userFromDepId = 666;

        var currentUserDep = "BA";

        var model = GetClientsAndProjectUsers(currentUserId, managedClientId, 200, anotherUserId, true);
        var projectUsers = model.ProjectUsers;
        projectUsers.Add(TestDataGenerator.GetFinanceProjectUserEntity(
                TestDataGenerator.GetFinanceProjectEntity(model.OtherClient),
                TestDataGenerator.GetFinanceUserEntity(userFromDepId, departmentId: currentUserDep),
                isTimeOnProjectBillable: true));
        var query = Setup(projectUsers);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(currentUserDep);

        _identityContractMock.Setup(x =>
            x.UsersWorkInDepartment(2026, It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>(), 7))
            .ReturnsAsync([currentUserId, userFromDepId]);

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
            .Returns(false);

        _clockServiceMock
            .Setup(x => x.TodayUtc())
            .Returns(new DateOnly(2026, 7, 2));

        // Act
        var result = await _builder.BuildAvailableFinanceProjectUsersAsync(query, 2026, 7, _cancellationToken);

        // Assert
        var view = result.View.ToList();
        var manage = result.Manage.ToList();

        view.Should().HaveCount(3);
        manage.Should().HaveCount(3);

        view.Should().Contain(x => x.UserId == currentUserId);
        view.Should().Contain(x => x.Project!.ClientId == managedClientId);
        view.Should().Contain(x => x.UserId == userFromDepId);

        view.Should().NotContain(x => x.UserId == anotherUserId);
    }

    [Fact]
    public async Task BuildAvailableFinanceProjectUsersAsyncWhenUserHasNoGlobalRightsAndGMShouldReturnRestrictedQueries()
    {
        // Arrange
        var currentUserId = 10;
        var managedClientId = 100;
        var anotherUserId = 888;
        var userFromGroupId = 666;

        var currentUserGroup = 1;

        var model = GetClientsAndProjectUsers(currentUserId, managedClientId, 200, anotherUserId, true);

        model.CurrentUser.ResourceGroupId = currentUserGroup;
        var userFromGroup = TestDataGenerator.GetFinanceUserEntity(userFromGroupId, resourceGroupId: currentUserGroup);

        var projectUsers = model.ProjectUsers;
        projectUsers.Add(TestDataGenerator.GetFinanceProjectUserEntity(
                TestDataGenerator.GetFinanceProjectEntity(model.OtherClient), userFromGroup, isTimeOnProjectBillable: true));

        var query = Setup(projectUsers);

        _dbContextMock.SetupGet(x => x.FinanceUsers).Returns(new[] { model.CurrentUser, userFromGroup }.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(currentUserGroup);

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
            .Returns(false);

        // Act
        var result = await _builder.BuildAvailableFinanceProjectUsersAsync(query, 2026, 7, _cancellationToken);

        // Assert
        var view = result.View.ToList();
        var manage = result.Manage.ToList();

        view.Should().HaveCount(3);
        manage.Should().HaveCount(3);

        view.Should().Contain(x => x.UserId == currentUserId);
        view.Should().Contain(x => x.Project!.ClientId == managedClientId);
        view.Should().Contain(x => x.UserId == userFromGroupId);

        view.Should().NotContain(x => x.UserId == anotherUserId);
    }

    [Fact]
    public async Task BuildAvailabletoViewFinanceProjectUsersAsyncWhenUserHasNoGlobalRightsShouldReturnRestrictedQueries()
    {
        // Arrange
        var currentUserId = 10;
        var managedClientId = 100;
        var anotherUserId = 888;

        var model = GetClientsAndProjectUsers(currentUserId, managedClientId, 200, anotherUserId, true);
        var query = Setup(model.ProjectUsers);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(Array.Empty<string>());

        // Act
        var result = await _builder.BuildAvailabletoViewFinanceProjectUsersAsync(query, 2026, 7, _cancellationToken);

        // Assert
        var users = result.ToList();

        users.Should().HaveCount(2);

        users.Should().Contain(x => x.UserId == currentUserId);
        users.Should().Contain(x => x.Project!.ClientId == managedClientId);

        users.Should().NotContain(x => x.UserId == anotherUserId);
    }

    private IQueryable<FinanceProjectUserEntity> Setup(List<FinanceProjectUserEntity> projectUsers)
    {
        var query = projectUsers.AsQueryable();

        _identityContractMock.Setup(x => x.GetCeoCtoIds(_cancellationToken)).ReturnsAsync([]);

        _authenticatedUserContextMock
            .Setup(x => x.HasClaim(FinanceClaimTypeNames.ViewAllRates.Name))
            .Returns(false);

        return query;
    }

    private ClientsAndProjectUsersModel GetClientsAndProjectUsers(int currentUserId, int managedClientId,
        int otherClientId, int anotherUserId, bool isTimeOnProjectBillable)
    {
        var currentUser = TestDataGenerator.GetFinanceUserEntity(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(currentUser.Id);

        var managedClient = TestDataGenerator.GetFinanceClientEntity(id: managedClientId);
        managedClient.SalesManagerId = currentUser.Id;

        var otherClient = TestDataGenerator.GetFinanceClientEntity(id: otherClientId);
        otherClient.SalesManagerId = 999;

        _dbContextMock.SetupGet(x => x.FinanceClients).Returns(new[] { managedClient, otherClient }.BuildMockDbSet().Object);

        var projectUsers = new List<FinanceProjectUserEntity>
        {
            TestDataGenerator.GetFinanceProjectUserEntity(
                TestDataGenerator.GetFinanceProjectEntity(otherClient), currentUser, isTimeOnProjectBillable: isTimeOnProjectBillable),
            TestDataGenerator.GetFinanceProjectUserEntity(
                TestDataGenerator.GetFinanceProjectEntity(managedClient), TestDataGenerator.GetFinanceUserEntity(777), isTimeOnProjectBillable: isTimeOnProjectBillable),
            TestDataGenerator.GetFinanceProjectUserEntity(
                TestDataGenerator.GetFinanceProjectEntity(otherClient), TestDataGenerator.GetFinanceUserEntity(anotherUserId), isTimeOnProjectBillable: isTimeOnProjectBillable),
        };

        return new ClientsAndProjectUsersModel(currentUser, managedClient, otherClient, projectUsers);
    }

    private record ClientsAndProjectUsersModel(
        FinanceUserEntity CurrentUser,
        FinanceClientEntity ManagedClient,
        FinanceClientEntity OtherClient,
        List<FinanceProjectUserEntity> ProjectUsers);
}
