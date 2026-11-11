using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core;

internal interface IDisposeUserAgreementTransactionComposer
{
    IAsyncEnumerable<UserAgreementTransactionEntity> Compose(
        int inquiryTemplateId,
        int userId,
        DateTime transactionTime,
        CancellationToken cancellationToken,
        bool createIfExists = false,
        DateTime? ignoreTransactionsAfter = null);
}

internal class DisposeUserAgreementTransactionComposer : IDisposeUserAgreementTransactionComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public DisposeUserAgreementTransactionComposer(
        IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async IAsyncEnumerable<UserAgreementTransactionEntity> Compose(
        int inquiryTemplateId,
        int userId,
        DateTime transactionTime,
        [EnumeratorCancellation] CancellationToken cancellationToken,
        bool createIfExists = false,
        DateTime? ignoreTransactionsAfter = null)
    {
        var isDisposalTransactionExist = await IsDisposalTransactionExistAsync(
            inquiryTemplateId, userId, transactionTime);

        if (isDisposalTransactionExist && !createIfExists)
        {
            yield break;
        }

        var balance = await _inquiriesDbContext.UserAgreementTransactions
            .Where(x => x.UserId == userId)
            .Where(x => x.TransactionDate <= (ignoreTransactionsAfter ?? transactionTime))
            .Where(x => x.InquiryTemplateId == inquiryTemplateId)
            .SumAsync(x => x.AmountOfDays, cancellationToken);

        // case when balance < 0 is possible because of corrections after accruals and requests(usings)
        if (balance < 0)
        {
            yield return new UserAgreementTransactionEntity
            {
                AmountOfDays = -balance,
                TransactionType = UserAgreementTransactionType.Accrual,
                UserId = userId,
                TransactionDate = transactionTime,
                UserAgreementId = null,
                UserInquiryId = null,
                InquiryTemplateId = inquiryTemplateId
            };
        }

        if (balance > 0)
        {
            yield return new UserAgreementTransactionEntity
            {
                AmountOfDays = -balance,
                TransactionType = UserAgreementTransactionType.Disposal,
                UserId = userId,
                TransactionDate = transactionTime,
                UserAgreementId = null,
                UserInquiryId = null,
                InquiryTemplateId = inquiryTemplateId
            };
        }
    }

    private Task<bool> IsDisposalTransactionExistAsync(int inquiryTemplateId, int userId, DateTime transactionDate)
    {
        return _inquiriesDbContext.UserAgreementTransactions
            .Where(transaction => transaction.InquiryTemplateId == inquiryTemplateId)
            .Where(transaction => transaction.UserId == userId)
            .Where(transaction => transaction.TransactionType == UserAgreementTransactionType.Disposal)
            .Where(transaction => DateOnly.FromDateTime(transaction.TransactionDate) == DateOnly.FromDateTime(transactionDate))
            .AnyAsync();
    }
}
