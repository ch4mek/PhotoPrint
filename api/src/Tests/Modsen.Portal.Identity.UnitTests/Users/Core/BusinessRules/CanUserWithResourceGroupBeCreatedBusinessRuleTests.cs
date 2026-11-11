using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class CanUserWithResourceGroupBeCreatedBusinessRuleTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    private readonly CanUserWithResourceGroupBeCreatedBusinessRule _rule;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserWithResourceGroupBeCreatedBusinessRuleTests()
    {
        _rule = new CanUserWithResourceGroupBeCreatedBusinessRule(
            _authenticatedUserContextMock.Object,
            _identityDbContextMock.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadAssigningResourceGroupNotFromHisDepartmentReturnBusinessRuleFailResult()
    {
        //Arrange
        var model = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
            null,
            1,
            "Employee");

        var manager = new User
        {
            Id = 3,
            DepartmentId = "Net",
            FirstName = "test",
            LastName = "test"
        };

        var group = new ResourceGroup
        {
            Id = 1,
            Name = "TestName",
            IsActive = true,
            DepartmentId = "Net"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Js");

        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup> { group }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { manager }.BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.HeadCannotAssignUserToGroupFromAnotherDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAssigningResourceGroupNotFromUsersDepartmentReturnBusinessRuleFailResult()
    {
        //Arrange
        var model = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
            "Js",
            1,
            "Employee");

        var manager = new User
        {
            Id = 3,
            DepartmentId = "Net",
            FirstName = "test",
            LastName = "test"
        };

        var group = new ResourceGroup
        {
            Id = 1,
            Name = "TestName",
            IsActive = true,
            DepartmentId = "Net"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup> { group }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { manager }.BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.ResourceGroupMustBeFromDepartment);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenGroupAlreadyHasGmReturnBusinessRuleFailResult()
    {
        //Arrange
        var model = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
            "Js",
            1,
            "GroupManager");

        var group = new ResourceGroup
        {
            Id = 1,
            Name = "TestName",
            IsActive = true,
            DepartmentId = "Net"
        };

        var manager = new User
        {
            Id = 3,
            DepartmentId = "Net",
            UserRoles = [new UserRole { Role = new Role { Name = PortalRoleNames.GroupManager, PublicName = "Test" } }],
            IsActive = true,
            ResourceGroupId = group.Id,
            FirstName = "test",
            LastName = "test"
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup> { group }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { manager }.BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Reason.Should().Be(PortalMessages.GroupAlreadyHasManager);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenRequestIsValidReturnBusinessRuleSuccessResult()
    {
        //Arrange
        var model = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
            "Net",
            1,
            "GroupManager");
       
        var group = new ResourceGroup
        {
            Id = 1,
            Name = "TestName",
            DepartmentId = "Net",
            IsActive = true,
        };

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        _identityDbContextMock
            .Setup(x => x.ResourceGroups)
            .Returns(new List<ResourceGroup> { group }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User>().BuildMockDbSet().Object);

        //Act
        var result = await _rule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
