using MediatR;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class CreateDisposalTransactionsRequest :
    IRequestHandler<CreateDisposalTransactionsRequest.Command, int[]>
{
    private readonly IResetUserAgreementBalanceComposer _resetUserAgreementBalanceComposer;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IConfiguration _configuration;
    private readonly IConnectionFactory _connectionFactory;
    private readonly ITeamBoardContract _teamBoardContract;

    public CreateDisposalTransactionsRequest(
        IResetUserAgreementBalanceComposer resetUserAgreementBalanceComposer,
        IBackgroundJobManager backgroundJobManager,
        IConfiguration configuration,
        IConnectionFactory connectionFactory,
        ITeamBoardContract teamBoardContract)
    {
        _resetUserAgreementBalanceComposer = resetUserAgreementBalanceComposer;
        _backgroundJobManager = backgroundJobManager;
        _configuration = configuration;
        _connectionFactory = connectionFactory;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<int[]> Handle(Command request, CancellationToken cancellationToken)
    {
        var dayAgo = DateOnly.FromDateTime(request.TransactionDate).AddDays(-1);
        var lastTransactionTime = dayAgo.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        // contracts expired a day ago
        var expiredContracts = await _teamBoardContract.GetUsersExpiredContractsDates(dayAgo, cancellationToken);

        var usersToResetIds = expiredContracts
            .Where(x => x.ContractStartDate.HasValue)
            .Select(userDates => userDates.UserId)
            .Distinct();

        if (request.UserId != null)
        {
            usersToResetIds = usersToResetIds.Where(x => x == request.UserId).ToList();
        }

        var queueName = _configuration["AppSettings:PeopleForceSettings:OneDisposeTransactionsQueue"]!;

        foreach (var userdToResetId in usersToResetIds)
        {
            _backgroundJobManager.Schedule(
                queueName,
                () => CreateDisposeTransactionsJob(
                    userdToResetId,
                    request.TransactionDate,
                    lastTransactionTime,
                    cancellationToken));
        }

        return usersToResetIds.ToArray();
    }

    public async Task CreateDisposeTransactionsJob(
        int userId, DateTime transactionDate, DateTime lastTransactionTime,
        CancellationToken cancellationToken)
    {
        await _resetUserAgreementBalanceComposer.Compose(userId, transactionDate, cancellationToken, lastTransactionTime);
        await _connectionFactory.CommitCurrentTransaction(cancellationToken);
    }

    internal sealed class Command : ICommand<int[]>, ITransactionalRequest
    {
        public DateTime TransactionDate { get; set; }
        public int? UserId { get; set; }
    }
}
