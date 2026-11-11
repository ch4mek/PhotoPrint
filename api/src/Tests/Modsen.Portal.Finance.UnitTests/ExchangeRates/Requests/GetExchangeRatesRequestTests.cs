using AwesomeAssertions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExchangeRates.Requests;

public sealed class GetExchangeRatesRequestTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetExchangeRatesRequest _getExchangeRatesRequest;

    public GetExchangeRatesRequestTests()
    {
        _getExchangeRatesRequest = new GetExchangeRatesRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenExchangeRatesNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        _dbContextMock.SetupGet(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _getExchangeRatesRequest.Handle(new GetExchangeRatesRequest.Query(DateOnly.MinValue), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage($"There are no exchange rates for year: {DateOnly.MinValue.Year} and month: {DateOnly.MinValue.Month} in db.");
    }

    [Fact]
    public async Task HandleWhenExchangeRatesFoundThenReturnInСorrectOrder()
    {
        // Arrange
        var date = new DateOnly(2026, 7, 31);

        _dbContextMock.SetupGet(x => x.ExchangeRates)
            .Returns(TestDataGenerator.GetExchangeRateEntities(date).BuildMockDbSet().Object);

        // Act
        var result = await _getExchangeRatesRequest.Handle(new GetExchangeRatesRequest.Query(date), _cancellationToken);

        // Assert
        result[0].FromCurrency.CurrencyCode.Should().Be(CurrencyCodes.USDollar);
        result[1].FromCurrency.CurrencyCode.Should().Be(CurrencyCodes.Euro);
        result[2].FromCurrency.CurrencyCode.Should().Be(CurrencyCodes.RussianRuble);
        result[3].FromCurrency.CurrencyCode.Should().Be(CurrencyCodes.PolishZloty);
    }
}
