using AwesomeAssertions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Core.Calculators;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Managements.Core;

public sealed class ManagementsCalculatorTests
{
    private readonly Mock<ICurrencyConverter> _currencyConverterMock = new();
    private readonly Mock<ITeamBoardContract> _teamBoardContractMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ManagementsCalculator _calculator;

    public ManagementsCalculatorTests()
    {
        _calculator = new ManagementsCalculator(_currencyConverterMock.Object, _teamBoardContractMock.Object);
    }

    [Fact]
    public async Task CalculateWhenNoProjectsShouldNotThrowAndReturnZeroAggregates()
    {
        // Arrange
        var request = new ManagementsCalculator.Request
        {
            Year = 2026,
            Month = 7,
            ManagementUsers = []
        };

        // Act
        var response = await _calculator.Calculate(request, _cancellationToken);

        // Assert
        response.IncomeBynAmount.Should().BeNull();
        response.SalaryAmount.Should().BeNull();
        response.AverageKpi.Should().BeNull();
        response.Profit.Should().BeNull();
    }

    [Fact]
    public async Task CalculateWhenProjectInUsdShouldCalculateIncomeAndConvertToByn()
    {
        // Arrange
        var usdCurrency = TestDataGenerator.GetCurrencyEntity(CurrencyCodes.USDollar);

        var managements = GetManagementUserDtos(CurrencyCoreDto.Of(usdCurrency));

        _currencyConverterMock
            .Setup(x => x.Convert(It.IsAny<IReadOnlyCollection<CurrencyConverter.Request>>(), _cancellationToken))
            .ReturnsAsync(new List<CurrencyConverter.Response>
            {
                new()
                {
                    FromCurrencyCode = CurrencyCodes.USDollar,
                    FromCurrencyCount = 200,
                    ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                    Year = 2026,
                    Month = 7,
                    Result = 640m
                }
            });

        var request = new ManagementsCalculator.Request
        {
            Year = 2026,
            Month = 7,
            ManagementUsers = managements,
            Expenses = 200,
            ShouldCalculateColumnStatistics = true
        };

        // Act
        var response = await _calculator.Calculate(request, _cancellationToken);

        // Assert
        var user = managements.First();
        var project = user.Projects.First();

        project.Income.Should().Be(200m);
        project.IncomeByn.Should().Be(640m);

        user.BillableTrackedHoursAmount.Should().Be(10);
        user.HourlyRateAmount.Should().Be(20);
        user.IncomeAmount.Should().Be(200m);
        user.IncomeBynAmount.Should().Be(640m);

        response.IncomeBynAmount.Should().Be(640m);
        response.SalaryAmount.Should().Be(100);
    }

    [Fact]
    public async Task CalculateWhenProjectInBynShouldCalculateIncomeWithoutConversion()
    {
        // Arrange
        var bynCurrency = TestDataGenerator.GetCurrencyEntity(CurrencyCodes.BelarusianRuble);

        var managements = GetManagementUserDtos(CurrencyCoreDto.Of(bynCurrency));

        var request = new ManagementsCalculator.Request
        {
            Year = 2026,
            Month = 7,
            ManagementUsers = managements,
            Expenses = 200
        };

        // Act
        var response = await _calculator.Calculate(request, _cancellationToken);

        // Assert
        var user = managements.First();
        var project = user.Projects.First();

        project.Income.Should().Be(200m);
        project.IncomeByn.Should().Be(200m);

        user.IncomeAmount.Should().Be(200m);
        user.IncomeBynAmount.Should().Be(200m);

        _currencyConverterMock.Verify(x => x.Convert(It.IsAny<IReadOnlyCollection<CurrencyConverter.Request>>(), _cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CalculateWhenShouldCalculateProfitIsTrueShouldCalculateProfit()
    {
        // Arrange
        var usdCurrency = TestDataGenerator.GetCurrencyEntity(CurrencyCodes.USDollar);

        var managements = GetManagementUserDtos(CurrencyCoreDto.Of(usdCurrency));

        _currencyConverterMock
            .Setup(x => x.Convert(It.IsAny<IReadOnlyCollection<CurrencyConverter.Request>>(), _cancellationToken))
            .ReturnsAsync(new List<CurrencyConverter.Response>
            {
                new()
                {
                    FromCurrencyCode = CurrencyCodes.USDollar,
                    FromCurrencyCount = 200,
                    ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                    Year = 2026,
                    Month = 7,
                    Result = 640
                }
            });

        var request = new ManagementsCalculator.Request
        {
            Year = 2026,
            Month = 7,
            ManagementUsers = managements,
            Expenses = 200,
            ShouldCalculateProfit = true
        };

        // Act
        var response = await _calculator.Calculate(request, _cancellationToken);

        // Assert
        response.Profit.Should().Be(340m);
    }

    [Fact]
    public async Task CalculateWhenShouldCalculateStatisticsIsFalseShouldNotCalculateProfit()
    {
        // Arrange
        var usdCurrency = TestDataGenerator.GetCurrencyEntity(CurrencyCodes.USDollar);

        var managements = GetManagementUserDtos(CurrencyCoreDto.Of(usdCurrency));

        _currencyConverterMock
            .Setup(x => x.Convert(It.IsAny<IReadOnlyCollection<CurrencyConverter.Request>>(), _cancellationToken))
            .ReturnsAsync(new List<CurrencyConverter.Response>
            {
                new()
                {
                    FromCurrencyCode = CurrencyCodes.USDollar,
                    FromCurrencyCount = 200,
                    ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                    Year = 2026,
                    Month = 7,
                    Result = 640
                }
            });

        var request = new ManagementsCalculator.Request
        {
            Year = 2026,
            Month = 7,
            ManagementUsers = managements,
            Expenses = 200,
            ShouldCalculateProfit = false
        };

        // Act
        var response = await _calculator.Calculate(request, _cancellationToken);

        // Assert
        response.Profit.Should().BeNull();
    }

    private static List<ManagementUserDto> GetManagementUserDtos(CurrencyCoreDto hourlyRateCurrency)
    {
        return new List<ManagementUserDto>
        {
            new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Salary = 100,
                Projects = new List<ManagementProjectDto>
                {
                    new()
                    {
                        ProjectId = 1,
                        ProjectName = "Project 1",
                        ClientName = "Client 1",
                        HourlyRate = 20,
                        BillableTrackedHours = 10,
                        HourlyRateCurrency = hourlyRateCurrency,
                        BynCurrency = CurrencyCoreDto.Of(TestDataGenerator.GetCurrencyEntity(CurrencyCodes.BelarusianRuble))
                    }
                }
            }
        };
    }
}
