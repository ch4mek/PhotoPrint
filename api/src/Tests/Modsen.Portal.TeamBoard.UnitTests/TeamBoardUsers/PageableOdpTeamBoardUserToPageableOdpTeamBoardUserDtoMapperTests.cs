using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.UnitTests.TeamBoardUsers;

public class PageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapperTests
{
    private readonly IPageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper _mapper = new PageableOdpTeamBoardUserToPageableOdpTeamBoardUserDtoMapper();

    [Fact]
    public void MapWhenCareerStartAtDateIsNotNullShouldReturnCountOfMonthsBetweenCareerStartAtDateAndUtcNow()
    {
        // Arrange
        var startDate = new DateOnly(2023, 05, 12);
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var pageableTeamBoardUsers = new PageableListOfItems<OdpTeamBoardUser>
        {
            Items = new List<OdpTeamBoardUser>
            {
                new()
                {
                    CareerStartedAt = startDate
                }
            },
            Page = 1,
            PageSize = 5,
            Total = 1
        };

        var yearsDifference = endDate.Year - startDate.Year;
        var monthsDifference = endDate.Month - startDate.Month;
        var expectedNumberOfMonths = yearsDifference * 12 + monthsDifference;

        if (endDate.Day < startDate.Day)
        {
            expectedNumberOfMonths--;
        }

        // Act
        var result = _mapper.Map(pageableTeamBoardUsers);

        // Assert
        result.Items.First().ProfExperienceInMonths.Should().NotBe(0);
        result.Items.First().ProfExperienceInMonths.Should().Be(expectedNumberOfMonths);
    }

    [Fact]
    public void MapWhenCareerStartAtDateIsNullShouldReturnZeroMonthsBetweenCareerStartAtDateAndUtcNow()
    {
        // Arrange
        var pageableTeamBoardUsers = new PageableListOfItems<OdpTeamBoardUser>
        {
            Items = new List<OdpTeamBoardUser>
            {
                new()
                {
                    CareerStartedAt = null
                }
            },
            Page = 1,
            PageSize = 5,
            Total = 1
        };

        var expectedNumberOfMonths = 0;

        // Act
        var result = _mapper.Map(pageableTeamBoardUsers);

        // Assert
        result.Items.First().ProfExperienceInMonths.Should().Be(expectedNumberOfMonths);
    }
}
