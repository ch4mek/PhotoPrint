using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Composers
{
    public class UserAgreementTransactionComposerTests
    {
        private readonly Mock<IInquiriesDbContext> _dbContextMock;
        private readonly Mock<IDisposeUserAgreementTransactionComposer> _disposeComposerMock;
        private readonly Mock<IAccrualUserAgreementTransactionComposer> _accrualComposerMock;
        private readonly UserAgreementTransactionComposer _transactionComposer;

        public UserAgreementTransactionComposerTests()
        {
            _dbContextMock = new Mock<IInquiriesDbContext>();
            _disposeComposerMock = new Mock<IDisposeUserAgreementTransactionComposer>();
            _accrualComposerMock = new Mock<IAccrualUserAgreementTransactionComposer>();

            _transactionComposer = new UserAgreementTransactionComposer(
                _dbContextMock.Object,
                _disposeComposerMock.Object,
                _accrualComposerMock.Object);
        }

        [Fact]
        public async Task ComposeWhenInquiryTemplateNotFoundShouldThrowEntityNotFoundException()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                TransactionDate = DateTime.Now,
                IsTransactionForYear = true
            };

            var userAgreements = new List<UserAgreementEntity>
            {
                new UserAgreementEntity
                {
                    UserId = 1,
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = 1 } }
            };

            _dbContextMock
                .Setup(db => db.UserAgreements)
                .Returns(userAgreements.BuildMockDbSet().Object);
            _dbContextMock
                .Setup(db => db.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity>().BuildMockDbSet().Object);

            // Act &
            var result = () => _transactionComposer.Compose(transactionInfo, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(result);
        }

        [Fact]
        public async Task ComposeWhenAccrualTypeOnceInYearShouldCallDisposeAndAccrualComposer()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                TransactionDate = DateTime.Now,
                IsTransactionForYear = true
            };

            var userAgreements = new List<UserAgreementEntity>
            {
                new()
                {
                    UserId = 1,
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = 1 },
                    DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                    DateTo = null
                }
            };

            var inquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                AccrualType = AccrualType.OnceInYear,
                IsStaked = false,
                Name = "Test",
            };

            _dbContextMock
                .Setup(db => db.UserAgreements)
                .Returns(userAgreements.BuildMockDbSet().Object);
            _dbContextMock
                .Setup(db => db.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity> { inquiryTemplate }.BuildMockDbSet().Object);

            _disposeComposerMock
                .Setup(c => c.Compose(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>(), false, null))
                .Returns(new List<UserAgreementTransactionEntity> { new() }.ToAsyncEnumerable());

            _accrualComposerMock
                .Setup(c => c.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>(), It.IsAny<CancellationToken>(), false))
                .Returns(new List<UserAgreementTransactionEntity> { new() }.ToAsyncEnumerable());

            // Act
            var result = await _transactionComposer.Compose(transactionInfo, CancellationToken.None);

            // Assert
            result.Count.Should().Be(2);
            _disposeComposerMock.Verify(c => c.Compose(It.IsAny<int>(), transactionInfo.UserId, transactionInfo.TransactionDate, It.IsAny<CancellationToken>(), false, null), Times.Once);
            _accrualComposerMock.Verify(c => c.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>(), It.IsAny<CancellationToken>(), false), Times.Once);
        }

        [Fact]
        public async Task Compose_WhenAccrualTypeProportianallyEachMonth_ShouldCallAccrualComposer()
        {
            // Arrange
            var transactionInfo = new UserAgreementTransactionInfoCoreDto
            {
                UserId = 1,
                TransactionDate = DateTime.Now,
                IsTransactionForYear = false
            };

            var userAgreements = new List<UserAgreementEntity>
            {
                new UserAgreementEntity
                {
                    UserId = 1,
                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { InquiryTemplateId = 1 },
                    DateFrom = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1)),
                    DateTo = null
                }
            };

            var inquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                AccrualType = AccrualType.ProportianallyEachMonth,
                Name = "Test",
            };

            _dbContextMock
                .Setup(db => db.UserAgreements)
                .Returns(userAgreements.BuildMockDbSet().Object);
            _dbContextMock
                .Setup(db => db.InquiryTemplates)
                .Returns(new List<InquiryTemplateEntity> { inquiryTemplate }.BuildMockDbSet().Object);

            _accrualComposerMock
                .Setup(c => c.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>(), It.IsAny<CancellationToken>(), false))
                .Returns(new List<UserAgreementTransactionEntity> { new () }.ToAsyncEnumerable());

            // Act
            var result = await _transactionComposer.Compose(transactionInfo, CancellationToken.None);

            // Assert
            result.Count.Should().Be(1);    
            _accrualComposerMock.Verify(c =>
                c.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<UserAgreementTransactionInfoCoreDto>(), It.IsAny<CancellationToken>(), false),
                Times.Once);
        }
    }
}
