//using MockQueryable.Moq;
//using Modsen.Portal.Core.Inquiries;
//using Modsen.Portal.Core.Services;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Transactions.Composers;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Transactions.Resolvers;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Resolvers;

//public class DefaultInquiriesResolverTests
//{
//    private readonly Mock<IDefaultUserAgreementTransactionComposer> _defaultUserAgreementTransactionComposerMock;
//    private readonly Mock<IDefaultUserInquiriesTemplateComposer> _defaultUserInquiriesTemplateComposerMock;
//    private readonly Mock<IClockService> _clockServiceMock;
//    private readonly Mock<IDefaultAgreementsComposer> _defaultAgreementsComposerMock;
//    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock;
//    private readonly Mock<IDefaultUserAgreementDisposalTransactionComposer> _defaultUserAgreementDisposalTransactionComposerMock;
//    private readonly IDefaultInquiriesResolver _resolver;
//    private readonly CancellationToken _cancellationToken;

//    public DefaultInquiriesResolverTests()
//    {
//        _defaultUserAgreementTransactionComposerMock = new Mock<IDefaultUserAgreementTransactionComposer>();
//        _defaultUserInquiriesTemplateComposerMock = new Mock<IDefaultUserInquiriesTemplateComposer>();
//        _clockServiceMock = new Mock<IClockService>();
//        _defaultAgreementsComposerMock = new Mock<IDefaultAgreementsComposer>();
//        _inquiriesDbContextMock = new Mock<IInquiriesDbContext>();
//        _defaultUserAgreementDisposalTransactionComposerMock = new Mock<IDefaultUserAgreementDisposalTransactionComposer>();
//        _resolver = new DefaultInquiriesResolver(
//            _defaultUserAgreementTransactionComposerMock.Object,
//            _defaultUserInquiriesTemplateComposerMock.Object,
//            _clockServiceMock.Object,
//            _defaultAgreementsComposerMock.Object,
//            _inquiriesDbContextMock.Object,
//            _defaultUserAgreementDisposalTransactionComposerMock.Object);
//        _cancellationToken = CancellationToken.None;
//    }

//    [Fact]
//    public async Task ResolveWhenValidDataProvidedAddsAgreementsTransactionsAndTemplates()
//    {
//        // Arrange
//        var userId = 1;
//        var today = DateOnly.FromDateTime(DateTime.Now);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = userId,
//                DateFrom = today,
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { Id = 1 }
//            }
//        };
//        var accrualTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = userId,
//                TransactionDate = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
//                TransactionType = UserAgreementTransactionType.Accrual,
//                UserAgreementId = 1
//            }
//        };
//        var disposalTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 2,
//                UserId = userId,
//                TransactionDate = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
//                TransactionType = UserAgreementTransactionType.Disposal,
//                UserAgreementId = 1
//            }
//        };
//        var userInquiryTemplates = new List<UserInquiryTemplateEntity>
//        {
//            new()
//            {
//                Id = 1,
//                AssignedToUserId = userId,
//                DateFrom = today,
//                InquiryTemplateId = 1
//            }
//        };

//        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(today);
//        _defaultAgreementsComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userAgreements);
//        _defaultUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(accrualTransactions);
//        _defaultUserAgreementDisposalTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);
//        _defaultUserInquiriesTemplateComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userInquiryTemplates);

//        var userAgreementsDbSet = new List<UserAgreementEntity>().BuildMockDbSet();
//        var userAgreementTransactionsDbSet = new List<UserAgreementTransactionEntity>().BuildMockDbSet();
//        var userInquiryTemplatesDbSet = new List<UserInquiryTemplateEntity>().BuildMockDbSet();

//        _inquiriesDbContextMock.Setup(x => x.UserAgreements).Returns(userAgreementsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserAgreementTransactions).Returns(userAgreementTransactionsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserInquiryTemplates).Returns(userInquiryTemplatesDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken)).ReturnsAsync(1);

//        // Act
//        await _resolver.Resolve(userId, _cancellationToken);

