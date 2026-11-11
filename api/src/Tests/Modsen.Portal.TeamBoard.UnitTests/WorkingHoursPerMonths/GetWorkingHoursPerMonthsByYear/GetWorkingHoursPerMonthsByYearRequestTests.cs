using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Calendar.Core.Dto;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.WorkingHoursPerMonths.GetWorkingHoursPerMonthsByYear;

public class GetWorkingHoursPerMonthsByYearRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetWorkingHoursPerMonthsByYearRequest _handler;

    public GetWorkingHoursPerMonthsByYearRequestTests()
    {
        _handler = new GetWorkingHoursPerMonthsByYearRequest(_teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenThereIsNoWorkingHoursPerMonthsEntriesShouldReturnWorkingHoursPerMonthsSheetWithEmptyLists()
    {
        // Arrange
        var query = new GetWorkingHoursPerMonthsByYearRequest.Query(2024);
        var expectedDto = new WorkingHoursPerMonthsSheetDto
        {
            HoursPerMonth = [],
            HoursPerYear = []
        };

        SetupWorkingHours([]);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task HandleWhenThereIsWorkingHoursPerMonthsForOneCountryNotForEveryMonthShouldReturnWorkingHoursPerMonthsSheetWithHoursForSpecifiedMonths()
    {
        // Arrange
        var query = new GetWorkingHoursPerMonthsByYearRequest.Query(2024);
        var expectedDto = new WorkingHoursPerMonthsSheetDto
        {
            HoursPerMonth =
            [
                new WorkingHoursPerMonthDto {Id = 1, Year = 2024, MonthOfYear = 1, CountryId = 1, Hours = 21},
                new WorkingHoursPerMonthDto {Id = 3, Year = 2024, MonthOfYear = 2, CountryId = 1, Hours = 43},
                new WorkingHoursPerMonthDto {Id = 4, Year = 2024, MonthOfYear = 3, CountryId = 1, Hours = 54},
                new WorkingHoursPerMonthDto {Id = 2, Year = 2024, MonthOfYear = 5, CountryId = 1, Hours = 32},
                new WorkingHoursPerMonthDto {Id = 5, Year = 2024, MonthOfYear = 9, CountryId = 1, Hours = 65}
            ],
            HoursPerYear = [new WorkingHoursTotalDto { Year = 2024, CountryId = 1, Hours = 215 }]
        };

        var workingHours = new List<WorkingHoursPerMonth>
        {
            new() {Id = 1, Year = 2024, MonthOfYear = 1, CountryId = 1, Hours = 21},
            new() {Id = 3, Year = 2024, MonthOfYear = 2, CountryId = 1, Hours = 43},
            new() {Id = 4, Year = 2024, MonthOfYear = 3, CountryId = 1, Hours = 54},
            new() {Id = 2, Year = 2024, MonthOfYear = 5, CountryId = 1, Hours = 32},
            new() {Id = 5, Year = 2024, MonthOfYear = 9, CountryId = 1, Hours = 65}
        };

        SetupWorkingHours(workingHours);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task HandleWhenThereIsWorkingHoursPerMonthsForMultipleCountryNotForEveryMonthShouldReturnWorkingHoursPerMonthsSheetWithHoursForSpecifiedMonthsAndCountries()
    {
        // Arrange
        var query = new GetWorkingHoursPerMonthsByYearRequest.Query(2024);
        var expectedDto = new WorkingHoursPerMonthsSheetDto
        {
            HoursPerMonth =
            [
                new WorkingHoursPerMonthDto {Id = 1, Year = 2024, MonthOfYear = 1, CountryId = 1, Hours = 21},
                new WorkingHoursPerMonthDto {Id = 3, Year = 2024, MonthOfYear = 2, CountryId = 1, Hours = 43},
                new WorkingHoursPerMonthDto {Id = 4, Year = 2024, MonthOfYear = 3, CountryId = 1, Hours = 54},
                new WorkingHoursPerMonthDto {Id = 2, Year = 2024, MonthOfYear = 5, CountryId = 1, Hours = 32},
                new WorkingHoursPerMonthDto {Id = 5, Year = 2024, MonthOfYear = 9, CountryId = 1, Hours = 65},

                new WorkingHoursPerMonthDto {Id = 6, Year = 2024, MonthOfYear = 1, CountryId = 2, Hours = 25},
                new WorkingHoursPerMonthDto {Id = 7, Year = 2024, MonthOfYear = 6, CountryId = 2, Hours = 46},
                new WorkingHoursPerMonthDto {Id = 8, Year = 2024, MonthOfYear = 7, CountryId = 2, Hours = 57},
                new WorkingHoursPerMonthDto {Id = 9, Year = 2024, MonthOfYear = 8, CountryId = 2, Hours = 38},
                new WorkingHoursPerMonthDto {Id = 10, Year = 2024, MonthOfYear = 9, CountryId = 2, Hours = 69}
            ],
            HoursPerYear =
            [
                new WorkingHoursTotalDto { Year = 2024, CountryId = 1, Hours = 215 },
                new WorkingHoursTotalDto { Year = 2024, CountryId = 2, Hours = 235 }
            ]

        };

        var workingHours = new List<WorkingHoursPerMonth>
        {
            new() {Id = 1, Year = 2024, MonthOfYear = 1, CountryId = 1, Hours = 21},
            new() {Id = 3, Year = 2024, MonthOfYear = 2, CountryId = 1, Hours = 43},
            new() {Id = 4, Year = 2024, MonthOfYear = 3, CountryId = 1, Hours = 54},
            new() {Id = 2, Year = 2024, MonthOfYear = 5, CountryId = 1, Hours = 32},
            new() {Id = 5, Year = 2024, MonthOfYear = 9, CountryId = 1, Hours = 65},

            new() {Id = 6, Year = 2024, MonthOfYear = 1, CountryId = 2, Hours = 25},
            new() {Id = 7, Year = 2024, MonthOfYear = 6, CountryId = 2, Hours = 46},
            new() {Id = 8, Year = 2024, MonthOfYear = 7, CountryId = 2, Hours = 57},
            new() {Id = 9, Year = 2024, MonthOfYear = 8, CountryId = 2, Hours = 38},
            new() {Id = 10, Year = 2024, MonthOfYear = 9, CountryId = 2, Hours = 69}
        };

        SetupWorkingHours(workingHours);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    private void SetupWorkingHours(List<WorkingHoursPerMonth> workingHours)
    {
        _teamBoardDbContextMock
            .Setup(mock => mock.WorkingHoursPerMonths)
            .Returns(workingHours.BuildMockDbSet().Object);
    }
}
