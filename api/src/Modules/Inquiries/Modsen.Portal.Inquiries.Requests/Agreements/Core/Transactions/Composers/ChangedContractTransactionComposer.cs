using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core
{
    internal interface IChangedContractTransactionComposer
    {
        Task Compose(
            int userId, DateOnly contractFrom,
            CancellationToken cancellationToken);
    }

    internal class ChangedContractTransactionComposer : IChangedContractTransactionComposer
    {
        private readonly ICompensationTransactionComposer _compensationTransactionComposer;
        private readonly IClockService _clockService;
        private readonly IDateCalculator _dateCalculator;
        private readonly ITeamBoardContract _teamBoardContract;
        private readonly IResetUserAgreementBalanceComposer _resetUserAgreementBalanceComposer;

        public ChangedContractTransactionComposer(
            ICompensationTransactionComposer compensationTransactionComposer,
            IClockService clockService,
            IDateCalculator dateCalculator,
            ITeamBoardContract teamBoardContract,
            IResetUserAgreementBalanceComposer resetUserAgreementBalanceComposer)
        {
            _compensationTransactionComposer = compensationTransactionComposer;
            _clockService = clockService;
            _dateCalculator = dateCalculator;
            _teamBoardContract = teamBoardContract;
            _resetUserAgreementBalanceComposer = resetUserAgreementBalanceComposer;
        }

        public async Task Compose(
            int userId, DateOnly contractFrom,
            CancellationToken cancellationToken)
        {
            var now = _clockService.UtcNow;
            var today = _clockService.TodayUtc();

            var contracts = await _teamBoardContract.GetUserContractsDates(userId, cancellationToken);

            var lastActiveContract = contracts
                .Where(x => x.ContractStartDate <= today)
                .OrderBy(x => x.ContractStartDate)
                .LastOrDefault();

            // contract not found
            if (lastActiveContract == null || !lastActiveContract.ContractStartDate.HasValue)
            {
                await _resetUserAgreementBalanceComposer.Compose(userId, cancellationToken);
                return;
            }

            var contractEndDate = lastActiveContract.FireDate ?? lastActiveContract.ContractEndDate;

            // last contract is expired
            if (contractEndDate.HasValue && contractEndDate.Value < today)
            {
                await _resetUserAgreementBalanceComposer.Compose(userId, cancellationToken);
                return;
            }

            // last contract is active
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = userId,
                ContractStartDate = lastActiveContract.ContractStartDate.Value,
                ContractEndDate = contractEndDate,
                TransactionDate = now,
                IsTransactionForMonth = _dateCalculator.IsWholeMonthsPassed(lastActiveContract.ContractStartDate.Value, today),
                IsTransactionForYear = _dateCalculator.IsWholeYearsPassed(lastActiveContract.ContractStartDate.Value, today),
                IsTransactionForIncompleteYear = _dateCalculator
                        .IsWholeYearsPassed(lastActiveContract.ContractStartDate.Value.AddDays(-1), today)
            };

            var description = lastActiveContract.ContractEndDate.HasValue
                ? $"Correction due to change of contract (New contract date {lastActiveContract.ContractStartDate:dd/MM/yyyy}-{lastActiveContract.ContractEndDate:dd/MM/yyyy})"
                : $"Correction due to change of contract (New contract date {lastActiveContract.ContractStartDate:dd/MM/yyyy})";

            await _compensationTransactionComposer.Compose(
                transactionInfo,
                description,
                [],
                null, // recalculate all inquiry templates
                cancellationToken);
        }
    }
}
