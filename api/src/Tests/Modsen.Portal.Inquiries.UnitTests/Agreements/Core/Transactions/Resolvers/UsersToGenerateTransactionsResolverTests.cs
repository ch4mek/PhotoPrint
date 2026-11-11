using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Resolvers;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Resolvers
{
    public class UsersToGenerateTransactionsResolverTests
    {
        private readonly Mock<ITeamBoardContract> _mockTeamBoardContract;
        private readonly Mock<IDateCalculator> _mockDateCalculator;
        private readonly UsersToGenerateTransactionsResolver _resolver;

        public UsersToGenerateTransactionsResolverTests()
        {
            _mockTeamBoardContract = new Mock<ITeamBoardContract>();
            _mockDateCalculator = new Mock<IDateCalculator>();
            _resolver = new UsersToGenerateTransactionsResolver(
                _mockTeamBoardContract.Object,
                _mockDateCalculator.Object
            );
        }

        [Fact]
        public async Task ResolveAsyncNoActiveContractsShouldReturnEmptyList()
        {
            // Arrange
            var transactionDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            _mockTeamBoardContract
                .Setup(x => x.GetUsersActiveContractDates(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var result = await _resolver.ResolveAsync(transactionDate, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ResolveAsyncWithActiveContractsShouldReturnUserTransactions()
        {
            // Arrange
            var transactionDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var userContractDates = new List<UserContractDatesOutput>
            {
                new()
                {
                    UserId = 5,
                    ContractStartDate = new DateOnly(2025, 1, 1),
                    ContractEndDate = new DateOnly(2026, 12, 31),
                    FireDate = null,
                    HireDate = null,
                }
            };

            _mockTeamBoardContract.Setup(x => x.GetUsersActiveContractDates(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userContractDates);

            _mockDateCalculator.Setup(x => x.IsWholeMonthsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .Returns(true);
            _mockDateCalculator.Setup(x => x.IsWholeYearsPassed(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()))
                .Returns(false);

            // Act
            var result = await _resolver.ResolveAsync(transactionDate, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            var transaction = result.First();
            transaction.UserId.Should().Be(5);
            transaction.ContractStartDate.Should().Be(new DateOnly(2025, 1, 1));
            transaction.ContractEndDate.Should().Be(new DateOnly(2026, 12, 31));
            transaction.TransactionDate.Should().Be(transactionDate);
            transaction.IsTransactionForMonth.Should().BeTrue();
        }
    }
}
