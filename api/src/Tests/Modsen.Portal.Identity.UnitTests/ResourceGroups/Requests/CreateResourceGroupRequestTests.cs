using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Requests;

public class CreateResourceGroupRequestTests
{
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<ICanUserUpdateGroupBusinessRule> _canUserUpdateGroupBusinessRuleMock = new();
    private readonly Mock<IIsNewGroupManagerValidBusinessRule> _isNewGroupManagerValidBusinessRule = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly CreateResourceGroupRequest _createResourceGroupRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateResourceGroupRequestTests()
    {
        _createResourceGroupRequest = new(
            _communicationBusMock.Object,
            _canUserUpdateGroupBusinessRuleMock.Object,
            _isNewGroupManagerValidBusinessRule.Object,
            _identityDbContextMock.Object,
            _authenticatedUserContext.Object);   
    }

    [Fact]
    public async Task HandleWhenUserIsCanNotCreateShouldThrowException()
    {
        // Arrange
        var reason = "reason";
        var groupManagerId = 1;
        var departmentId = ".Net";
        var groupName = "groupName";

        SetupIsUserCanCreate(BusinessRuleResult.Fail(reason));

        var request = new CreateResourceGroupRequest.Command(groupManagerId, departmentId, groupName);

        // Act
        var result = () => _createResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(reason);
    }

    [Fact]
    public async Task HandleWhenGroupManagerIsNotValidShouldThrowException()
    {
        // Arrange
        var reason = "reason";
        var groupManagerId = 1;
        var departmentId = ".Net";
        var groupName = "groupName";

        SetupIsUserCanCreate(BusinessRuleResult.Success());
        SetupIsGroupManagerValid(BusinessRuleResult.Fail(reason));

        var request = new CreateResourceGroupRequest.Command(groupManagerId, departmentId, groupName);

        // Act
        var result = () => _createResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(reason);
    }

    [Fact]
    public async Task HandleWhenGroupNameIsExistShouldThrowException()
    {
        // Arrange
        var groupManagerId = 1;
        var departmentId = ".Net";
        var groupName = "groupName";

        var resourceGroup = new ResourceGroup()
        {
            Id = 1,
            Name = groupName,
            DepartmentId = departmentId
        };

        SetupIsUserCanCreate(BusinessRuleResult.Success());
        SetupIsGroupManagerValid(BusinessRuleResult.Success());
        SetupGroups([resourceGroup]);

        var request = new CreateResourceGroupRequest.Command(groupManagerId, departmentId, groupName);

        // Act
        var result = () => _createResourceGroupRequest.Handle(request, _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.GroupWithTheSameNameExists);
    }

    private void SetupGroups(List<ResourceGroup> resourceGroups)
    {
        _identityDbContextMock
            .Setup(mock => mock.ResourceGroups)
            .Returns(resourceGroups.BuildMockDbSet().Object);
    }

    private void SetupIsGroupManagerValid(BusinessRuleResult result)
    {
        _isNewGroupManagerValidBusinessRule
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<IsNewGroupManagerValidBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(result);
    }

    private void SetupIsUserCanCreate(BusinessRuleResult result)
    {
        _canUserUpdateGroupBusinessRuleMock
            .Setup(mock => mock.IsCorrespondsToBusiness(It.IsAny<CanUserUpdateGroupBusinessRule.Request>(), _cancellationToken))
            .ReturnsAsync(result);
    }
}