//        // Assert
//        _defaultAgreementsComposerMock.Verify(x =>
//            x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserAgreements.AddRange(userAgreements), Times.Once());
//        _defaultUserAgreementTransactionComposerMock.Verify(x =>
//            x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserAgreementTransactions.AddRange(accrualTransactions), Times.Once());
//        _defaultUserAgreementDisposalTransactionComposerMock.Verify(x =>
//            x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserAgreementTransactions.AddRange(disposalTransactions), Times.Once());
//        _defaultUserInquiriesTemplateComposerMock.Verify(x =>
//            x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserInquiryTemplates.AddRange(userInquiryTemplates), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.SaveChangesAsync(_cancellationToken), Times.Exactly(4));
//    }

//    [Fact]
//    public async Task ResolveWhenNoAgreementsReturnedDoesNotAddTransactionsOrTemplates()
//    {
//        // Arrange
//        var userId = 1;
//        var today = DateOnly.FromDateTime(DateTime.Now);
//        var userAgreements = new List<UserAgreementEntity>();

//        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(today);
//        _defaultAgreementsComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userAgreements);

//        var userAgreementsDbSet = new List<UserAgreementEntity>().BuildMockDbSet();
//        var userAgreementTransactionsDbSet = new List<UserAgreementTransactionEntity>().BuildMockDbSet();
//        var userInquiryTemplatesDbSet = new List<UserInquiryTemplateEntity>().BuildMockDbSet();

//        _inquiriesDbContextMock.Setup(x => x.UserAgreements).Returns(userAgreementsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserAgreementTransactions).Returns(userAgreementTransactionsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserInquiryTemplates).Returns(userInquiryTemplatesDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken)).ReturnsAsync(1);

//        // Act
//        await _resolver.Resolve(userId, _cancellationToken);

//        // Assert
//        _defaultAgreementsComposerMock.Verify(x => x.Compose(userId, _cancellationToken), Times.Once());
//        _defaultUserAgreementTransactionComposerMock.Verify(x => x.Compose(It.IsAny<List<UserAgreementEntity>>(), today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserAgreementTransactions.AddRange(It.IsAny<IEnumerable<UserAgreementTransactionEntity>>()), Times.Exactly(2));
//        _defaultUserAgreementDisposalTransactionComposerMock.Verify(x => x.Compose(It.IsAny<List<UserAgreementEntity>>(), today, _cancellationToken), Times.Once());
//        _defaultUserInquiriesTemplateComposerMock.Verify(x => x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserInquiryTemplates.AddRange(It.IsAny<IEnumerable<UserInquiryTemplateEntity>>()), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Exactly(4));
//    }

//    [Fact]
//    public async Task ResolveWhenNoTransactionsReturnedDoesNotAddTransactions()
//    {
//        // Arrange
//        var userId = 1;
//        var today = DateOnly.FromDateTime(DateTime.Now);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = userId,
//                DateFrom = today,
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { Id = 1 }
//            }
//        };
//        var accrualTransactions = new List<UserAgreementTransactionEntity>();
//        var disposalTransactions = new List<UserAgreementTransactionEntity>();
//        var userInquiryTemplates = new List<UserInquiryTemplateEntity>
//        {
//            new()
//            {
//                Id = 1,
//                AssignedToUserId = userId,
//                DateFrom = today,
//                InquiryTemplateId = 1
//            }
//        };

//        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(today);
//        _defaultAgreementsComposerMock
//            .Setup(x => x.Compose(userId, , _cancellationToken))
//            .ReturnsAsync(userAgreements);
//        _defaultUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(accrualTransactions);
//        _defaultUserAgreementDisposalTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);
//        _defaultUserInquiriesTemplateComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userInquiryTemplates);

//        var userAgreementsDbSet = new List<UserAgreementEntity>().BuildMockDbSet();
//        var userAgreementTransactionsDbSet = new List<UserAgreementTransactionEntity>().BuildMockDbSet();
//        var userInquiryTemplatesDbSet = new List<UserInquiryTemplateEntity>().BuildMockDbSet();

