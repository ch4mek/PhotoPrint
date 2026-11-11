using System.Linq.Expressions;
using MediatR;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.Controllers.Jobs;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;
using Moq;

namespace Modsen.Portal.Finance.UnitTests.Jobs;

public class JobSchedulerTests
{
    private readonly Mock<IBackgroundJobManager> _backgroundJobManagerMock = new();
    private readonly Mock<ISender> _senderMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly BackFillExchangeRatesJobScheduler _schedulerBackFillExchangeRates;
    private readonly UpdateCurrentExchangeRatesJobScheduler _schedulerUpdateCurrentExchangeRates;

    public JobSchedulerTests()
    {
        _schedulerBackFillExchangeRates = new BackFillExchangeRatesJobScheduler(
            _backgroundJobManagerMock.Object,
            _senderMock.Object);

        _schedulerUpdateCurrentExchangeRates = new UpdateCurrentExchangeRatesJobScheduler(
            _backgroundJobManagerMock.Object,
            _senderMock.Object);
    }

    [Fact]
    public async Task BackFillExchangeRatesShouldSendCommand()
    {
        // Arrange
        _senderMock
            .Setup(x => x.Send(It.IsAny<BackFillExchangeRatesRequest.Command>(), _cancellationToken))
            .ReturnsAsync(Unit.Value);

        // Act
        await _schedulerBackFillExchangeRates.BackFillExchangeRates();

        // Assert
        _senderMock.Verify(
            x => x.Send(It.IsAny<BackFillExchangeRatesRequest.Command>(), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public void ScheduleBackFillExchangeRatesShouldScheduleBackFillExchangeRatesJob()
    {
        // Act
        _schedulerBackFillExchangeRates.Schedule();

        // Assert
        _backgroundJobManagerMock.Verify(
            x => x.ScheduleRecurringJob(
                "BackFillExchangeRates",
                "default",
                It.IsAny<Expression<Func<Task>>>(),
                It.IsAny<Func<string>>(),
                It.IsAny<TimeZoneInfo>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateCurrentExchangeRatesShouldSendCommand()
    {
        // Arrange
        _senderMock
            .Setup(x => x.Send(It.IsAny<UpdateCurrentExchangeRatesRequest.Command>(), _cancellationToken))
            .ReturnsAsync(Unit.Value);

        // Act
        await _schedulerUpdateCurrentExchangeRates.UpdateCurrentExchangeRates();

        // Assert
        _senderMock.Verify(
            x => x.Send(It.IsAny<UpdateCurrentExchangeRatesRequest.Command>(), _cancellationToken),
            Times.Once);
    }

    [Fact]
    public void ScheduleUpdateCurrentExchangeRatesShouldScheduleUpdateCurrentExchangeRates()
    {
        // Act
        _schedulerUpdateCurrentExchangeRates.Schedule();

        // Assert
        _backgroundJobManagerMock.Verify(
            x => x.ScheduleRecurringJob(
                "UpdateCurrentExchangeRates",
                "default",
                It.IsAny<Expression<Func<Task>>>(),
                It.IsAny<Func<string>>(),
                It.IsAny<TimeZoneInfo>()),
            Times.Once);
    }
}
