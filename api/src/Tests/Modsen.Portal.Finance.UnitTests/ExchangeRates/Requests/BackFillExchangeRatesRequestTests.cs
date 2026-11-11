using AwesomeAssertions;
using MediatR;
using MockQueryable.Moq;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExchangeRates.Requests;

public sealed class BackFillExchangeRatesRequestTests
{
    private readonly Mock<IBcseApiClient> _bcseApiClientMock = new();
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<IExchangeRateComposer> _composerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly BackFillExchangeRatesRequest _handler;

    public BackFillExchangeRatesRequestTests()
    {
        _handler = new BackFillExchangeRatesRequest(
            _bcseApiClientMock.Object,
            _dbContextMock.Object,
            _composerMock.Object);
    }

    [Fact]
    public async Task HandleWhenNoExchangeRatesFromApiShouldReturnWithoutSaving()
    {
        // Arrange
        _bcseApiClientMock
            .Setup(x => x.GetExchangeRatesByDates(It.IsAny<IReadOnlyCollection<DateOnly>>(), _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new BackFillExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleWhenNoExchangeRatesInDbShouldSave()
    {
        // Arrange
        _bcseApiClientMock
            .Setup(x => x.GetExchangeRatesByDates(It.IsAny<IReadOnlyCollection<DateOnly>>(), _cancellationToken))
            .ReturnsAsync([TestDataGenerator.GetExchangeRatesByDateModel(DateOnly.MaxValue)]);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new BackFillExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenExchangeRatesAlreadyExistReturnWithoutSaving()
    {
        // Arrange
        _bcseApiClientMock
            .Setup(x => x.GetExchangeRatesByDates(It.IsAny<IReadOnlyCollection<DateOnly>>(), _cancellationToken))
            .ReturnsAsync([TestDataGenerator.GetExchangeRatesByDateModel(new DateOnly(2026, 2, 5))]);

        _dbContextMock.Setup(x => x.ExchangeRates)
            .Returns(TestDataGenerator.GetExchangeRateEntities(new DateOnly(2026, 2, 20)).BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(new BackFillExchangeRatesRequest.Command(), _cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _composerMock.Verify(
            x => x.Compose(It.IsAny<ExchangeRatesByDateModel>(), It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()),
            Times.Never);
    }
}
