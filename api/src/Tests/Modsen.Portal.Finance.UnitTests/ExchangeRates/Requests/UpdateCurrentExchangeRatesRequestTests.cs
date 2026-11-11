using AwesomeAssertions;
using MediatR;
using MockQueryable.Moq;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExchangeRates.Requests;

public sealed class UpdateCurrentExchangeRatesRequestTests
{
    private readonly Mock<IBcseApiClient> _bcseApiClientMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<IExchangeRateComposer> _composerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateCurrentExchangeRatesRequest _handler;

    public UpdateCurrentExchangeRatesRequestTests()
    {
        _handler = new UpdateCurrentExchangeRatesRequest(
            _bcseApiClientMock.Object,
            _dbContextMock.Object,
            _composerMock.Object);
    }

    [Fact]
    public async Task HandleWhenNoExchangeRatesFromApiShouldThrowException()
    {
        // Arrange
        var emptyResponse = new ExchangeRatesByDateModel
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ExchangeRates = []
        };

        _bcseApiClientMock.Setup(x => x.GetCurrentExchangeRates(_cancellationToken))
            .ReturnsAsync(emptyResponse);

        // Act
        var action = () => _handler.Handle(new UpdateCurrentExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<AppException>()
            .WithMessage("There are no currency exchange rates for the current date.");

        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenNoExchangeRatesInDbShouldSave()
    {
        // Arrange
        var currentDate = new DateOnly(2026, 1, 1);
        var exchangeRatesFromApi = TestDataGenerator.GetExchangeRatesByDateModel(currentDate);

        _bcseApiClientMock
            .Setup(x => x.GetCurrentExchangeRates(_cancellationToken))
            .ReturnsAsync(exchangeRatesFromApi);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new UpdateCurrentExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(exchangeRatesFromApi, CurrencyCodes.BelarusianRuble, _cancellationToken, true),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenLatestDateEqualsCurrentDateShouldReturnWithoutSaving()
    {
        // Arrange
        var currentDate = DateOnly.MaxValue;
        var exchangeRatesFromApi = TestDataGenerator.GetExchangeRatesByDateModel(currentDate);

        _bcseApiClientMock
            .Setup(x => x.GetCurrentExchangeRates(_cancellationToken))
            .ReturnsAsync(exchangeRatesFromApi);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(TestDataGenerator.GetExchangeRateEntities(currentDate).BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new UpdateCurrentExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenLatestDateIsLastDayOfMonthShould()
    {
        // Arrange
        var currentDate = new DateOnly(2026, 2, 1);
        var latestDate = new DateOnly(2026, 1, 31);
        var exchangeRatesFromApi = TestDataGenerator.GetExchangeRatesByDateModel(currentDate);

        _bcseApiClientMock
            .Setup(x => x.GetCurrentExchangeRates(_cancellationToken))
            .ReturnsAsync(exchangeRatesFromApi);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(TestDataGenerator.GetExchangeRateEntities(latestDate).BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new UpdateCurrentExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(exchangeRatesFromApi, CurrencyCodes.BelarusianRuble, _cancellationToken, false),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenLatestDateIsNotLastDayOfMonthShouldUpdateExistingExchangeRates()
    {
        // Arrange
        var currentDate = new DateOnly(2026, 1, 16);
        var latestDate = new DateOnly(2026, 1, 15);
        var exchangeRatesFromApi = TestDataGenerator.GetExchangeRatesByDateModel(currentDate);

        var existingExchangeRates = TestDataGenerator.GetExchangeRateEntities(latestDate);

        _bcseApiClientMock
            .Setup(x => x.GetCurrentExchangeRates(_cancellationToken))
            .ReturnsAsync(exchangeRatesFromApi);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(existingExchangeRates.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new UpdateCurrentExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
        _dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}
