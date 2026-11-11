using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Requests;

public class DeleteResourceGroupRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICanUserUpdateGroupBusinessRule> _canUserUpdateGroupBusinessRuleMock = new();
    private readonly DeleteResourceGroupRequest _deleteResourceGroupRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DeleteResourceGroupRequestTests()
    {
        _deleteResourceGroupRequest = new(
            _identityDbContextMock.Object,
            _canUserUpdateGroupBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenGroupIsNotExistShouldThrowException()
    {
        // Arrange
        var groupId = 1;

        SetupResourceGroups([]);

        var request = new DeleteResourceGroupRequest.Command(groupId);

        // Act
        var result = () => _deleteResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.GroupNotFound);
    }

    [Fact]
    public async Task HandleWhenUserCanNotDeleteGroupShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var reason = "reason";

        var group = new ResourceGroup
        {
            Id = groupId,
            Name = "RG_TEST",
            DepartmentId = "Net"
        };

        SetupResourceGroups([group]);
        SetupIsCanUserDeleteGroup(BusinessRuleResult.Fail(reason));

        var request = new DeleteResourceGroupRequest.Command(groupId);

        // Act
        var result = () => _deleteResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(reason);
    }

    [Fact]
    public async Task HandleWhenGroupHasUsersShouldThrowException()
    {
        // Arrange
        var groupId = 1;

        var group = new ResourceGroup
        {
            Id = groupId,
            Name = "RG_TEST",
            DepartmentId = "Net"
        };

        var user = new User
        {
            Id = 1,
            IsActive = true,
            ResourceGroupId = groupId,
            FirstName = "test",
            LastName = "test",
        };

        SetupResourceGroups([group]);
        SetupIsCanUserDeleteGroup(BusinessRuleResult.Success());
        SetupUsers([user]);

        var request = new DeleteResourceGroupRequest.Command(groupId);

        // Act
        var result = () => _deleteResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.GroupCannotBeDeleted);
    }

    private void SetupUsers(List<User> users)
    {
        _identityDbContextMock
            .Setup(mock => mock.Users)
            .Returns(users.BuildMockDbSet().Object);
    }

    private void SetupIsCanUserDeleteGroup(BusinessRuleResult result)
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
