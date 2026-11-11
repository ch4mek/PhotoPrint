// TODO: Fix in story with transactions

//using MockQueryable.Moq;
//using Modsen.Portal.Core.Inquiries;
//using Modsen.Portal.Core.Exceptions;
//using Modsen.Portal.Core.UseCases.Core.Calculators;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Calculators;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Composers;

//public class UsingUserAgreementTransactionComposerTests
//{
//    private readonly UsingUserAgreementTransactionComposer _usingUserAgreementTransactionComposer;
//    private readonly Mock<IGetTransactionsWithPositiveAmountQuery> _getTransactionsWithPositiveAmountQueryMock = new();
//    private readonly IValidTillCalculator _validTillCalculator = new ValidTillCalculator();
//    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
//    private readonly IAccrualAmountOfDaysCalculator _accrualAmountOfDaysCalculator = new AccrualAmountOfDaysCalculator(new DateCalculator());
//    private readonly INextAccrualTransactionDateCalculator _nextAccrualTransactionDateCalculator = new NextAccrualTransactionDateCalculator();
//    private readonly CancellationToken _cancellationToken = CancellationToken.None;

//    public UsingUserAgreementTransactionComposerTests()
//    {
//        _usingUserAgreementTransactionComposer = new UsingUserAgreementTransactionComposer(
//            _getTransactionsWithPositiveAmountQueryMock.Object,
//            _validTillCalculator,
//            _inquiriesDbContextMock.Object,
//            _accrualAmountOfDaysCalculator,
//            _nextAccrualTransactionDateCalculator);
//    }

//    [Fact]
//    public async Task Handle_WhenAllCorrect_ShouldReturnTransactions()
//    {
//        //Arrange
//        var transactionDate = DateOnly.FromDateTime(DateTime.Now);
//        var transactionsCount = 2;
//        var userInquiryId = 1;
//        var amountToUse = 4.16M;
//        var userAgreement = new UserAgreementEntity
//        {
//            Id = 1,
//        };

//        _getTransactionsWithPositiveAmountQueryMock
//           .Setup(x => x.Execute(It.IsAny<UserAgreementEntity>(), It.IsAny<DateOnly>(), _cancellationToken))
//           .Returns(GetAccrualsAsync(transactionDate, transactionsCount, userAgreement.Id));

//        //Act
//        var transactions = _usingUserAgreementTransactionComposer.Compose(
//            [userAgreement],
//            transactionDate,
//            amountToUse,
//            userInquiryId,
//            _cancellationToken);

//        //Arrange
//        await foreach (var transaction in transactions)
//        {
//            amountToUse += transaction.AmountOfDays;
//            transactionsCount--;
//        }

//        amountToUse.Should().Be(0);
//        transactionsCount.Should().Be(0);
//    }

//    [Fact]
//    public async Task Handle_WhenRequestWithNegativeBalance_ShouldReturnTransactions()
//    {
//        //Arrange
//        var transactionDate = new DateOnly(2025, 3, 10);
//        var transactionsCount = 2;
//        var userInquiryId = 1;
//        var amountToUse = 9M;
//        var userAgreement = new UserAgreementEntity
//        {
//            Id = 1,
//            IsStaked = true,
//            DateFrom = DateOnly.FromDateTime(DateTime.Now),
//            DaysPerContractYear = 25,
//            AccrualType = AccrualType.ProportianallyEachMonth
//        };

//        _getTransactionsWithPositiveAmountQueryMock
//           .Setup(x => x.Execute(It.IsAny<UserAgreementEntity>(), It.IsAny<DateOnly>(), _cancellationToken))
//           .Returns(GetAccrualsAsync(transactionDate, transactionsCount, userAgreement.Id));

//        var accrualTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                UserAgreementId = userAgreement.Id,
//                TransactionDate = new DateOnly(2025, 3, 10),
//                TransactionType = UserAgreementTransactionType.Accrual,
//            },
//        };

//        _inquiriesDbContextMock
//            .Setup(x => x.UserAgreementTransactions)
//            .Returns(accrualTransactions.BuildMockDbSet().Object);

//        //Act
//        var transactions = _usingUserAgreementTransactionComposer.Compose(
//            [userAgreement],
//            transactionDate,
//            amountToUse,
//            userInquiryId,
//            _cancellationToken,
//            userAgreement);

//        //Arrange
//        var usingTransactions = 0;

//        await foreach (var transaction in transactions)
//        {
//            if (transaction.TransactionType == UserAgreementTransactionType.Using)
//            {
//                amountToUse += transaction.AmountOfDays;
//                usingTransactions++;
//            }
//        }

//        amountToUse.Should().Be(0);
//        usingTransactions.Should().Be(5);
//    }

//    [Fact]
//    public async Task Handle_WhenRequestWithNegativeBalance_ShouldThrowException()
//    {
//        //Arrange
//        var transactionDate = DateOnly.FromDateTime(DateTime.Now);
//        var transactionsCount = 2;
//        var userInquiryId = 1;
//        var amountToUse = 20M;
//        var userAgreement = new UserAgreementEntity
//        {
//            Id = 1,
//        };

//        _getTransactionsWithPositiveAmountQueryMock
//           .Setup(x => x.Execute(It.IsAny<UserAgreementEntity>(), It.IsAny<DateOnly>(), _cancellationToken))
//           .Returns(GetAccrualsAsync(transactionDate, transactionsCount, userAgreement.Id));

//        //Act
//        var transactions = _usingUserAgreementTransactionComposer.Compose(
//            [userAgreement],
//            transactionDate,
//            amountToUse,
//            userInquiryId,
//            _cancellationToken);

//        //Arrange
//        await Assert.ThrowsAsync<InvalidDataAppException>(async () =>
//        {
//            await foreach (var transaction in transactions) { }
//        });
//    }

//    private static async IAsyncEnumerable<(UserAgreementTransactionEntity accrual, decimal totalLeft)> GetAccrualsAsync(
//         DateOnly transactionDate,
//         int count,
//         int agreementId)
//    {
//        for (var i = 1; i < count + 1; i++)
//        {
//            yield return (
//                new UserAgreementTransactionEntity
//                {
//                    Id = i,
//                    TransactionDate = transactionDate,
//                    AmountOfDays = 2.08M,
//                    TransactionType = UserAgreementTransactionType.Accrual,
//                    UserAgreementId = agreementId,
//                    ValidTill = transactionDate,
//                }, 2.08m);
//        }
//    }
//}
