using MediatR;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

namespace Modsen.Portal.Finance.Controllers.Jobs;

public sealed class BackFillExchangeRatesJobScheduler : IRecurringJobScheduler
{
    private const string JobId = "BackFillExchangeRates";
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly ISender _sender;

    public BackFillExchangeRatesJobScheduler(IBackgroundJobManager backgroundJobManager, ISender sender)
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
            methodCall: () => BackFillExchangeRates(),
            cronExpression: () => "0 0 30 2 *"); // can only be started by admin
    }

    public async Task BackFillExchangeRates()
    {
        await _sender.Send(new BackFillExchangeRatesRequest.Command(), CancellationToken.None);
    }
}
