using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Requests;
    public class UpdateAgreementTemplateRequestTests
    {
        private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new ();
        private readonly UpdateAgreementTemplateRequest _handler;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        public UpdateAgreementTemplateRequestTests()
        {
            _handler = new UpdateAgreementTemplateRequest(
                _inquiriesDbContextMock.Object);
        }

        [Fact]
        public async Task HandleWhenUpdateNumberOfDaysOfAccrualAndAgreementHasTransactionsThrowsNotAllowedException()
        {
        // Arrange
        var templateAgreement = new InquiryTemplateAgreementEntity()
        {
            Id = 10,
            Name = "Test agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            Type = 0,
            InquiryTemplateId = 7,
        };

        _inquiriesDbContextMock
            .Setup(context => context.InquiryTemplateAgreements)
            .Returns(new List<InquiryTemplateAgreementEntity>
            {
                templateAgreement
            }.BuildMockDbSet().Object);

        var userInquiry = new UserInquiryEntity
        {
            Id = 1,
            AgreementTemplateId = 10,
            InquiryTemplateId = 7
        };

        _inquiriesDbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>
            {
                userInquiry
            }.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        // Act
        var action = () =>  _handler.Handle(new UpdateAgreementTemplateRequest.Command(){
            Id = 10,
            Name = "Test agreement",
            DaysPerContractYear = 20,
            IsDefault = true,
        }, _cancellationToken);

            // Assert
           await action.Should().ThrowAsync<ActionNotAllowedException>();
        }

        [Fact]
        public async Task HandleWhenUpdateDaysPerContractYearAndAgreementHasUserAgreementTransactionsThrowsNotAllowedException()
        {
            // Arrange
            var templateAgreement = new InquiryTemplateAgreementEntity()
            {
                Id = 10,
                Name = "Test agreement",
                DaysPerContractYear = 25,
                IsDefault = true,
                Type = 0,
                InquiryTemplateId = 7,
            };

            _inquiriesDbContextMock
                .Setup(context => context.InquiryTemplateAgreements)
                .Returns(new List<InquiryTemplateAgreementEntity>
                {
                    templateAgreement
                }.BuildMockDbSet().Object);

            _inquiriesDbContextMock
                .Setup(context => context.UserInquiries)
                .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);

            var transaction = new UserAgreementTransactionEntity
            {
                Id = 1,
                InquiryTemplateId = 7,
                UserId = 123,
                TransactionDate = DateTime.UtcNow,
                TransactionType = 0,
                AmountOfDays = 1,
                UserAgreementId = null,
                UserInquiryId = null,
                IsCreatedManually = false
            };

            _inquiriesDbContextMock
                .Setup(context => context.UserAgreementTransactions)
                .Returns(new List<UserAgreementTransactionEntity>
                {
                    transaction
                }.BuildMockDbSet().Object);

            // Act
            var action = () => _handler.Handle(new UpdateAgreementTemplateRequest.Command()
            {
                Id = 10,
                Name = "Test agreement",
                DaysPerContractYear = 20,
                IsDefault = true,
            }, _cancellationToken);

            // Assert
            await action.Should().ThrowAsync<ActionNotAllowedException>();
        }
    }
