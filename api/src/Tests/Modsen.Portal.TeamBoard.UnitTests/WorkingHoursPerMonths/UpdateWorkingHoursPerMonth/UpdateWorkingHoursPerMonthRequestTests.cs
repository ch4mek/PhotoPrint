using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.WorkingHoursPerMonths.UpdateWorkingHoursPerMonth;

public class UpdateWorkingHoursPerMonthRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateWorkingHoursPerMonthRequest _handler;

    public UpdateWorkingHoursPerMonthRequestTests()
    {
        _handler = new UpdateWorkingHoursPerMonthRequest(_teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenWorkingHoursPerMonthDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new UpdateWorkingHoursPerMonthRequest.Command(Id: 1, Hours: 20);

        SetupWorkingHours([]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldUpdateWorkingHoursPerMonth()
    {
        // Arrange
        var command = new UpdateWorkingHoursPerMonthRequest.Command(Id: 1, Hours: 20);
        var dto = new WorkingHoursPerMonthDto
        {
            Id = 1,
            Year = 2023,
            MonthOfYear = 2,
            CountryId = 2,
            Hours = 20
        };

        var workingHours = new List<WorkingHoursPerMonth>
        {
            new()
            {
                Id = 1,
                Year = 2023,
                MonthOfYear = 2,
                CountryId = 2,
                Hours = 20
            }
        };

        SetupWorkingHours(workingHours);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(dto);
    }

    private void SetupWorkingHours(List<WorkingHoursPerMonth> workingHours)
    {
        _teamBoardDbContextMock
            .Setup(mock => mock.WorkingHoursPerMonths)
            .Returns(workingHours.BuildMockDbSet().Object);
    }
}
