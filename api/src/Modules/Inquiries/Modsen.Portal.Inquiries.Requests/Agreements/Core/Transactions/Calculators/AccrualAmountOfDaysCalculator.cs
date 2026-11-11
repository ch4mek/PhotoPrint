using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;

internal interface IAccrualAmountOfDaysCalculator
{
    decimal Calculate(UserAgreementEntity agreement, UserAgreementTransactionInfoCoreDto transactionInfo);
}

internal sealed class AccrualAmountOfDaysCalculator : IAccrualAmountOfDaysCalculator
{
    private readonly IDateCalculator _dateCalculator;

    public AccrualAmountOfDaysCalculator(IDateCalculator dateCalculator)
    {
        _dateCalculator = dateCalculator;
    }

    public decimal Calculate(UserAgreementEntity agreement, UserAgreementTransactionInfoCoreDto transactionInfo)
    {
        var inquiryAccrualType = agreement.InquiryTemplateAgreement!.InquiryTemplate!.AccrualType;
        var daysPerContractYear = agreement.InquiryTemplateAgreement.DaysPerContractYear;
        var transactionDate = DateOnly.FromDateTime(transactionInfo.TransactionDate);
        var isContractEndDate = transactionInfo.ContractEndDate == transactionDate;
        var addDayForContractEnd = isContractEndDate && transactionInfo.IsTransactionForMonth && !transactionInfo.IsTransactionForYear;

        // accrual for agreement with yearly accruals 
        if (inquiryAccrualType == AccrualType.OnceInYear)
        {            
            if ((transactionDate == transactionInfo.ContractStartDate || transactionInfo.IsTransactionForYear) &&
                agreement.DateFrom <= transactionDate &&
                (agreement.DateTo == null || agreement.DateTo >= transactionDate))
            {
                return daysPerContractYear;
            }
            else
            {
                return 0;
            }
        }

        // accrual for agreement with monthly accruals
        var monthPassed = _dateCalculator.CalculatePassedMonthsDates(
            transactionInfo.ContractStartDate,
            transactionDate)
            .Count;
        var accrualDays = GetAccrualDays(transactionInfo, daysPerContractYear, isContractEndDate, ref monthPassed);

        // 'month' here refers to a work month, not a calendar month
        var firstMonthDay = transactionInfo.ContractStartDate
            .AddMonths(monthPassed - 1);
        var lastMonthDay = transactionInfo.ContractStartDate
            .AddMonths(monthPassed)
            .AddDays(-1);

        // range of dates when agreement and contract was active within last work month
        var dateFrom = new[]
        {
            firstMonthDay,
            agreement.DateFrom,
            transactionInfo.ContractStartDate
        }.Max();

        var dateTo = new[]
        {
            lastMonthDay,
            agreement.DateTo ?? DateOnly.MaxValue,
            transactionInfo.ContractEndDate ?? DateOnly.MaxValue
        }.Min();
         
        if (dateFrom > dateTo)
        {
            return 0;
        }

        var daysInMonth = lastMonthDay.DayNumber - firstMonthDay.DayNumber + 1;

        return Math.Round(accrualDays * (dateTo.DayNumber - dateFrom.DayNumber + 1 + (addDayForContractEnd ? 1 : 0)) / daysInMonth, 2);
    }

    private static decimal GetAccrualDays(
        UserAgreementTransactionInfoCoreDto transactionInfo,
        int daysPerContractYear,
        bool isContractEndDate,
        ref int monthPassed)
    {
        decimal accrualDays = 0;

        if (transactionInfo.IsTransactionForMonth && !transactionInfo.IsTransactionForYear)
        {
            // This is because the transaction occurs one day after the last day of the work month,
            // so there is an additional month
            monthPassed--;

            accrualDays = Math.Round(daysPerContractYear / 12m, 2);
        }
        else if (transactionInfo.IsTransactionForIncompleteYear)
        {
            accrualDays = daysPerContractYear - Math.Round(daysPerContractYear / 12m, 2) * 11;
        }
        else if (isContractEndDate)
        {
            accrualDays = monthPassed % 12 == 0
                ? daysPerContractYear - Math.Round(daysPerContractYear / 12m, 2) * 11
                : Math.Round(daysPerContractYear / 12m, 2);
        }

        return accrualDays;
    }
}
