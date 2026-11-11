using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Clients;

public class UpdateClientStatusUseCaseTests
{
    private readonly Mock<ITimeTrackDbContext> _timeTrackDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateClientStatusRequest _handler;

    public UpdateClientStatusUseCaseTests()
    {
        _handler = new UpdateClientStatusRequest(
            _timeTrackDbContextMock.Object,
            _communicationBusMock.Object);
    }

    [Fact]
    public async Task HandleWhenClientDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new UpdateClientStatusRequest.Command(1, false);

        SetupClients([]);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenClientAlreadyDeactivatedShouldThrowEntityConflictException()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            IsActive = false,
            Name = "Modsen",
            Color = "#000000",
            Comments = "Comment",
            Projects = [
                new()
                {
                    Name = "test"
                }
                ]
        };

        SetupClients([client]);

        var command = new UpdateClientStatusRequest.Command(1, false);

        // Act
        var result = () => _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task HandleWhenClientHasActiveProjectsShouldReturnDtoWithNotSuccessfulResult()
    {
        // Arrange
        var client = new Client
        {
            Id = 1,
            IsActive = true,
            Name = "Modsen",
            Color = "#000000",
            Comments = "Comment",
            Projects = [
                new() {
                    IsActive = true, Name = "test"
                }
            ]
        };

        SetupClients([client]);

        var expectedResult = new ClientStatusDto
        {
            IsSuccessful = false
        };

        var command = new UpdateClientStatusRequest.Command(client.Id, false);

        // Act
        var result = await _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task HandleWhenClientHasNoActiveProjectsShouldReturnDtoWithSuccessfulResult(int projectsCount)
    {
        // Arrange
        var isActiveValue = false;
        var client = new Client
        {
            Id = 1,
            IsActive = !isActiveValue,
            Name = "Modsen",
            Color = "#000000",
            Comments = "Comment",
            Projects = Enumerable.Range(1, projectsCount).Select(x => new Project() { Name = "test" }).ToList(),
        };

        SetupClients([client]);

        var expectedResult = new ClientStatusDto
        {
            IsSuccessful = true
        };

        var command = new UpdateClientStatusRequest.Command(client.Id, false);

        // Act
        var result = await _handler.Handle(
            command,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    private void SetupClients(List<Client> clients)
    {
        _timeTrackDbContextMock
            .Setup(mock => mock.Clients)
            .Returns(clients.BuildMockDbSet().Object);
    }
}
