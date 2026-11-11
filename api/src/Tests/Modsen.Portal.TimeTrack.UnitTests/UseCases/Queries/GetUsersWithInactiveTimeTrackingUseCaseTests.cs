using Modsen.Portal.Core.Services;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.Queries;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Queries;

public class GetUsersWithInactiveTimeTrackingUseCaseTests
{
    private readonly GetUsersWithInactiveTimeTrackingUseCase _handler;
    private readonly Mock<IGetUsersWithInactiveTimeTrackingDbQuery> _dbQuery = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUsersWithInactiveTimeTrackingUseCaseTests()
    {
        _handler = new GetUsersWithInactiveTimeTrackingUseCase(_dbQuery.Object, _clockService.Object);
    }

    [Fact]
    public async Task GetUsersWithInactiveTimeTrackingWhenRequestIsValidShouldReturnUsers()
    {
        // Arrange
        var daysBeforeCurrent = 25;
        var startDate = DateTime.UtcNow.AddDays(-daysBeforeCurrent);

        _clockService
            .Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        var dbResponse = new List<GetUsersWithInactiveTimeTrackingDbResponse>()
        {
            new(Email: "email 1", Name: "name 1"),
            new(Email: "email 2", Name: "name 2"),
            new(Email: "email 3", Name: "name 3"),
        };

        _dbQuery
            .Setup(x => x.ExecuteAsync(It.IsAny<GetUsersWithInactiveTimeTrackingDbRequest>(), _cancellationToken))
            .ReturnsAsync(dbResponse);

        var request = new GetUsersWithInactiveTimeTrackingUseCase.Query(daysBeforeCurrent);

        var expected = new GetUsersWithInactiveTimeTrackingUseCase.Response(
        [
            new(Email: "email 1", Name: "name 1"),
            new(Email: "email 2", Name: "name 2"),
            new(Email: "email 3", Name: "name 3"),
        ]);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
