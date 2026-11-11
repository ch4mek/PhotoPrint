using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.WorkingHoursPerMonths.CreateWorkingHoursPerMonth;

public class CreateWorkingHoursPerMonthRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly CreateWorkingHoursPerMonthRequest _handler;

    public CreateWorkingHoursPerMonthRequestTests()
    {
        _handler = new CreateWorkingHoursPerMonthRequest(_teamboardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenCountryDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var command = new CreateWorkingHoursPerMonthRequest.Command(Year: 2024, MonthOfYear: 12, CountryId: 1, Hours: 160);

        SetupCountries([]);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenWorkingHoursPerMonthWithTheSameYearAndMonthOfYearAndCountryIdShouldThrowEntityConflictException()
    {
        // Arrange
        var command = new CreateWorkingHoursPerMonthRequest.Command(Year: 2024, MonthOfYear: 12, CountryId: 1, Hours: 160);

        var countries = new List<Country>
        {
            new()
            {
                Id = 1,
                Name = "Name"
            }
        };

        SetupCountries(countries);

        var workingHours = new List<WorkingHoursPerMonth>
        {
            new()
            {
                Year = 2024,
                MonthOfYear = 12,
                CountryId = 1,
            }
        };

        SetupWorkingHours(workingHours);

        // Act
        var result = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldCreateWorkingHoursPerMonth()
    {
        // Arrange
        var command = new CreateWorkingHoursPerMonthRequest.Command(Year: 2024, MonthOfYear: 12, CountryId: 1, Hours: 160);
        var dto = new WorkingHoursPerMonthDto
        {
            Id = 0,
            Year = 2024,
            MonthOfYear = 12,
            CountryId = 1,
            Hours = 160
        };

        var countries = new List<Country>
        {
            new()
            {
                Id = 1,
                Name = "Name"
            }
        };

        SetupCountries(countries);
        SetupWorkingHours([]);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(dto);
    }

    private void SetupWorkingHours(List<WorkingHoursPerMonth> workingHours)
    {
        _teamboardDbContextMock
            .Setup(mock => mock.WorkingHoursPerMonths)
            .Returns(workingHours.BuildMockDbSet().Object);
    }

    private void SetupCountries(List<Country> countries)
    {
        _teamboardDbContextMock
            .Setup(mock => mock.Countries)
            .Returns(countries.BuildMockDbSet().Object);
    }
}
