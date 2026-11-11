using MediatR;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class CreateAgreementTransactionsRequest :
    IRequestHandler<CreateAgreementTransactionsRequest.Command, int[]>
{
    private readonly IUserAgreementTransactionComposer _userAgreementTransactionComposer;
    private readonly IUsersToGenerateTransactionsResolver _usersToGenerateTransactionsResolver;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IConfiguration _configuration;
    private readonly IConnectionFactory _connectionFactory;

    public CreateAgreementTransactionsRequest(
        IUserAgreementTransactionComposer userAgreementTransactionComposer,
        IUsersToGenerateTransactionsResolver usersToGenerateTransactionsResolver,
        IInquiriesDbContext inquiriesDbContext,
        IBackgroundJobManager backgroundJobManager,
        IConfiguration configuration,
        IConnectionFactory connectionFactory)
    {
        _userAgreementTransactionComposer = userAgreementTransactionComposer;
        _usersToGenerateTransactionsResolver = usersToGenerateTransactionsResolver;
        _inquiriesDbContext = inquiriesDbContext;
        _backgroundJobManager = backgroundJobManager;
        _configuration = configuration;
        _connectionFactory = connectionFactory;
    }

    public async Task<int[]> Handle(Command request, CancellationToken cancellationToken)
    {
        var transactionsInfos = await _usersToGenerateTransactionsResolver.ResolveAsync(
            request.TransactionTime,
            cancellationToken);

        if (request.UserId != null)
        {
            transactionsInfos = transactionsInfos.Where(x => x.UserId == request.UserId).ToList();
        }

        var queueName = _configuration["AppSettings:PeopleForceSettings:OneAgreementQueue"]!;

        foreach (var transactionInfo in transactionsInfos)
        {
            _backgroundJobManager.Schedule(
                queueName,
                () => CreateAgreementTransactionsJob(
                    transactionInfo,
                    cancellationToken));
        }

        return transactionsInfos.Select(x => x.UserId).Distinct().ToArray();
    }

    public async Task CreateAgreementTransactionsJob(
        UserAgreementTransactionInfoCoreDto transactionInfo,
        CancellationToken cancellationToken)
    {
        var transactions = await _userAgreementTransactionComposer.Compose(transactionInfo, cancellationToken);

        _inquiriesDbContext.UserAgreementTransactions.AddRange(transactions);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        await _connectionFactory.CommitCurrentTransaction(cancellationToken);
    }

    internal sealed class Command : ICommand<int[]>, ITransactionalRequest
    {
        public DateTime TransactionTime { get; set; }
        public int? UserId { get; set; }
    }
}
