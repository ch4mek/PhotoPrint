using Hangfire;
using MediatR;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

namespace Modsen.Portal.Finance.Controllers.Jobs;

public sealed class UpdateCurrentExchangeRatesJobScheduler : IRecurringJobScheduler
{
    private const string JobId = "UpdateCurrentExchangeRates";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ISender _sender;

    public UpdateCurrentExchangeRatesJobScheduler(IBackgroundJobManager backgroundJobManager, ISender sender)
    {
        _backgroundJobManager = backgroundJobManager;
        _sender = sender;
    }

    public void Schedule()
    {
        var queueName = "default";

        _backgroundJobManager.ScheduleRecurringJob(
            JobId,
            queueName,
            methodCall: () => UpdateCurrentExchangeRates(),
            cronExpression: () => "0 1 * * *",
            TimeZoneInfo.FindSystemTimeZoneById("Belarus Standard Time"));
    }

    [AutomaticRetry(
        Attempts = 3,
        DelaysInSeconds = [3600, 3600, 3600],
        OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task UpdateCurrentExchangeRates()
    {
        await _sender.Send(new UpdateCurrentExchangeRatesRequest.Command(), CancellationToken.None);
    }
}
