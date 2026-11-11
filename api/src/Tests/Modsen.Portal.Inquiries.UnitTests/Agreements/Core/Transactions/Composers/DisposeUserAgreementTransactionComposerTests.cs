using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Composers
{
    public class DisposeUserAgreementTransactionComposerTests
    {
        private readonly Mock<IInquiriesDbContext> _mockDbContext;
        private readonly DisposeUserAgreementTransactionComposer _composer;

        public DisposeUserAgreementTransactionComposerTests()
        {
            _mockDbContext = new Mock<IInquiriesDbContext>();
            _composer = new DisposeUserAgreementTransactionComposer(_mockDbContext.Object);
        }

        [Fact]
        public async Task ComposeWhenBalanceIsNegativeShouldReturnAccrualTransaction()
        {
            // Arrange
            var inquiryTemplateId = 1;
            var userId = 1;
            var transactionDate = DateTime.Now;

            _mockDbContext.Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>
                {
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = 10
                    },
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = -15
                    }
                }.BuildMockDbSet().Object);

            // Act
            var result = _composer.Compose(inquiryTemplateId, userId, transactionDate, CancellationToken.None);
            var transactionsList = await result.ToListAsync();

            // Assert
            transactionsList.Should().NotBeEmpty();
            transactionsList.Count.Should().Be(1);
            transactionsList.First().AmountOfDays.Should().Be(5);
            transactionsList.First().TransactionType.Should().Be(UserAgreementTransactionType.Accrual);
        }

        [Fact]
        public async Task ComposeWhenBalanceIsPositiveShouldReturnDisposalTransaction()
        {
            // Arrange
            var inquiryTemplateId = 1;
            var userId = 1;
            var transactionDate = DateTime.Now;

            _mockDbContext.Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>
                {
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = 20
                    },
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = -10
                    }
                }.BuildMockDbSet().Object);

            // Act
            var result = _composer.Compose(inquiryTemplateId, userId, transactionDate, CancellationToken.None);
            var transactionsList = await result.ToListAsync();

            // Assert
            transactionsList.Should().NotBeEmpty();
            transactionsList.Count.Should().Be(1);
            transactionsList.First().AmountOfDays.Should().Be(-10);
            transactionsList.First().TransactionType.Should().Be(UserAgreementTransactionType.Disposal);
        }

        [Fact]
        public async Task ComposeWhenBalanceIsZeroShouldNotReturnAnyTransactions()
        {
            // Arrange
            var inquiryTemplateId = 1;
            var userId = 1;
            var transactionDate = DateTime.Now;

            _mockDbContext.Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>
                {
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = 15
                    },
                    new()
                    {
                        UserId = userId,
                        InquiryTemplateId = inquiryTemplateId,
                        AmountOfDays = -15
                    }
                }.BuildMockDbSet().Object);

            // Act
            var result = _composer.Compose(inquiryTemplateId, userId, transactionDate, CancellationToken.None);
            var transactionsList = await result.ToListAsync();

            // Assert
            Assert.Empty(transactionsList);
            transactionsList.Should().BeEmpty();
        }
    }
}
