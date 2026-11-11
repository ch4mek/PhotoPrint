using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.UnitTests.Payload.Core.Calculators;

public class TotalUserTimeByMonthsCalculatorTests
{
    private readonly WorkingDaysCalculator _workingDaysCalculator = new();
    private readonly TotalUserTimeByMonthsCalculator _totalUserTimeByMonthsCalculator;

    public TotalUserTimeByMonthsCalculatorTests()
    {
        _totalUserTimeByMonthsCalculator = new(_workingDaysCalculator);
    }

    [Fact]
    public void HandleWhenOneContractOnYearOnlyWithFromShouldReturnCorrectData()
    {
        // Assert
        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 1, 15),
                ContractTo = null,
                FireDate = null,
            }
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(contractDates, true, null, 2025, 1, 176, new DateOnly(2025, 1, 15), null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(13 * 8);
    }

    [Fact]
    public void HandleWhenOneContractOnYearWithIntervalShouldReturnCorrectData()
    {
        // Assert
        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 2, 15),
                ContractTo = new DateOnly(2025, 4, 15),
                FireDate = null,
            }
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(contractDates, true, null, 2025,  3, 170, new DateOnly(2025, 2, 15), null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(10 * 8);
        result[3].Should().Be(170);
        result[4].Should().Be(11 * 8);
        result[5].Should().Be(0);
        result[6].Should().Be(0);
        result[7].Should().Be(0);
        result[8].Should().Be(0);
        result[9].Should().Be(0);
        result[10].Should().Be(0);
        result[11].Should().Be(0);
        result[12].Should().Be(0);
    }

    [Fact]
    public void HandleWhenOneContractOnYearWithFireDateShouldReturnCorrectData()
    {
        // Assert
        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 2, 15),
                ContractTo = null,
                FireDate = new DateOnly(2025, 4, 15),
            }
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(contractDates, true, null, 2025,  3, 170, new DateOnly(2025, 2, 15), null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(10 * 8);
        result[3].Should().Be(170);
        result[4].Should().Be(11 * 8);
        result[5].Should().Be(0);
        result[6].Should().Be(0);
        result[7].Should().Be(0);
        result[8].Should().Be(0);
        result[9].Should().Be(0);
        result[10].Should().Be(0);
        result[11].Should().Be(0);
        result[12].Should().Be(0);
    }

    [Fact]
    public void HandleWhenTwoContractInOneMonthShouldReturnCorrectData()
    {
        // Assert
        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2025, 2, 15),
                ContractTo = null,
                FireDate = new DateOnly(2025, 4, 15),
            },
            new()
            {
                ContractFrom = new DateOnly(2025, 4, 23),
                ContractTo = null,
                FireDate = new DateOnly(2025, 4, 29),
            }
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(contractDates, true, null, 2025,  3, 170,new DateOnly(2025, 2, 15), null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(10 * 8);
        result[3].Should().Be(170);
        result[4].Should().Be(16 * 8);
        result[5].Should().Be(0);
        result[6].Should().Be(0);
        result[7].Should().Be(0);
        result[8].Should().Be(0);
        result[9].Should().Be(0);
        result[10].Should().Be(0);
        result[11].Should().Be(0);
        result[12].Should().Be(0);
    }

    [Fact]
    public void HandleWhenContractNotInYearShouldReturnCorrectData()
    {
        // Assert
        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2026, 2, 15),
                ContractTo = null,
                FireDate = new DateOnly(2026, 4, 15),
            },
            new()
            {
                ContractFrom = new DateOnly(2024, 2, 15),
                ContractTo = null,
                FireDate = new DateOnly(2024, 4, 15),
            },
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(contractDates, true, null, 2025, 1, 170, new DateOnly(2025, 2, 15), null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(0);
        result[3].Should().Be(0);
        result[4].Should().Be(0);
        result[5].Should().Be(0);
        result[6].Should().Be(0);
        result[7].Should().Be(0);
        result[8].Should().Be(0);
        result[9].Should().Be(0);
        result[10].Should().Be(0);
        result[11].Should().Be(0);
        result[12].Should().Be(0);
    }

    [Fact]
    public void HandleWhenUserHasNoContractsShouldCreateOne()
    {
        // Assert
        var contractDates = new List<ContractDatesDto> { };

        var request = new TotalUserTimeByMonthsCalculatorRequest(ContractDates: contractDates,
            IsUserHasContracts: false,
            HireDate: null,
            Year: 2025,
            Month: 2,
            WorkingHoursPerMonth: 170,
            CareerStartedAt: new DateOnly(2025, 2, 1),
            DepartmentRanges: null);

        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(170); // full month
    }

    [Fact]
    public void HandleWhenContractInFutureYearShouldReturnZeroForEachMonth()
    {
        // Assert

        var contractDates = new List<ContractDatesDto>
        {
            new()
            {
                ContractFrom = new DateOnly(2026, 2, 15),
                ContractTo = null,
                FireDate = null,
            },
        };

        var request = new TotalUserTimeByMonthsCalculatorRequest(ContractDates: contractDates,
            IsUserHasContracts: true,
            HireDate: null,
            Year: 2025,
            Month: 1,
            WorkingHoursPerMonth: 170,
            CareerStartedAt: new DateOnly(2025, 1, 1),
            DepartmentRanges: null);


        // Act
        var result = _totalUserTimeByMonthsCalculator.Calculate(request);

        // Arrange
        result[1].Should().Be(0);
        result[2].Should().Be(0);
        result[3].Should().Be(0);
        result[4].Should().Be(0);
        result[5].Should().Be(0);
        result[6].Should().Be(0);
        result[7].Should().Be(0);
        result[8].Should().Be(0);
        result[9].Should().Be(0);
        result[10].Should().Be(0);
        result[11].Should().Be(0);
        result[12].Should().Be(0);
    }
}
