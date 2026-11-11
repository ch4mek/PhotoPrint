//using MockQueryable.Moq;
//using Modsen.Portal.Core.Exceptions;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Transactions.Composers;
//using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
//using Modsen.Portal.TeamBoard.Contract;
//using Modsen.Portal.TeamBoard.Contract.Contract.Output;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Requests
//{
//    public class CreatePeopleForceDefaultAssignsRequestTests
//    {
//        private readonly Mock<IDefaultAgreementsComposer> _mockDefaultAgreementsComposer;
//        private readonly Mock<IDefaultInquiryTemplatesComposer> _mockDefaultInquiriesComposer;
//        private readonly Mock<IInquiriesDbContext> _mockInquiriesDbContext;
//        private readonly Mock<IDefaultUserAgreementTransactionComposer> _mockDefaultUserAgreementTransactionComposer;
//        private readonly Mock<IInquiriesUserAgreementTransactionComposer> _mockInquiriesUserAgreementTransactionComposer;
//        private readonly Mock<ICompareAgreementBalancesCalculator> _mockCompareAgreementBalancesCalculator;
//        private readonly Mock<ITeamBoardContract> _mockTeamBoardContract;
//        private readonly CreatePeopleForceInquiryTemplateAssignsRequest _handler;

//        public CreatePeopleForceDefaultAssignsRequestTests()
//        {
//            _mockDefaultAgreementsComposer = new Mock<IDefaultAgreementsComposer>();
//            _mockDefaultInquiriesComposer = new Mock<IDefaultInquiryTemplatesComposer>();
//            _mockInquiriesDbContext = new Mock<IInquiriesDbContext>();
//            _mockDefaultUserAgreementTransactionComposer = new Mock<IDefaultUserAgreementTransactionComposer>();
//            _mockInquiriesUserAgreementTransactionComposer = new Mock<IInquiriesUserAgreementTransactionComposer>();
//            _mockCompareAgreementBalancesCalculator = new Mock<ICompareAgreementBalancesCalculator>();
//            _mockTeamBoardContract = new Mock<ITeamBoardContract>();

//            _handler = new CreatePeopleForceInquiryTemplateAssignsRequest(
//                _mockDefaultAgreementsComposer.Object,
//                _mockDefaultInquiriesComposer.Object,
//                _mockInquiriesDbContext.Object,
//                _mockDefaultUserAgreementTransactionComposer.Object,
//                _mockInquiriesUserAgreementTransactionComposer.Object,
//                _mockCompareAgreementBalancesCalculator.Object,
//                _mockTeamBoardContract.Object);
//        }

//        [Fact]
//        public async Task HandleShouldThrowEntityConflictExceptionWhenNoContractsExists()
//        {
//            // Arrange
//            var userId = 1;
//            var transactionDate = DateOnly.FromDateTime(DateTime.UtcNow);
//            var command = new CreatePeopleForceInquiryTemplateAssignsRequest.Command { UserId = userId, TransactionDate = transactionDate };

//            _mockTeamBoardContract
//                .Setup(t => t.GetUserContractsDates(userId, It.IsAny<CancellationToken>()))
//                .ReturnsAsync([]);

//            // Act
//            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            await act.Should().ThrowAsync<EntityConflictException>()
//                .WithMessage($"User {userId} doesn't have a contract");
//        }

//        [Fact]
//        public async Task HandleShouldAddDefaultInquiriesToDatabaseWhenInquiriesAreReturned()
//        {
//            // Arrange
//            var userId = 1;
//            var transactionDate = DateOnly.FromDateTime(DateTime.UtcNow);

//            var command = new CreatePeopleForceInquiryTemplateAssignsRequest.Command
//            {
//                UserId = userId,
//                TransactionDate = transactionDate
//            };

//            var inquiryTemplate = new InquiryTemplateEntity
//            {
//                Name = "test"
//            };
//            var defaultInquiries = new List<UserInquiryTemplateEntity>
//            {
//                new()
//                {
//                    AssignedToUserId = userId,
//                    InquiryTemplateId = inquiryTemplate.Id,
//                    InquiryTemplate = inquiryTemplate,
//                }
//            };
//            var defaultAgreements = new List<UserAgreementEntity>
//            {
//                new()
//                {
//                    UserId = userId,
//                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                    { InquiryTemplate = inquiryTemplate }
//                }
//            };

