using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core
{
    internal interface IAccrualUserAgreementTransactionComposer
    {
        IAsyncEnumerable<UserAgreementTransactionEntity> Compose(
            UserAgreementEntity userAgreement,
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            CancellationToken cancellationToken,
            bool createIfExist = false);
    }

    internal class AccrualUserAgreementTransactionComposer : IAccrualUserAgreementTransactionComposer
    {
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IAccrualAmountOfDaysCalculator _accrualAmountOfDaysCalculator;

        public AccrualUserAgreementTransactionComposer(
            IInquiriesDbContext inquiriesDbContext,
            IAccrualAmountOfDaysCalculator accrualAmountOfDaysCalculator)
        {
            _inquiriesDbContext = inquiriesDbContext;
            _accrualAmountOfDaysCalculator = accrualAmountOfDaysCalculator;
        }

        public async IAsyncEnumerable<UserAgreementTransactionEntity> Compose(
            UserAgreementEntity userAgreement,
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            bool createIfExist = false)
        {
            var isAccrualTransactionExist = await IsAccrualTransactionExistAsync(
                userAgreement.Id,
                transactionInfoCoreDto.TransactionDate);

            if (isAccrualTransactionExist && !createIfExist)
            {
                yield break;
            }

            var days = _accrualAmountOfDaysCalculator.Calculate(userAgreement, transactionInfoCoreDto);

            if (days > 0)
            {
                yield return new UserAgreementTransactionEntity
                {
                    AmountOfDays = days,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    UserId = userAgreement.UserId,
                    TransactionDate = transactionInfoCoreDto.TransactionDate,
                    UserAgreementId = userAgreement.Id,
                    UserInquiryId = null,
                    InquiryTemplateId = userAgreement.InquiryTemplateAgreement!.InquiryTemplateId
                };
            }
        }

        private Task<bool> IsAccrualTransactionExistAsync(int agreementId, DateTime transactionDate)
        {
            return _inquiriesDbContext.UserAgreementTransactions
                .Where(transaction => transaction.UserAgreementId == agreementId)
                .Where(transaction => transaction.TransactionType == UserAgreementTransactionType.Accrual)
                .Where(transaction => !transaction.IsCreatedManually)
                .Where(transaction => DateOnly.FromDateTime(transaction.TransactionDate) == DateOnly.FromDateTime(transactionDate))
                .AnyAsync();
        }
    }
}
