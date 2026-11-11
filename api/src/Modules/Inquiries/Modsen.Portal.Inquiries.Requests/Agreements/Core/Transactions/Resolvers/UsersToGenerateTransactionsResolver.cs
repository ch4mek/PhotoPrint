using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;

internal interface IUsersToGenerateTransactionsResolver
{
    Task<List<UserAgreementTransactionInfoCoreDto>> ResolveAsync(DateTime transactionTime, CancellationToken cancellationToken);
}

internal class UsersToGenerateTransactionsResolver : IUsersToGenerateTransactionsResolver
{
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IDateCalculator _dateCalculator;

    public UsersToGenerateTransactionsResolver(
        ITeamBoardContract teamBoardContract,
        IDateCalculator dateCalculator)
    {
        _teamBoardContract = teamBoardContract;
        _dateCalculator = dateCalculator;
    }

    public async Task<List<UserAgreementTransactionInfoCoreDto>> ResolveAsync(
        DateTime transactionTime,
        CancellationToken cancellationToken)
    {
        var transactionDate = DateOnly.FromDateTime(transactionTime);

        var usersDates = await _teamBoardContract.GetUsersActiveContractDates(transactionDate, cancellationToken);

        return usersDates
            .Where(x => x.ContractStartDate.HasValue)
            .Select(userDates => new UserAgreementTransactionInfoCoreDto()
            {
                UserId = userDates.UserId,
                ContractStartDate = userDates.ContractStartDate!.Value,
                ContractEndDate = userDates.FireDate ?? userDates.ContractEndDate,
                TransactionDate = transactionTime,

                IsTransactionForMonth = _dateCalculator
                    .IsWholeMonthsPassed(userDates.ContractStartDate!.Value, transactionDate),
                IsTransactionForYear = _dateCalculator
                    .IsWholeYearsPassed(userDates.ContractStartDate!.Value, transactionDate),
                IsTransactionForIncompleteYear = _dateCalculator
                    .IsWholeYearsPassed(userDates.ContractStartDate!.Value.AddDays(-1), transactionDate)
            })
            .Where(x =>
                x.IsTransactionForMonth || x.IsTransactionForYear || x.IsTransactionForIncompleteYear ||
                x.ContractEndDate == transactionDate || x.ContractStartDate == transactionDate)
            .ToList();
    }
}

