using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers
{
    public class ChangedAgreementTransactionComposerTests
    {
        private readonly Mock<ICompensationTransactionComposer> _mockCompensationTransactionComposer;
        private readonly Mock<IClockService> _mockClockService;
        private readonly Mock<IDateCalculator> _mockDateCalculator;
        private readonly Mock<ITeamBoardContract> _mockTeamBoardContract;
        private readonly Mock<IResetUserAgreementBalanceComposer> _mockResetUserAgreementBalanceComposer;
        private readonly ChangedAgreementTransactionComposer _composer;

        public ChangedAgreementTransactionComposerTests()
        {
            _mockCompensationTransactionComposer = new Mock<ICompensationTransactionComposer>();
            _mockClockService = new Mock<IClockService>();
            _mockDateCalculator = new Mock<IDateCalculator>();
            _mockTeamBoardContract = new Mock<ITeamBoardContract>();
            _mockResetUserAgreementBalanceComposer = new Mock<IResetUserAgreementBalanceComposer>();
            _composer = new ChangedAgreementTransactionComposer(
                _mockCompensationTransactionComposer.Object,
                _mockClockService.Object,
                _mockDateCalculator.Object,
                _mockTeamBoardContract.Object,
                _mockResetUserAgreementBalanceComposer.Object);
        }

        [Fact]
        public async Task ComposeWhenNoActiveContractsShouldNotCallCompensationComposer()
        {
            // Arrange
            var userId = 1;

            _mockClockService.Setup(x => x.TodayUtc()).Returns(new DateOnly(2026, 1, 1));
            _mockTeamBoardContract
                .Setup(x => x.GetUserContractsDates(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync([]);

            // Act
            await _composer.Compose(userId, null, null, [], null, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.IsAny<UserAgreementTransactionInfoCoreDto>(), It.IsAny<string>(), It.IsAny<List<int>>(), null, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ComposeWhenHasActiveContractsShouldCallCompensationComposer()
        {
            // Arrange
            var userId = 1;
            var now = DateOnly.FromDateTime(DateTime.Now);

            _mockClockService.Setup(x => x.UtcNow).Returns(DateTime.Now);
            _mockClockService.Setup(x => x.TodayUtc()).Returns(now);

            _mockTeamBoardContract
                .Setup(x => x.GetUserContractsDates(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(
                [
                    new()
                    {
                        UserId = userId,
                        HireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                        ContractStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                        ContractEndDate = null,
                        FireDate = null
                    }
                ]);

            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, null, null, [], null, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer.Verify(x => x.Compose(It.IsAny<UserAgreementTransactionInfoCoreDto>(), null, It.IsAny<List<int>>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ComposeWhenAssignmentFromProvidedShouldIncludeDescription()
        {
            // Arrange
            var userId = 1;
            var now = DateOnly.FromDateTime(DateTime.Now);

            _mockClockService.Setup(x => x.UtcNow).Returns(DateTime.Now);
            _mockClockService.Setup(x => x.TodayUtc()).Returns(now);

            _mockTeamBoardContract
                .Setup(x => x.GetUserContractsDates(userId, It.IsAny<CancellationToken>(), true))
                .ReturnsAsync(
                [
                    new()
                    {
                        UserId = userId,
                        HireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                        ContractStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-5)),
                        ContractEndDate = null,
                        FireDate = null
                    }
                ]);

            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>())).Returns(false);

            // Act
            await _composer.Compose(userId, new DateOnly(2026, 1, 1), null, [], null, CancellationToken.None);

            // Assert
            _mockCompensationTransactionComposer
                .Verify(x => x.Compose(It.IsAny<UserAgreementTransactionInfoCoreDto>(),
                "Correction due to change of assignment (Started from 01/01/2026)", It.IsAny<List<int>>(), null, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
