using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class CreateClientCommandHandlerTests
{
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<IIsUserSalesManagerBusinessRule> _isUserSalesManagerBusinessRule = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateClientRequest _handler;

    public CreateClientCommandHandlerTests()
    {
        _handler = new CreateClientRequest
        (
            _timeTrackDbContextMock.Object,
            _teamBoardContractMock.Object,
            _communicationBusMock.Object,
            _isUserSalesManagerBusinessRule.Object
        );
    }

    [Fact]
    public async Task HandleWhenClientWithSameNameAlreadyExistShouldThrowEntityConflictException()
    {
        // Arrange
        const string existingClientName = "Modsen";

        var client = new Client
        {
            Id = 100,
            IsActive = true,
            Name = existingClientName,
            Color = "#0f0f0f",
            FactTrackedTime = 0,
            Comments = "asdsfgdf",
            Projects = [],
            RegisteredLocation = "Belarus"
        };

        SetupClients([client]);

        var command = new CreateClientRequest.Command(existingClientName, existingClientName, null, null, 1, 2);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityConflictException>()
            .WithMessage(PortalMessages.ClientWithTheSameNameExists);
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
