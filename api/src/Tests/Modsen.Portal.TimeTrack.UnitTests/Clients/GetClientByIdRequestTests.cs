using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class GetClientByIdRequestTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContext = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
    { CallBase = true }.As<IAuthenticatedUserContext>();

    private readonly CancellationToken _cancellationToken;
    private readonly GetClientByIdRequest _handler;

    public GetClientByIdRequestTests()
    {
        _handler = new GetClientByIdRequest(
            _timeTrackDbContext.Object,
            _authenticatedUserContextMock.Object);

        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task GetClientWhenClientExistReturnSuccess()
    {
        //Arrange
        var clientId = 1;

        var model = new Client
        {
            Id = clientId,
            Name = "TestName",
            IsActive = true,
            Projects = [],
            RegisteredLocation = "Belarus"
        };

        var expectedModel = new ClientDto
        {
            Id = clientId,
            Name = "TestName",
            FactTrackedTimeInMinutes = 0,
            BillableTrackedTimeInMinutes = 0,
            IsActive = true,
            IsProjectExist = false,
            RegisteredLocation = "Belarus"
        };

        var currentUserId = 1;
        string[] currentUserRoles = [PortalRoleNames.Ceo];

        SetupClients([model]);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(currentUserRoles);

        //Act
        var result = await _handler.Handle(new GetClientByIdRequest.Query(clientId), _cancellationToken);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedModel);
    }

    [Fact]
    public async Task GetClientWhenClientDoesNotExistReturnNotFoundException()
    {
        //Arrange
        var clientId = 1;

        var currentUserId = 1;
        string[] currentUserRoles = [PortalRoleNames.Ceo];

        SetupClients([]);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(currentUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(currentUserRoles);

        //Act
        var result = () => _handler.Handle(new GetClientByIdRequest.Query(clientId), _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContext
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
