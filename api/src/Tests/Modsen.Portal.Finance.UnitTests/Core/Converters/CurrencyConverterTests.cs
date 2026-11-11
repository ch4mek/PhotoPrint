using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Core.Converters;

public sealed class CurrencyConverterTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CurrencyConverter _converter;

    public CurrencyConverterTests()
    {
        _converter = new CurrencyConverter(_dbContextMock.Object);
    }

    [Fact]
    public async Task ConvertSingle_WhenExchangeRateExists_ShouldReturnCalculatedResult()
    {
        // Arrange
        var request = new CurrencyConverter.Request
        {
            FromCurrencyCode = CurrencyCodes.USDollar,
            FromCurrencyCount = 100,
            ToCurrencyCode = CurrencyCodes.RussianRuble,
            Year = 2026,
            Month = 7
        };

        var exchangeRates = new List<ExchangeRateEntity>
        {
            new()
            {
                Id = 1,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.RussianRuble,
                ExchangeDate = new DateOnly(2026, 7, 15),
                Rate = 90.5m,
                CurrencyMultiplier = 10
            }
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(exchangeRates.BuildMockDbSet().Object);

        // Act
        var result = await _converter.Convert(request, _cancellationToken);

        // Assert
        result.Result.Should().Be(905m);
    }

    [Fact]
    public async Task ConvertSingleWhenExchangeRateDoesNotExistShouldThrowAppException()
    {
        // Arrange
        var request = new CurrencyConverter.Request
        {
            FromCurrencyCode = CurrencyCodes.USDollar,
            FromCurrencyCount = 100,
            ToCurrencyCode = CurrencyCodes.Euro,
            Year = 2026,
            Month = 7
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        // Act
        var act = () => _converter.Convert(request, _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage($"Error during currency conversion. There is no {CurrencyCodes.USDollar} to {CurrencyCodes.Euro} exchange rate.");
    }

    [Fact]
    public async Task ConvertBatchWhenRequestsEmptyShouldReturnEmptyList()
    {
        // Arrange
        var requests = new List<CurrencyConverter.Request>();

        // Act
        var result = await _converter.Convert(requests, _cancellationToken);

        // Assert
        result.Should().BeEmpty();

        _dbContextMock.Verify(x => x.ExchangeRates, Times.Never);
    }

    [Fact]
    public async Task ConvertBatchWhenAllRatesExistShouldReturnAllCalculated()
    {
        // Arrange
        var requests = new List<CurrencyConverter.Request>
        {
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 100,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = 2026,
                Month = 6
            },
            new()
            {
                FromCurrencyCode = CurrencyCodes.Euro,
                FromCurrencyCount = 50,
                ToCurrencyCode = CurrencyCodes.USDollar,
                Year = 2026,
                Month = 6
            },
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 200,
                ToCurrencyCode = CurrencyCodes.RussianRuble,
                Year = 2026,
                Month = 7
            }
        };

        var exchangeRates = new List<ExchangeRateEntity>
        {
            new()
            {
                Id = 1,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ExchangeDate = new DateOnly(2026, 6, 15),
                Rate = 3.2m,
                CurrencyMultiplier = 1
            },
            new()
            {
                Id = 2,
                FromCurrencyId = CurrencyCodes.Euro,
                ToCurrencyId = CurrencyCodes.USDollar,
                ExchangeDate = new DateOnly(2026, 6, 15),
                Rate = 1.1m,
                CurrencyMultiplier = 1
            },
            new()
            {
                Id = 3,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.RussianRuble,
                ExchangeDate = new DateOnly(2026, 7, 15),
                Rate = 90m,
                CurrencyMultiplier = 1
            }
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(exchangeRates.BuildMockDbSet().Object);

        // Act
        var result = await _converter.Convert(requests, _cancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(x => x.FromCurrencyCode == CurrencyCodes.USDollar && x.ToCurrencyCode == CurrencyCodes.BelarusianRuble && x.Result == 320m);
        result.Should().Contain(x => x.FromCurrencyCode == CurrencyCodes.Euro && x.ToCurrencyCode == CurrencyCodes.USDollar && x.Result == 55m);
        result.Should().Contain(x => x.FromCurrencyCode == CurrencyCodes.USDollar && x.ToCurrencyCode == CurrencyCodes.RussianRuble && x.Result == 18000m);
    }

    [Fact]
    public async Task ConvertBatchWhenSomeRatesMissingShouldThrowAppException()
    {
        // Arrange
        var requests = new List<CurrencyConverter.Request>
        {
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 100,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = 2026,
                Month = 6
            },
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 50,
                ToCurrencyCode = CurrencyCodes.Euro,
                Year = 2026,
                Month = 6
            }
        };

        var exchangeRates = new List<ExchangeRateEntity>
        {
            new()
            {
                Id = 1,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ExchangeDate = new DateOnly(2026, 6, 15),
                Rate = 3.2m,
                CurrencyMultiplier = 1
            }
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(exchangeRates.BuildMockDbSet().Object);

        // Act
        var act = () => _converter.Convert(requests, _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage($"Error during currency conversion. There is no {CurrencyCodes.USDollar} to {CurrencyCodes.Euro} exchange rate.");
    }

    [Fact]
    public async Task ConvertBatchWhenNoRatesExistShouldThrowAppException()
    {
        // Arrange
        var requests = new List<CurrencyConverter.Request>
        {
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 100,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = 2026,
                Month = 6
            }
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        // Act
        var act = () => _converter.Convert(requests, _cancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .WithMessage("Error during currency conversion. The required exchange rates do not exist.");
    }

    [Fact]
    public async Task ConvertBatchWhenDifferentMonthsShouldPickCorrectRates()
    {
        // Arrange
        var requests = new List<CurrencyConverter.Request>
        {
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 100,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = 2026,
                Month = 5
            },
            new()
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = 100,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = 2026,
                Month = 6
            }
        };

        var exchangeRates = new List<ExchangeRateEntity>
        {
            new()
            {
                Id = 1,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ExchangeDate = new DateOnly(2026, 5, 15),
                Rate = 3.0m,
                CurrencyMultiplier = 1
            },
            new()
            {
                Id = 2,
                FromCurrencyId = CurrencyCodes.USDollar,
                ToCurrencyId = CurrencyCodes.BelarusianRuble,
                ExchangeDate = new DateOnly(2026, 6, 15),
                Rate = 3.2m,
                CurrencyMultiplier = 1
            }
        };

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(exchangeRates.BuildMockDbSet().Object);

        // Act
        var result = await _converter.Convert(requests, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Month == 5 && x.Result == 300m);
        result.Should().Contain(x => x.Month == 6 && x.Result == 320m);
    }
}
