using MediatR;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;

namespace Modsen.Portal.Inquiries.Controllers.Jobs;

internal sealed class CreateAgreementTransactionsJobScheduler : IRecurringJobScheduler
{
    private const string s_jobId = "CreateAgreementTransactionsJob";

    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IClockService _clockService;
    private readonly IConfiguration _configuration;
    private readonly ISender _sender;

    public CreateAgreementTransactionsJobScheduler(
        IBackgroundJobManager backgroundJobManager,
        IClockService clockService,
        IConfiguration configuration,
        ISender sender)
    {
        _backgroundJobManager = backgroundJobManager;
        _clockService = clockService;
        _configuration = configuration;
        _sender = sender;
    }

    public void Schedule()
    {
        var queueName = _configuration["AppSettings:PeopleForceSettings:AgreementsQueue"]!;

        _backgroundJobManager.ScheduleRecurringJob(
            s_jobId,
            queueName,
            methodCall: () => CreateAgreementTransactions(),
            cronExpression: () => "30 0 * * *");
    }

    public async Task CreateAgreementTransactions()
    {
        await _sender.Send(new CreateAgreementTransactionsRequest.Command
        {
            TransactionTime = _clockService.UtcNow,
        });
    }
}
