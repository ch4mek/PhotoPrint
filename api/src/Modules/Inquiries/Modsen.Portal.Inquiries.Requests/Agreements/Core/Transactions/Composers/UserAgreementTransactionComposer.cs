using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core;

internal interface IUserAgreementTransactionComposer
{
    Task<IReadOnlyCollection<UserAgreementTransactionEntity>> Compose(
        UserAgreementTransactionInfoCoreDto transactionInfo,
        CancellationToken cancellationToken);
}

internal class UserAgreementTransactionComposer : IUserAgreementTransactionComposer
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IDisposeUserAgreementTransactionComposer _disposeTransactionComposer;
    private readonly IAccrualUserAgreementTransactionComposer _accrualTransactionComposer;

    public UserAgreementTransactionComposer(
        IInquiriesDbContext inquiriesDbContext,
        IDisposeUserAgreementTransactionComposer disposeTransactionComposer,
        IAccrualUserAgreementTransactionComposer accrualTransactionComposer)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _disposeTransactionComposer = disposeTransactionComposer;
        _accrualTransactionComposer = accrualTransactionComposer;
    }

    public async Task<IReadOnlyCollection<UserAgreementTransactionEntity>> Compose(
        UserAgreementTransactionInfoCoreDto transactionInfo, CancellationToken cancellationToken)
    {
        var transactions = new List<UserAgreementTransactionEntity>();

        var userAgreementsGroups = await _inquiriesDbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Include(x => x.InquiryTemplateAgreement)
                .ThenInclude(x => x!.InquiryTemplate)
            .AsNoTracking()
            .Where(x => x.UserId == transactionInfo.UserId)
            .GroupBy(x => x.InquiryTemplateAgreement!.InquiryTemplateId)
            .ToListAsync(cancellationToken);

        foreach (var userAgreementsGroup in userAgreementsGroups)
        {
            var inquiryTemplate = await GetInquiryTemplate(userAgreementsGroup.Key, cancellationToken);

            // yearly accrual
            if ((transactionInfo.IsTransactionForYear ||
                transactionInfo.ContractStartDate == DateOnly.FromDateTime(transactionInfo.TransactionDate)) &&
                inquiryTemplate.AccrualType == AccrualType.OnceInYear)
            {
                await HandleYearlyAccruals(userAgreementsGroup, transactionInfo, inquiryTemplate, transactions, cancellationToken);
            }

            // monthly accruals and accrual for last contract date
            if (inquiryTemplate.AccrualType == AccrualType.ProportianallyEachMonth)
            {
                await HandleMonthlyAccruals(userAgreementsGroup, transactionInfo, transactions, cancellationToken);
            }
        }

        return transactions.AsReadOnly();
    }

    private async Task<InquiryTemplateEntity> GetInquiryTemplate(int templateId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.InquiryTemplates
            .FirstOrDefaultAsync(x => x.Id == templateId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.InquiryTemplateNotFound);
    }

    private async Task HandleYearlyAccruals(
        IGrouping<int, UserAgreementEntity> userAgreementsGroup,
        UserAgreementTransactionInfoCoreDto transactionInfo,
        InquiryTemplateEntity inquiryTemplate,
        List<UserAgreementTransactionEntity> transactions,
        CancellationToken cancellationToken)
    {
        // reset days
        if (inquiryTemplate.IsStaked == false &&            
            transactionInfo.IsTransactionForYear) // don't reset at the first contract day
        {
            var disposeTransactionsStream = _disposeTransactionComposer.Compose(
                inquiryTemplate.Id,
                transactionInfo.UserId,
                transactionInfo.TransactionDate,
                cancellationToken);

            await foreach (var transaction in disposeTransactionsStream)
            {
                transactions.Add(transaction);
            }
        }

        var activeUserAgreement = userAgreementsGroup
            .FirstOrDefault(x => x.DateFrom <= DateOnly.FromDateTime(transactionInfo.TransactionDate) &&
                                 (x.DateTo == null || x.DateTo >= DateOnly.FromDateTime(transactionInfo.TransactionDate)));

        if (activeUserAgreement != null)
        {
            var accrualTransactionsStream = _accrualTransactionComposer.Compose(
                activeUserAgreement,
                transactionInfo,
                cancellationToken);

            await foreach (var transaction in accrualTransactionsStream)
            {
                // reset and accrual transactions should have different TransactionDate to correct display
                transaction.TransactionDate = transaction.TransactionDate.AddSeconds(10);
                transactions.Add(transaction);
            }
        }
    }

    private async Task HandleMonthlyAccruals(
        IGrouping<int, UserAgreementEntity> userAgreementsGroup,
        UserAgreementTransactionInfoCoreDto transactionInfo,
        List<UserAgreementTransactionEntity> transactions,
        CancellationToken cancellationToken)
    {
        var userAgreementsActiveLastMonth = userAgreementsGroup
            .Where(x => x.DateTo == null || x.DateTo >= DateOnly.FromDateTime(transactionInfo.TransactionDate).AddMonths(-1));

        foreach (var agreement in userAgreementsActiveLastMonth)
        {
            var accrualTransactionsStream = _accrualTransactionComposer.Compose(
                agreement,
                transactionInfo,
                cancellationToken);

            await foreach (var transaction in accrualTransactionsStream)
            {
                transactions.Add(transaction);
            }
        }
    }
}