//        _inquiriesDbContextMock.Setup(x => x.UserAgreements).Returns(userAgreementsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserAgreementTransactions).Returns(userAgreementTransactionsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserInquiryTemplates).Returns(userInquiryTemplatesDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken)).ReturnsAsync(1);

//        // Act
//        await _resolver.Resolve(userId, _cancellationToken);

//        // Assert
//        _defaultAgreementsComposerMock.Verify(x =>
//            x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserAgreements.AddRange(userAgreements), Times.Once());
//        _defaultUserAgreementTransactionComposerMock.Verify(x =>
//            x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserAgreementTransactions.AddRange(It.IsAny<IEnumerable<UserAgreementTransactionEntity>>()),
//            Times.Exactly(2));
//        _defaultUserAgreementDisposalTransactionComposerMock.Verify(x =>
//            x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _defaultUserInquiriesTemplateComposerMock.Verify(x =>
//            x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.UserInquiryTemplates.AddRange(userInquiryTemplates), Times.Once());
//        _inquiriesDbContextMock.Verify(x =>
//            x.SaveChangesAsync(_cancellationToken), Times.Exactly(4));
//    }

//    [Fact]
//    public async Task ResolveWhenNoTemplatesReturnedDoesNotAddTemplates()
//    {
//        // Arrange
//        var userId = 1;
//        var today = DateOnly.FromDateTime(DateTime.Now);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = userId,
//                DateFrom = today,
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity { Id = 1 }
//            }
//        };
//        var accrualTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = userId,
//                TransactionDate = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
//                TransactionType = UserAgreementTransactionType.Accrual,
//                UserAgreementId = 1
//            }
//        };
//        var disposalTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 2,
//                UserId = userId,
//                TransactionDate = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
//                TransactionType = UserAgreementTransactionType.Disposal,
//                UserAgreementId = 1
//            }
//        };
//        var userInquiryTemplates = new List<UserInquiryTemplateEntity>();

//        _clockServiceMock.Setup(x => x.TodayUtc()).Returns(today);
//        _defaultAgreementsComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userAgreements);
//        _defaultUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(accrualTransactions);
//        _defaultUserAgreementDisposalTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements, today, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);
//        _defaultUserInquiriesTemplateComposerMock
//            .Setup(x => x.Compose(userId, _cancellationToken))
//            .ReturnsAsync(userInquiryTemplates);

//        var userAgreementsDbSet = new List<UserAgreementEntity>().BuildMockDbSet();
//        var userAgreementTransactionsDbSet = new List<UserAgreementTransactionEntity>().BuildMockDbSet();
//        var userInquiryTemplatesDbSet = new List<UserInquiryTemplateEntity>().BuildMockDbSet();

//        _inquiriesDbContextMock.Setup(x => x.UserAgreements).Returns(userAgreementsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserAgreementTransactions).Returns(userAgreementTransactionsDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.UserInquiryTemplates).Returns(userInquiryTemplatesDbSet.Object);
//        _inquiriesDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken)).ReturnsAsync(1);

//        // Act
//        await _resolver.Resolve(userId, _cancellationToken);

//        // Assert
//        _defaultAgreementsComposerMock.Verify(x => x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserAgreements.AddRange(userAgreements), Times.Once());
//        _defaultUserAgreementTransactionComposerMock.Verify(x => x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserAgreementTransactions.AddRange(accrualTransactions), Times.Once());
//        _defaultUserAgreementDisposalTransactionComposerMock.Verify(x => x.Compose(userAgreements, today, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserAgreementTransactions.AddRange(disposalTransactions), Times.Once());
//        _defaultUserInquiriesTemplateComposerMock.Verify(x => x.Compose(userId, _cancellationToken), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.UserInquiryTemplates.AddRange(It.IsAny<IEnumerable<UserInquiryTemplateEntity>>()), Times.Once());
//        _inquiriesDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Exactly(4));
//    }
//}
