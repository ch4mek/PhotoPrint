using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Modsen.Portal.BcseApi.Contract.Models;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.ExchangeRates.Core.Composers;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.ExchangeRates.Composers;

public class ExchangeRateComposerTests
{
    private readonly Mock<IFinanceDbContext> _dbContextMock = new();
    private readonly Mock<ILogger<ExchangeRateComposer>> _loggerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ExchangeRateComposer _composer;

    public ExchangeRateComposerTests()
    {
        _composer = new ExchangeRateComposer(
            _dbContextMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ComposeWhenCurrencyIsSupportedAddExchangeRate()
    {
        // Arrange
        var date = new DateOnly(2026, 7, 28);
        var model = TestDataGenerator.GetExchangeRatesByDateModel(date);

        var addedEntities = new List<ExchangeRateEntity>();
        var dbSetMock = new Mock<DbSet<ExchangeRateEntity>>();

        _dbContextMock.Setup(x => x.ExchangeRates).Returns(dbSetMock.Object);
        dbSetMock
            .Setup(x => x.Add(It.IsAny<ExchangeRateEntity>()))
            .Callback<ExchangeRateEntity>(entity => addedEntities.Add(entity));

        // Act
        await _composer.Compose(model, CurrencyCodes.BelarusianRuble, CancellationToken.None);

        // Assert
        addedEntities.Should().HaveCount(model.ExchangeRates.Count);

        foreach (var expected in model.ExchangeRates)
        {
            addedEntities.Should().ContainSingle(actual =>
                actual.FromCurrencyId == expected.CurrencyCode &&
                actual.ToCurrencyId == CurrencyCodes.BelarusianRuble &&
                actual.CurrencyMultiplier == expected.RateCoef &&
                actual.Rate == expected.RateNB &&
                actual.ExchangeDate == model.Date);
        }

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ComposeWhenUnsupportedCurrencyShouldSkip()
    {
        // Arrange
        var dbSetMock = new Mock<DbSet<ExchangeRateEntity>>();
        _dbContextMock.Setup(x => x.ExchangeRates).Returns(dbSetMock.Object);

        var rate = TestDataGenerator.GetExchangeRateModel("UNKNOWN currency code", "UNKNOWN currency name", 1, 100);
        var model = new ExchangeRatesByDateModel
        {
            Date = new DateOnly(2026, 7, 28),
            ExchangeRates = [ rate ]
        };

        // Act
        await _composer.Compose(model, CurrencyCodes.BelarusianRuble, CancellationToken.None);

        // Assert
        dbSetMock.Verify(x => x.Add(It.IsAny<ExchangeRateEntity>()),Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(rate.CurrencyCode)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ComposeWhenSaveChangesIsTrueShouldSaveChanges()
    {
        // Arrange   
        _dbContextMock.SetupGet(x => x.ExchangeRates)
            .Returns(new List<ExchangeRateEntity>().BuildMockDbSet().Object);

        var model = TestDataGenerator.GetExchangeRatesByDateModel(new DateOnly(2026, 7, 28));

        // Act
        await _composer.Compose(model, CurrencyCodes.BelarusianRuble, _cancellationToken, saveChanges: true);

        // Assert
        _dbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once);
    }
}

