using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class UpdateClientCommandHandlerTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContext = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IIsUserSalesManagerBusinessRule> iIsUserSalesManagerBusinessRule = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateClientRequest _handler;

    public UpdateClientCommandHandlerTests()
    {
        _handler = new UpdateClientRequest(
            _timeTrackDbContext.Object,
            _teamBoardContractMock.Object,
            _communicationBusMock.Object,
            iIsUserSalesManagerBusinessRule.Object);
    }

    [Fact]
    public async Task HandleWhenClientDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new UpdateClientRequest.Command(
            Id: 7453,
            Name: "Lorem Ipsum",
            LegalEntity: "Lorem Ipsum",
            Color: "#000000",
            Comments: "Comments",
            RegisteredLocation: 1,
            SalesManagerId: 2);

        SetupClients([]);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenClientWithSameNameAlreadyExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var request = new UpdateClientRequest.Command(
            Id: 7453,
            Name: "Sit Amet",
            LegalEntity: "Sit Amet",
            Color: "#000000",
            Comments: "Comments",
            RegisteredLocation: 1,
            SalesManagerId:2);

        var client = new Client
        {
            Id = request.Id,
            IsActive = true,
            Name = "Sit Amet",
            Color = "#0f0f0f",
            FactTrackedTime = 0,
            Comments = request.Comments,
            Projects = [],
            RegisteredLocation = "Belarus"
        };

        var existingClient = new Client
        {
            Id = request.Id + 63,
            IsActive = true,
            Name = "Sit Amet",
            Color = request.Name,
            FactTrackedTime = 0,
            Comments = request.Comments,
            Projects = [],
            RegisteredLocation = "Belarus"
        };

        SetupClients([client, existingClient]);

        var countries = new List<CountryOutput>
        {
            new()
            {
                Id = 1,
                Name = "Belarus"
            }
        };

        _teamBoardContractMock
            .Setup(mock => mock.GetCountries(It.IsAny<CancellationToken>()))
            .ReturnsAsync(countries);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.ClientWithTheSameNameExists);
    }

    [Fact]
    public async Task HandleWhenClientIsNotActiveShouldThrowActionNotAllowedException()
    {
        // Arrange
        var request = new UpdateClientRequest.Command(
            Id: 7453,
            Name: "Lorem Ipsum",
            LegalEntity: "Lorem Ipsum",
            Color: "#000000",
            Comments: "Comments",
            1,
            SalesManagerId: 2);

        var client = new Client
        {
            Id = request.Id,
            IsActive = false,
            Name = "Sit Amet",
            Color = "#0f0f0f",
            FactTrackedTime = 0,
            Comments = request.Comments,
            Projects = [],
            RegisteredLocation = "Belarus"
        };

        SetupClients([client]);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.YouCantUpdateInactiveClient);
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContext
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
