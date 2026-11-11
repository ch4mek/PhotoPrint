using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;

internal interface IAccrualTransactionDatesCalculator
{
    IEnumerable<UserAgreementTransactionInfoCoreDto> Calculate(
        DateOnly contractStartDate,
        DateOnly? contractEndDate,
        DateOnly transactionDate,
        AccrualType accrualType);
}

internal sealed class AccrualTransactionDatesCalculator : IAccrualTransactionDatesCalculator
{
    public IEnumerable<UserAgreementTransactionInfoCoreDto> Calculate(
        DateOnly contractStartDate,
        DateOnly? contractEndDate,
        DateOnly transactionDate,
        AccrualType accrualType)
    {
        var transactionDates = new List<UserAgreementTransactionInfoCoreDto>();

        if (accrualType == AccrualType.ProportianallyEachMonth)
        {
            transactionDates.AddRange(GetDatesForMonthlyAccrual(contractStartDate, contractEndDate, transactionDate));
        }
        else
        {
            var yearsToAdd = 0;

            do
            {
                transactionDates.Add(new UserAgreementTransactionInfoCoreDto
                {
                    TransactionDate = DateTime.SpecifyKind(contractStartDate.AddYears(yearsToAdd).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                    ContractStartDate = contractStartDate,
                    ContractEndDate = contractEndDate,
                    IsTransactionForMonth = true,
                    IsTransactionForIncompleteYear = false,
                    IsTransactionForYear = true,
                });

                yearsToAdd++;

                if (contractEndDate.HasValue && contractStartDate.AddYears(yearsToAdd) > contractEndDate)
                {
                    break;
                }

            } while (contractStartDate.AddYears(yearsToAdd) <= transactionDate);
        }

        return transactionDates;
    }

    private static List<UserAgreementTransactionInfoCoreDto> GetDatesForMonthlyAccrual(
        DateOnly contractStartDate,
        DateOnly? contractEndDate,
        DateOnly transactionDate)
    {
        var monthToAdd = 0;
        DateOnly newTransactionDate = default;
        var transactionDates = new List<UserAgreementTransactionInfoCoreDto>();
        contractEndDate ??= DateOnly.MaxValue;

        while (newTransactionDate <= transactionDate && newTransactionDate <= contractEndDate)
        {
            monthToAdd++;
            var isEndOfYear = monthToAdd % 12 == 0;
            
            newTransactionDate = contractStartDate
                .AddMonths(monthToAdd)
                .AddDays(isEndOfYear ? -1 : 0);

            if (newTransactionDate > transactionDate || newTransactionDate > contractEndDate)
            {
                break;
            }

            transactionDates.Add(new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.SpecifyKind(newTransactionDate.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                ContractStartDate = contractStartDate,
                ContractEndDate = contractEndDate,
                IsTransactionForMonth = !isEndOfYear,
                IsTransactionForIncompleteYear = isEndOfYear,
                IsTransactionForYear = false,
            });
        }

        if (contractEndDate.HasValue && contractEndDate <= transactionDate)
        {
            var isContractTransactionDateExists = transactionDates
                .Any(x => DateOnly.FromDateTime(x.TransactionDate) == contractEndDate);

            if (!isContractTransactionDateExists)
            {
                transactionDates.Add(new UserAgreementTransactionInfoCoreDto
                {
                    TransactionDate = DateTime.SpecifyKind(contractEndDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                    ContractStartDate = contractStartDate,
                    ContractEndDate = contractEndDate,
                    IsTransactionForMonth = false,
                    IsTransactionForIncompleteYear = false,
                    IsTransactionForYear = false,
                });
            }
        }

        return transactionDates;
    }
}