//            var userContractsDates = new List<UserContractDatesOutput>
//            {
//                new()
//                {
//                    UserId = userId,
//                    HireDate = null,
//                    ContractStartDate = new DateOnly(2025, 01, 01),
//                    FireDate = null,
//                    ContractEndDate = null
//                }
//            };

//            _mockDefaultAgreementsComposer
//                .Setup(d => d.Compose(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(defaultAgreements);
//            _mockDefaultInquiriesComposer
//                .Setup(d => d.Compose(userId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(defaultInquiries);

//            _mockDefaultUserAgreementTransactionComposer
//                .Setup(x => x.Compose(userId, It.IsAny<InquiryTemplateEntity>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly?>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync([]);

//            _mockTeamBoardContract
//                .Setup(t => t.GetUserContractsDates(userId, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(userContractsDates);

//            _mockInquiriesDbContext
//                .Setup(x => x.UserAgreements)
//                .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);
//            _mockInquiriesDbContext
//                .Setup(x => x.UserInquiryTemplates)
//                .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);
//            _mockInquiriesDbContext
//                .Setup(x => x.UserAgreementTransactions)
//                .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

//            // Act
//            await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            _mockInquiriesDbContext.Verify(db => db.UserInquiryTemplates.AddRange(defaultInquiries), Times.Once);
//            _mockInquiriesDbContext
//                .Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(4));
//        }

//        [Fact]
//        public async Task HandleShouldSkipWhenUserHasTransactions()
//        {
//            // Arrange
//            var userId = 1;
//            var transactionDate = DateOnly.FromDateTime(DateTime.UtcNow);

//            var command = new CreatePeopleForceInquiryTemplateAssignsRequest.Command
//            {
//                UserId = userId,
//                TransactionDate = transactionDate
//            };

//            var inquiryTemplate = new InquiryTemplateEntity
//            {
//                Name = "test"
//            };
//            var defaultInquiries = new List<UserInquiryTemplateEntity>
//            {
//                new()
//                {
//                    AssignedToUserId = userId,
//                    InquiryTemplateId = inquiryTemplate.Id,
//                    InquiryTemplate = inquiryTemplate,
//                }
//            };
//            var defaultAgreements = new List<UserAgreementEntity>
//            {
//                new()
//                {
//                    UserId = userId,
//                    InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                    { InquiryTemplate = inquiryTemplate }
//                }
//            };

//            var userContractsDates = new List<UserContractDatesOutput>
//            {
//                new()
//                {
//                    UserId = userId,
//                    HireDate = null,
//                    ContractStartDate = new DateOnly(2025, 01, 01),
//                    FireDate = null,
//                    ContractEndDate = null
//                }
//            };

//            var transactions = new List<UserAgreementTransactionEntity>
//            {
//                new ()
//                {
//                    UserId = userId,
//                }
//            };

//            _mockDefaultAgreementsComposer
//                .Setup(d => d.Compose(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(defaultAgreements);
//            _mockDefaultInquiriesComposer
//                .Setup(d => d.Compose(userId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync(defaultInquiries);

//            _mockDefaultUserAgreementTransactionComposer
//                .Setup(x => x.Compose(userId, It.IsAny<InquiryTemplateEntity>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly?>(), It.IsAny<CancellationToken>()))
//                .ReturnsAsync([]);

//            _mockTeamBoardContract
//                .Setup(t => t.GetUserContractsDates(userId, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(userContractsDates);

//            _mockInquiriesDbContext
//                .Setup(x => x.UserAgreements)
//                .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);
//            _mockInquiriesDbContext
//                .Setup(x => x.UserInquiryTemplates)
//                .Returns(new List<UserInquiryTemplateEntity>().BuildMockDbSet().Object);
//            _mockInquiriesDbContext
//                .Setup(x => x.UserAgreementTransactions)
//                .Returns(transactions.BuildMockDbSet().Object);

//            // Act
//            await _handler.Handle(command, CancellationToken.None);

//            // Assert
//            _mockInquiriesDbContext.Verify(db => db.UserInquiryTemplates.AddRange(defaultInquiries), Times.Once);
//            _mockInquiriesDbContext
//                .Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
//        }
//    }
//}
