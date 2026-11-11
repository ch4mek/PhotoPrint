using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers
{
    public class ChangedContractTransactionComposerTests
    {
        private readonly Mock<ICompensationTransactionComposer> _mockCompensationTransactionComposer;
        private readonly Mock<IClockService> _mockClockService;
        private readonly Mock<IDateCalculator> _mockDateCalculator;
        private readonly Mock<ITeamBoardContract> _mockTeamBoardContract;
        private readonly Mock<IResetUserAgreementBalanceComposer> _mockResetUserAgreementBalanceComposer;
        private readonly ChangedContractTransactionComposer _composer;

        public ChangedContractTransactionComposerTests()
        {
            _mockCompensationTransactionComposer = new Mock<ICompensationTransactionComposer>();
            _mockClockService = new Mock<IClockService>();
            _mockDateCalculator = new Mock<IDateCalculator>();
            _mockTeamBoardContract = new Mock<ITeamBoardContract>();
            _mockResetUserAgreementBalanceComposer = new Mock<IResetUserAgreementBalanceComposer>();
            _composer = new ChangedContractTransactionComposer(
                _mockCompensationTransactionComposer.Object,
                _mockClockService.Object,
                _mockDateCalculator.Object,
                _mockTeamBoardContract.Object,
                _mockResetUserAgreementBalanceComposer.Object);
        }

        [Fact]
        public async Task ComposeWithFireDateShouldSetFireDateAsContractEndDate()
        {
            // Arrange
            var userId = 1;
            var todayDate = DateOnly.FromDateTime(DateTime.Now);
            var contractFrom = todayDate.AddYears(-1);
            var contractTo = (DateOnly?)null;
            var fireDate = todayDate.AddYears(1); 

            _mockClockService.Setup(clock => clock.TodayUtc()).Returns(todayDate);
            _mockTeamBoardContract.Setup(x => x.GetUserContractsDates(1, CancellationToken.None, true))
                .ReturnsAsync([new UserContractDatesOutput
                {
                    ContractEndDate = contractTo,
                    ContractStartDate = contractFrom,
                    FireDate = fireDate,
                    HireDate = contractFrom,
                    UserId = userId,
                }]);
            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, contractFrom, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.Is<UserAgreementTransactionInfoCoreDto>(dto =>
                dto.ContractEndDate == fireDate
                && dto.ContractStartDate == contractFrom),
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                null,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ComposeWithoutFireDateShouldSetContractToAsContractEndDate()
        {
            // Arrange
            var userId = 1;
            var todayDate = DateOnly.FromDateTime(DateTime.Now);
            var contractFrom = todayDate.AddYears(-1);
            var contractTo = todayDate.AddYears(1);
            var fireDate = (DateOnly?)null;

            _mockClockService.Setup(clock => clock.TodayUtc()).Returns(todayDate);
            _mockTeamBoardContract.Setup(x => x.GetUserContractsDates(1, CancellationToken.None, true))
                .ReturnsAsync([new UserContractDatesOutput
                {
                    ContractEndDate = contractTo,
                    ContractStartDate = contractFrom,
                    FireDate = fireDate,
                    HireDate = contractFrom,
                    UserId = userId,
                }]);
            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, contractFrom, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.Is<UserAgreementTransactionInfoCoreDto>(dto =>
                dto.ContractEndDate == contractTo
                && dto.ContractStartDate == contractFrom),
                It.IsAny<string>(),
                It.IsAny<List<int>>(),
                null,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ComposeWithContractToShouldIncludeCorrectDescription()
        {
            // Arrange
            var userId = 1;
            var contractFrom = new DateOnly(2025, 1, 1);
            var contractTo = new DateOnly(2027, 1, 1);
            var fireDate = (DateOnly?)null;

            var todayDate = new DateOnly(2026, 1, 1);
            _mockClockService.Setup(clock => clock.TodayUtc()).Returns(todayDate);
            _mockTeamBoardContract.Setup(x => x.GetUserContractsDates(1, CancellationToken.None, true))
                .ReturnsAsync([new UserContractDatesOutput
                {
                    ContractEndDate = contractTo,
                    ContractStartDate = contractFrom,
                    FireDate = fireDate,
                    HireDate = contractFrom,
                    UserId = userId,
                }]);
            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, contractFrom, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.IsAny<UserAgreementTransactionInfoCoreDto>(),
                "Correction due to change of contract (New contract date 01/01/2025-01/01/2027)",
                It.IsAny<List<int>>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ComposeWithoutContractToShouldIncludeCorrectionDescription()
        {
            // Arrange
            var userId = 1;
            var contractFrom = new DateOnly(2025, 1, 1);
            var contractTo = (DateOnly?)null;
            var fireDate = (DateOnly?)null;

            var todayDate = new DateOnly(2026, 1, 1);
            _mockClockService.Setup(clock => clock.TodayUtc()).Returns(todayDate);
            _mockTeamBoardContract.Setup(x => x.GetUserContractsDates(1, CancellationToken.None, true))
                .ReturnsAsync([new UserContractDatesOutput
                {
                    ContractEndDate = contractTo,
                    ContractStartDate = contractFrom,
                    FireDate = fireDate,
                    HireDate = contractFrom,
                    UserId = userId,
                }]);
            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, contractFrom, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.IsAny<UserAgreementTransactionInfoCoreDto>(),
                "Correction due to change of contract (New contract date 01/01/2025)",
                It.IsAny<List<int>>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
