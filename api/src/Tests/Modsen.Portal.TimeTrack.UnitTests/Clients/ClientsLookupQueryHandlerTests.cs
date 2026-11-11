using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.UseCases.Clients.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class ClientsLookupQueryHandlerTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IClientLookupLoader> _clientLookupLoaderMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetClientsLookupRequest _handler;

    public ClientsLookupQueryHandlerTests()
    {
        _handler = new GetClientsLookupRequest(
            _authenticatedUserContextMock.Object,
            _clientLookupLoaderMock.Object,
            _identityContractMock.Object);
    }

    [Fact]
    public async Task HandleWhenRequestIsNotValidShouldThrowException()
    {
        // Arrange
        const string keyWord = "test";
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns(["SysAdmin"]);
        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        var request = new GetClientsLookupRequest.Query(
            keyWord,
            IsActive: null,
            OnlyManagedClients: false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenViewProjectExitsClaimAndNonPrivilegedRole_ShouldReturnEmptyList(string role)
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.IsCeo()).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.IsCto()).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.IsHeadOfDepartment()).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.IsGroupManager()).Returns(false);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);

        var request = new GetClientsLookupRequest.Query(null, null, null);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _clientLookupLoaderMock.Verify(x => x.LoadAsync(It.IsAny<ClientLookupRequest>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task HandleWhenManageAllProjectExitsClaim_ShouldCallLoaderWithForceAllTrue()
    {
        // Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);

        _clientLookupLoaderMock
            .Setup(x => x.LoadAsync(It.IsAny<ClientLookupRequest>(), _cancellationToken))
            .ReturnsAsync([]);

        var request = new GetClientsLookupRequest.Query(null, null, null);

        // Act
        await _handler.Handle(request, _cancellationToken);

        // Assert
        _clientLookupLoaderMock.Verify(
            x => x.LoadAsync(It.Is<ClientLookupRequest>(r => r.ForceAll), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenManageAllProjectExitsClaim_ShouldSkipValidation()
    {
        // Arrange
        const int userId = 1;

        _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);

        _clientLookupLoaderMock
            .Setup(x => x.LoadAsync(It.IsAny<ClientLookupRequest>(), _cancellationToken))
            .ReturnsAsync([]);

        var request = new GetClientsLookupRequest.Query(null, null, OnlyManagedClients: false);

        // Act
        var act = async () => await _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
