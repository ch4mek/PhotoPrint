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
    public class AccrualUserAgreementTransactionComposerTests
    {
        private readonly Mock<IInquiriesDbContext> _dbContextMock;
        private readonly Mock<IAccrualAmountOfDaysCalculator> _calculatorMock;
        private readonly AccrualUserAgreementTransactionComposer _composer;

        public AccrualUserAgreementTransactionComposerTests()
        {
            _dbContextMock = new Mock<IInquiriesDbContext>();
            _calculatorMock = new Mock<IAccrualAmountOfDaysCalculator>();
            _composer = new AccrualUserAgreementTransactionComposer(
                _dbContextMock.Object,
                _calculatorMock.Object);
        }

        [Fact]
        public async Task ComposeWhenTransactionExistsAndCreateIfExistIsFalseShouldSkipTransaction()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                Id = 1,
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplateId = 1,
                }
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now
            };

            _dbContextMock
                .Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>
                {
                    new() {
                        UserAgreementId = agreement.Id,
                        TransactionType = UserAgreementTransactionType.Accrual,
                        TransactionDate = transactionInfo.TransactionDate
                    }
                }.BuildMockDbSet().Object);

            // Act
            var result = _composer.Compose(agreement, transactionInfo, CancellationToken.None, false);

            // Assert
            var list = await result.ToListAsync();
            list.Count.Should().Be(0);
        }

        [Fact]
        public async Task ComposeWhenTransactionDoesNotExistAndDaysGreaterThanZeroShouldReturnTransaction()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                Id = 1,
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplateId = 1,
                }
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now
            };

            _dbContextMock
                .Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _calculatorMock.Setup(c => c.Calculate(agreement, transactionInfo)).Returns(5m);

            // Act
            var result = _composer.Compose(agreement, transactionInfo, CancellationToken.None, false);

            // Assert
            var list = await result.ToListAsync();
            list.Count.Should().Be(1);

            var transaction = list.First();

            transaction.Should().NotBeNull();
            transaction.AmountOfDays.Should().Be(5m);
            transaction.TransactionType.Should().Be(UserAgreementTransactionType.Accrual);
            transaction.UserId.Should().Be(agreement.UserId);
            transaction.TransactionDate.Should().Be(transactionInfo.TransactionDate);
            transaction.UserAgreementId.Should().Be(agreement.Id);           
        }

        [Fact]
        public async Task ComposeWhenTransactionDoesNotExistAndDaysZeroShouldNotReturnTransaction()
        {
            // Arrange
            var agreement = new UserAgreementEntity
            {
                Id = 1,
                UserId = 1,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    InquiryTemplateId = 1,
                }
            };

            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                TransactionDate = DateTime.Now
            };

            _dbContextMock
                .Setup(db => db.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

            _calculatorMock.Setup(c => c.Calculate(agreement, transactionInfo)).Returns(0m);

            // Act
            var result = _composer.Compose(agreement, transactionInfo, CancellationToken.None, false);

            // Assert
            var list = await result.ToListAsync();
            list.Count.Should().Be(0);
        }
    }
}
