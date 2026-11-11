using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Users.BusinessRules;

namespace Modsen.Portal.Equipment.UnitTests.BusinessRule;

public class IsAllowedToGetUserEquipmentBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task IsCorrespondingToBusinessWhenRoleCanGetAllEquipmentShouldReturnTrue(string roleType)
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([roleType]);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenHeadOfDepartmentTryToGetEquipmentOfUserFromAnotherDepartmentShouldReturnFalse()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Net");

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                DepartmentId = "QA",
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenHeadOfDepartmentTryToGetEquipmentOfUserFromSameDepartmentShouldReturnFalse()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Net");

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                DepartmentId = "Net",
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenGroupManagerTryToGetEquipmentOfUserFromAnotherResourceGroupShouldReturnFalse()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(1);

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                ResourceGroupId = 2,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenGroupManagerTryToGetEquipmentOfUserFromSamerResourceGroupShouldReturnTrue()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock
            .Setup(x => x.ResourceGroupId)
            .Returns(1);

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                ResourceGroupId = 1,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenEmployeeTryToGetEquipmentFromUserWithAnotherIdShouldReturnFalse()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(2);

        var users = new List<EquipmentUser>
        {
            new()
            {
                UserId = 1,
                ResourceGroupId = 2,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            }
        };

        SetupUsers(users);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondingToBusinessWhenEmployeeTryToGetEquipmentFromUserWithSameIdShouldReturnTrue()
    {
        //Arrange
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);

        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(1);

        var isAllowedToGetUserEquipmentBusinessRule = new IsAllowedToGetUserEquipmentBusinessRule(
            _authenticatedUserContextMock.Object,
            _equipmentDbContextMock.Object);

        //Act
        var result = await isAllowedToGetUserEquipmentBusinessRule.IsCorrespondsToBusiness(1, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    private void SetupUsers(List<EquipmentUser> users)
    {
        _equipmentDbContextMock
            .Setup(mock => mock.EquipmentUsers)
            .Returns(users.BuildMockDbSet().Object);
    }
}
