using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class UserActivityMonthsCalculatorTests
{
    private readonly UserActivityMonthsCalculator _calculator = new();

    [Fact]
    public void CalculateShouldReturnEmptyWhenNoContractsAndHireDateAfterYear()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>();
        var hireDate = new DateOnly(2026, 1, 1);
        var careerStartedAt = new DateOnly(2025, 1, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateShouldReturnAllMonthsWhenContractCoversWholeYear()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new() { ContractFrom = new DateOnly(2025, 1, 1), ContractTo = new DateOnly(2025, 12, 31) }
        };

        var hireDate = new DateOnly(2024, 6, 1);
        var careerStartedAt = new DateOnly(2024, 1, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
    }

    [Fact]
    public void CalculateShouldReturnPartialMonthsWhenContractStartsMidYear()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new() { ContractFrom = new DateOnly(2025, 4, 10), ContractTo = new DateOnly(2025, 8, 20) }
        };
        var hireDate = new DateOnly(2025, 3, 1);
        var careerStartedAt = new DateOnly(2025, 1, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([4, 5, 6, 7, 8]);
    }

    [Fact]
    public void CalculateShouldReturnMonthsWhenMultipleContracts()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new() { ContractFrom = new DateOnly(2025, 1, 1), ContractTo = new DateOnly(2025, 3, 31) },
            new() { ContractFrom = new DateOnly(2025, 6, 15), FireDate = new DateOnly(2025, 7, 10) }
        };
        var hireDate = new DateOnly(2024, 12, 1);
        var careerStartedAt = new DateOnly(2024, 12, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([1, 2, 3, 6, 7]);
    }

    [Fact]
    public void CalculateShouldTakeCareerStartedAtWhenHireDateIsEarlier()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new() { ContractFrom = new DateOnly(2025, 2, 1), ContractTo = new DateOnly(2025, 2, 28) }
        };
        var hireDate = new DateOnly(2025, 1, 1);
        var careerStartedAt = new DateOnly(2025, 2, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([2]);
    }

    [Fact]
    public void CalculateShouldReturnCorrectMonthsWhenMultipleContractsWithBreaks()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 1, 1),
                ContractTo = new DateOnly(2025, 1, 31)
            },
            new()
            {
                ContractFrom = new DateOnly(2025, 3, 1),
                ContractTo = new DateOnly(2025, 3, 31)
            },
            new()
            {
                ContractFrom = new DateOnly(2025, 4, 13),
                ContractTo = new DateOnly(2025, 4, 23),
                FireDate = new DateOnly(2025, 4, 23)
            },
            new()
            {
                ContractFrom = new DateOnly(2025, 7, 1),
                ContractTo = new DateOnly(2025, 7, 2),
                FireDate = new DateOnly(2025, 7, 2)
            },
            new()
            {
                ContractFrom = new DateOnly(2025, 7, 3),
                ContractTo = new DateOnly(2025, 7, 14),
                FireDate = new DateOnly(2025, 7, 14)
            },
            new()
            {
                ContractFrom = new DateOnly(2026, 8, 27),
                ContractTo = new DateOnly(2026, 8, 30),
                FireDate = new DateOnly(2026, 7, 29)
            },
            new()
            {
                ContractFrom = new DateOnly(2027, 8, 31),
                FireDate = new DateOnly(2027, 09, 16)
            }
        };

        var hireDate = new DateOnly(2020, 4, 1);
        var careerStartedAt = new DateOnly(2011, 06, 2);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([1, 3, 4, 7]);
    }

    [Fact]
    public void CalculateShouldReturnAllMonthsActiveWhenUserHasNoContractButCareerStartedAt()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto> { };

        var hireDate = new DateOnly(2024, 1, 1);
        var careerStartedAt = new DateOnly(2024, 2, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
    }

    [Fact]
    public void CalculateShouldReturnAllMonthsActiveWhenUserHasNoContractFromDateInCurrentYear()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto> { };

        var hireDate = new DateOnly(2025, 7, 14);
        var careerStartedAt = new DateOnly(2025, 8, 1);
        var year = 2025;

        //Act
        var result = _calculator.Calculate(userContracts, hireDate, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo([8, 9, 10, 11, 12]);
    }

    [Fact]
    public void CalculateShouldReturnNoMonthsActiveWhenUserHasContractStartedInCurrentYearAndActivityStartedAtInPrevious()
    {
        //Arrange
        var userContracts = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 8, 7),
            },
        };

        var careerStartedAt = new DateOnly(2024, 11, 2);
        var year = 2024;

        //Act
        var result = _calculator.Calculate(userContracts, null, careerStartedAt, year, null);

        //Assert
        result.Should().BeEquivalentTo(new List<int>());
    }
}
