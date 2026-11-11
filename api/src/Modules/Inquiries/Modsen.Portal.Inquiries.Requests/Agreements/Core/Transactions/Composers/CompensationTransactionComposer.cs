using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core
{
    internal interface ICompensationTransactionComposer
    {
        Task<List<UserAgreementTransactionEntity>> Compose(
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            string? description,
            List<int> currentlyUpdatedToInactiveUserAgreementIds,
            List<int>? inquiryTemplateToUpdateIds,
            CancellationToken cancellationToken);
    }

    internal class CompensationTransactionComposer : ICompensationTransactionComposer
    {
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IAccrualAmountOfDaysCalculator _accrualAmountOfDaysCalculator;
        private readonly IAccrualTransactionDatesCalculator _accrualTransactionDatesCalculator;

        public CompensationTransactionComposer(
            IInquiriesDbContext inquiriesDbContext,
            IAccrualAmountOfDaysCalculator accrualAmountOfDaysCalculator,
            IAccrualTransactionDatesCalculator accrualTransactionDatesCalculator)
        {
            _inquiriesDbContext = inquiriesDbContext;
            _accrualAmountOfDaysCalculator = accrualAmountOfDaysCalculator;
            _accrualTransactionDatesCalculator = accrualTransactionDatesCalculator;
        }

        public async Task<List<UserAgreementTransactionEntity>> Compose(
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            string? description,
            List<int> currentlyUpdatedToInactiveUserAgreementIds,
            List<int>? inquiryTemplateToUpdateIds, 
            CancellationToken cancellationToken)
        {
            var query = _inquiriesDbContext.UserAgreements
                .Where(x => x.UserId == transactionInfoCoreDto.UserId);

            query = currentlyUpdatedToInactiveUserAgreementIds.Count > 0
                ? query.Where(x => !x.IsDeleted || currentlyUpdatedToInactiveUserAgreementIds.Contains(x.Id))
                : query.Where(x => !x.IsDeleted);

            var inquiryTemplateIds = await query
                .Include(x => x.InquiryTemplateAgreement)
                .Select(x => x.InquiryTemplateAgreement!.InquiryTemplateId)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (inquiryTemplateToUpdateIds != null)
            {
                inquiryTemplateIds = inquiryTemplateIds
                    .Where(inquiryTemplateToUpdateIds.Contains)
                    .ToList();
            }

            var transactions = new List<UserAgreementTransactionEntity>();

            foreach (var inquiryTemplateId in inquiryTemplateIds)
            {
                var transaction = await GetCorrectionTransactionForInquiry(
                    inquiryTemplateId,
                    transactionInfoCoreDto,
                    description,
                    cancellationToken);

                if (transaction != null)
                {
                    transactions.Add(transaction);
                }
            }

            await _inquiriesDbContext.UserAgreementTransactions.AddRangeAsync(transactions, cancellationToken);
            await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

            return transactions;
        }

        private async Task<UserAgreementTransactionEntity?> GetCorrectionTransactionForInquiry(
            int inquiryTemplateId,
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            string? description,
            CancellationToken cancellationToken)
        {
            var inquiryTemplate = await _inquiriesDbContext.InquiryTemplates
                .Where(x => x.Id == inquiryTemplateId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.InquiryTemplateNotFound);

            var balance = await _inquiriesDbContext.UserAgreementTransactions
                .Where(x => x.UserId == transactionInfoCoreDto.UserId)
                .Where(x => x.InquiryTemplateId == inquiryTemplate.Id)
                .Where(x => !x.IsCreatedManually)

                // balance refunded due to rejection of an approved absence request
                .Where(x => !(x.TransactionType == UserAgreementTransactionType.Accrual && x.UserInquiryId != null))

                .SumAsync(x => x.AmountOfDays, cancellationToken);

            // balance that should be after recalculation and correction
            var newBalance = await GetAccrualsSum(inquiryTemplate, transactionInfoCoreDto, cancellationToken);
            newBalance += await GetUsedDays(inquiryTemplate, transactionInfoCoreDto, cancellationToken);
            
            if (newBalance - balance != 0)
            {
                return new UserAgreementTransactionEntity
                {
                    AmountOfDays = newBalance - balance,
                    UserId = transactionInfoCoreDto.UserId,
                    InquiryTemplateId = inquiryTemplateId,
                    TransactionDate = transactionInfoCoreDto.TransactionDate,
                    TransactionType = UserAgreementTransactionType.Accrual,
                    UserAgreementId = null,
                    UserInquiryId = null,
                    Desctiption = description,
                };
            }
            else
            {
                return null;
            }
        }

        private async Task<decimal> GetAccrualsSum(
            InquiryTemplateEntity inquiryTemplate,
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            CancellationToken cancellationToken)
        {
            var actualTransactionsStartDate = GetStartDateBasedOnTransferType(
                inquiryTemplate.IsStaked, transactionInfoCoreDto.ContractStartDate, DateOnly.FromDateTime(transactionInfoCoreDto.TransactionDate));

            var userAgreements = await _inquiriesDbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .Include(x => x.InquiryTemplateAgreement)
                .Where(x => x.UserId == transactionInfoCoreDto.UserId)
                .Where(x => x.InquiryTemplateAgreement!.InquiryTemplateId == inquiryTemplate.Id)
                .ToListAsync(cancellationToken);

            var newTransactionInfoDtos = _accrualTransactionDatesCalculator.Calculate(
                actualTransactionsStartDate,
                transactionInfoCoreDto.ContractEndDate,
                DateOnly.FromDateTime(transactionInfoCoreDto.TransactionDate),
                inquiryTemplate.AccrualType!.Value);

            var newBalance = 0m;

            foreach (var newTransactionInfoDto in newTransactionInfoDtos)
            {
                foreach (var userAgreement in userAgreements)
                {
                    newBalance += _accrualAmountOfDaysCalculator.Calculate(userAgreement, newTransactionInfoDto);
                }
            }

            return newBalance;
        }

        private async Task<decimal> GetUsedDays(
            InquiryTemplateEntity inquiryTemplate,
            UserAgreementTransactionInfoCoreDto transactionInfoCoreDto,
            CancellationToken cancellationToken)
        {
            var actualTransactionsStartDate = GetStartDateBasedOnTransferType(
                inquiryTemplate.IsStaked, transactionInfoCoreDto.ContractStartDate, DateOnly.FromDateTime(transactionInfoCoreDto.TransactionDate));

            return await _inquiriesDbContext.UserAgreementTransactions
                .Where(x => x.UserId == transactionInfoCoreDto.UserId)
                .Where(x => x.InquiryTemplateId == inquiryTemplate.Id)
                .Where(x => x.TransactionType == UserAgreementTransactionType.Using)
                .Where(x => DateOnly.FromDateTime(x.TransactionDate) >= actualTransactionsStartDate)
                .SumAsync(x => x.AmountOfDays, cancellationToken);
        }

        private static DateOnly GetStartDateBasedOnTransferType(bool? isStaked, DateOnly contractStartDate, DateOnly transactionDate)
        {
            var actualTransactionsStartDate = contractStartDate;

            if (isStaked == false)
            {
                var yearsToAdd = 0;

                while (actualTransactionsStartDate.AddYears(yearsToAdd + 1) <= transactionDate)
                {
                    yearsToAdd++;
                }

                actualTransactionsStartDate = actualTransactionsStartDate.AddYears(yearsToAdd);
            }

            return actualTransactionsStartDate;
        }
    }
}
