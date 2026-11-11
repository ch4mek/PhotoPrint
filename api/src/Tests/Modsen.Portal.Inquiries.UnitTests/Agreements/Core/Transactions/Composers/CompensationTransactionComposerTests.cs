using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Composers
{
    public class CompensationTransactionComposerTests
    {
        private readonly Mock<IInquiriesDbContext> _mockDbContext;
        private readonly Mock<IAccrualAmountOfDaysCalculator> _mockAccrualAmountOfDaysCalculator;
        private readonly Mock<IAccrualTransactionDatesCalculator> _mockAccrualTransactionDatesCalculator;
        private readonly CompensationTransactionComposer _composer;

        public CompensationTransactionComposerTests()
        {
            _mockDbContext = new Mock<IInquiriesDbContext>();
            _mockAccrualAmountOfDaysCalculator = new Mock<IAccrualAmountOfDaysCalculator>();
            _mockAccrualTransactionDatesCalculator = new Mock<IAccrualTransactionDatesCalculator>();
            _composer = new CompensationTransactionComposer(
                _mockDbContext.Object,
                _mockAccrualAmountOfDaysCalculator.Object,
                _mockAccrualTransactionDatesCalculator.Object);
        }

        [Fact]
        public async Task ComposeWhenNoInquiryTemplatesShouldReturnEmptyList()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                ContractStartDate = new DateOnly(2026, 1, 1),
                ContractEndDate = null,
                TransactionDate = DateTime.UtcNow
            };

            _mockDbContext
                .Setup(x => x.UserAgreements)
                .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);
            _mockDbContext
                .Setup(x => x.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            // Act
            var result = await _composer.Compose(transactionInfo, null, [], null, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ComposeWhenInquiryTemplatesExistShouldAddTransactions()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                ContractStartDate = new DateOnly(2026, 1, 1),
                ContractEndDate = null,
                TransactionDate = DateTime.UtcNow
            };

            var inquiryTemplateId = 1;

            var userAgreement = new UserAgreementEntity
            {
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplateId = inquiryTemplateId
                }
            };

            _mockDbContext
                .Setup(x => x.UserAgreements)
                .Returns(new List<UserAgreementEntity> { userAgreement }.BuildMockDbSet().Object);
            _mockDbContext
                .Setup(x => x.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            var inquiryTemplate = new InquiryTemplateEntity
            {
                Id = inquiryTemplateId,
                IsStaked = false,
                Name = "test",
                AccrualType = AccrualType.ProportianallyEachMonth
            };

            _mockDbContext
                .Setup(x => x.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity> { inquiryTemplate }.BuildMockDbSet().Object);
            _mockDbContext
                .Setup(x => x.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _mockAccrualTransactionDatesCalculator.Setup(x => x.Calculate(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<DateOnly>(),
                It.IsAny<AccrualType>()
            )).Returns(
            [
                new UserAgreementTransactionInfoCoreDto
                {
                    UserId = 1,
                    ContractStartDate = transactionInfo.ContractStartDate,
                    TransactionDate = DateTime.UtcNow,
                }
            ]);

            _mockAccrualAmountOfDaysCalculator
                .Setup(x => x.Calculate(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>()))
                .Returns(10m); 

            // Act
            var result = await _composer.Compose(transactionInfo, null, [], null, CancellationToken.None);

            // Assert
            result.Count.Should().Be(1);
            result.First().AmountOfDays.Should().Be(10m);

            _mockDbContext.Verify(x => x.UserAgreementTransactions.AddRangeAsync(It.IsAny<IEnumerable<UserAgreementTransactionEntity>>(), CancellationToken.None), Times.Once);
            _mockDbContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task ComposeWhenNewBalanceEqualsOldBalanceShouldNotAddTransaction()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                ContractStartDate = new DateOnly(2026, 1, 1),
                ContractEndDate = null,
                TransactionDate = DateTime.UtcNow
            };

            var inquiryTemplateId = 1;

            var userAgreement = new UserAgreementEntity
            {
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplateId = inquiryTemplateId
                }
            };

            _mockDbContext
                .Setup(x => x.UserAgreements)
                .Returns(new List<UserAgreementEntity> { userAgreement }.BuildMockDbSet().Object);
            _mockDbContext
                .Setup(x => x.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            var inquiryTemplate = new InquiryTemplateEntity
            {
                Id = inquiryTemplateId,
                IsStaked = false,
                Name = "Test",
                AccrualType = AccrualType.ProportianallyEachMonth
            };

            _mockDbContext
                .Setup(x => x.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity> { inquiryTemplate }.BuildMockDbSet().Object);
            _mockDbContext
                .Setup(x => x.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _mockAccrualTransactionDatesCalculator.Setup(x => x.Calculate(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly?>(),
                It.IsAny<DateOnly>(),
                It.IsAny<AccrualType>()
            )).Returns(
            [
                new UserAgreementTransactionInfoCoreDto
                {
                    UserId = 1,
                    ContractStartDate = transactionInfo.ContractStartDate,
                    TransactionDate = DateTime.UtcNow,
                }
            ]);

            _mockAccrualAmountOfDaysCalculator.Setup(x => x.Calculate(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>()))
                .Returns(0m);

            // Act
            var result = await _composer.Compose(transactionInfo, null, [], null, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
