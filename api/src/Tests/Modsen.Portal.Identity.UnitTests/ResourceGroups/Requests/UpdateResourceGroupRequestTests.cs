using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.Resolvers;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Requests;

public class UpdateResourceGroupRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICanUserUpdateGroupBusinessRule> _canUserUpdateGroupBusinessRuleMock = new();
    private readonly Mock<IIsNewGroupManagerValidBusinessRule> _isNewGroupManagerValidBusinessRule = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IUpdateGroupManagerResolver> _updateGroupManagerMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly UpdateResourceGroupRequest _updateResourceGroupRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateResourceGroupRequestTests()
    {
        _updateResourceGroupRequest = new(
            _identityDbContextMock.Object,
            _canUserUpdateGroupBusinessRuleMock.Object,
            _isNewGroupManagerValidBusinessRule.Object,
            _communicationBusMock.Object,
            _updateGroupManagerMock.Object,
            _authenticatedUserContext.Object);
    }

    [Fact]
    public async Task HandleWhenResourceGroupIsNotExistShouldThrowExcepion()
    {
        // Arrange
        var groupId = 1;
        var groupName = "Name";
        var groupManagerId = 1;
        var departmentId = "departmentId";

        SetupResourceGroups([]);

        var request = new UpdateResourceGroupRequest.Command(groupId, groupName, groupManagerId, departmentId);

        // Act
        var result = () => _updateResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.GroupNotFound);
    }

    [Fact]
    public async Task HandleWhenUserCanNotUpdateGroupShouldThrowExcepion()
    {
        // Arrange
        var groupId = 1;
        var groupName = "Name";
        var groupManagerId = 1;
        var departmentId = "departmentId";
        var reason = "reason";

        var group = new ResourceGroup
        {
            Id = groupId,
            IsActive = true,
            DepartmentId = departmentId,
            Name = "RG_TEST"
        };

        SetupResourceGroups([group]);
        SetupIsUserCanUpdateGroup(BusinessRuleResult.Fail(reason));

        var request = new UpdateResourceGroupRequest.Command(groupId, groupName, groupManagerId, departmentId);

        // Act
        var result = () => _updateResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(reason);
    }

    [Fact]
    public async Task HandleWhenGroupWithTheSameNameExistShouldThrowExcepion()
    {
        // Arrange
        var groupId = 1;
        var groupName = "Name";
        var groupManagerId = 1;
        var departmentId = "departmentId";

        var group = new ResourceGroup
        {
            Id = groupId,
            IsActive = true,
            DepartmentId = departmentId,
            Name = groupName,
        };

        var groupWithSameName = new ResourceGroup
        {
            Id = 100,
            IsActive = true,
            DepartmentId = departmentId,
            Name = groupName,
        };

        var user = new User
        {
            Id = groupManagerId,
            IsActive = true,
            ResourceGroupId = groupId,
            FirstName = "test",
            LastName = "test",
            UserRoles =
            [
                new UserRole {
                    Role = new Role {
                        Name = PortalRoleNames.GroupManager,
                        PublicName = "Test"
                    }
                }
            ]
        };

        SetupUsers([user]);
        SetupResourceGroups([group, groupWithSameName]);
        SetupIsUserCanUpdateGroup(BusinessRuleResult.Success());
        SetupIsGmValid(BusinessRuleResult.Success());

        var request = new UpdateResourceGroupRequest.Command(groupId, groupName, groupManagerId, departmentId);

        // Act
        var result = () => _updateResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.GroupWithTheSameNameExists);
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupIsGmValid(BusinessRuleResult result)
    {
        _isNewGroupManagerValidBusinessRule
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<IsNewGroupManagerValidBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupIsUserCanUpdateGroup(BusinessRuleResult result)
    {
        _canUserUpdateGroupBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanUserUpdateGroupBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupResourceGroups(List<ResourceGroup> resourceGroups)
    {
        _identityDbContextMock
            .Setup(mock => mock.ResourceGroups)
            .Returns(resourceGroups.BuildMockDbSet().Object);
    }
}
