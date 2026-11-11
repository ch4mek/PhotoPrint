using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core
{
    internal interface IResetUserAgreementBalanceComposer
    {
        Task<List<UserAgreementTransactionEntity>> Compose(int userId, CancellationToken cancellationToken);
        Task<List<UserAgreementTransactionEntity>> Compose(
            int userId,
            DateTime transactionTime,
            CancellationToken cancellationToken,
            DateTime? ignoreTransactionsAfter = null);

        Task<List<UserAgreementTransactionEntity>> Compose(int[] userIds, int templateId, CancellationToken cancellationToken);
    }

    internal class ResetUserAgreementBalanceComposer : IResetUserAgreementBalanceComposer
    {
        private readonly IDisposeUserAgreementTransactionComposer _disposeTransactionComposer;
        private readonly IClockService _clockService;
        private readonly IInquiriesDbContext _inquiriesDbContext;

        public ResetUserAgreementBalanceComposer(
            IDisposeUserAgreementTransactionComposer disposeTransactionComposer,
            IClockService clockService,
            IInquiriesDbContext inquiriesDbContext)
        {
            _disposeTransactionComposer = disposeTransactionComposer;
            _clockService = clockService;
            _inquiriesDbContext = inquiriesDbContext;
        }

        public Task<List<UserAgreementTransactionEntity>> Compose(
            int userId, CancellationToken cancellationToken)
        {
            return Compose([userId], _clockService.UtcNow, null, null, cancellationToken);
        }

        public Task<List<UserAgreementTransactionEntity>> Compose(int userId, DateTime transactionTime, CancellationToken cancellationToken,
            DateTime? ignoreTransactionsAfter = null)
        {
            return Compose([userId], transactionTime, null, ignoreTransactionsAfter, cancellationToken);
        }

        public Task<List<UserAgreementTransactionEntity>> Compose(int[] userIds, int templateId, CancellationToken cancellationToken)
        {
            return Compose(userIds,  _clockService.UtcNow, templateId, null, cancellationToken);
        }

        private async Task<List<UserAgreementTransactionEntity>> Compose(
            int[] userIds,
            DateTime transactionTime,
            int? templateId,
            DateTime? ignoreTransactionsAfter,
            CancellationToken cancellationToken)
        {
            var userInquiryTemplatesQuery = _inquiriesDbContext.UserAgreements
                .Where(x => userIds.Contains(x.UserId));

            if (templateId is not null)
            {
                userInquiryTemplatesQuery = userInquiryTemplatesQuery.Where(x => x.InquiryTemplateAgreement!.InquiryTemplateId == templateId.Value);
            }

            var userInquiryTemplatesWithAgreementIds = await userInquiryTemplatesQuery
                .Select(x => new {x.UserId, x.InquiryTemplateAgreement!.InquiryTemplateId})
                .Distinct()
                .ToListAsync(cancellationToken);

            var transactions = new List<UserAgreementTransactionEntity>();

            foreach (var userInquiryTemplateId in userInquiryTemplatesWithAgreementIds)
            {
                var disposeTransactionsStream = _disposeTransactionComposer.Compose(
                    userInquiryTemplateId.InquiryTemplateId,
                    userInquiryTemplateId.UserId,
                    transactionTime,
                    cancellationToken,
                    createIfExists: true,
                    ignoreTransactionsAfter: ignoreTransactionsAfter);

                await foreach (var transaction in disposeTransactionsStream)
                {
                    transactions.Add(transaction);
                }
            }

            if (transactions.Count > 0)
            {
                _inquiriesDbContext.UserAgreementTransactions.AddRange(transactions);
                await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
            }

            return transactions;
        }
    }
}
