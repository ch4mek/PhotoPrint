//using Modsen.Portal.Core;
//using Modsen.Portal.Core.Inquiries;
//using Modsen.Portal.Core.UseCases.Core.Calculators;
//using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core;
//using Modsen.Portal.Inquiries.Requests.Agreements.Core.Composers;
//using Moq;

//namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Composers;

//public class DefaultUserAgreementDisposalTransactionComposerTests
//{
//    private readonly IDefaultUserAgreementDisposalTransactionComposer _composer;
//    private readonly Mock<IDateCalculator> _dateCalculatorMock;
//    private readonly Mock<IDisposeUserAgreementTransactionComposer> _disposeUserAgreementTransactionComposerMock;
//    private readonly CancellationToken _cancellationToken;

//    public DefaultUserAgreementDisposalTransactionComposerTests()
//    {
//        _dateCalculatorMock = new Mock<IDateCalculator>();
//        _disposeUserAgreementTransactionComposerMock = new Mock<IDisposeUserAgreementTransactionComposer>();
//        _cancellationToken = CancellationToken.None;

//        _composer = new DefaultUserAgreementDisposalTransactionComposer(
//            _dateCalculatorMock.Object,
//            _disposeUserAgreementTransactionComposerMock.Object);
//    }

//    [Fact]
//    public async Task ComposeWhenNonStackedAgreementsWithYearlyAccrualExistReturnsCorrectDisposalTransactions()
//    {
//        // Arrange
//        var transactionDateTo = DateOnly.FromDateTime(DateTime.UtcNow);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                DateFrom = transactionDateTo.AddYears(-2),
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                {
//                    Id = 1,
//                    InquiryTemplateId = 1,
//                    InquiryTemplate = new InquiryTemplateEntity
//                    {
//                        Name = "test",
//                        Id = 1,
//                        AccrualType = AccrualType.OnceInYear,
//                        IsStaked = false,
//                        InquiryType = InquiryType.SickDay
//                    }
//                }
//            }
//        };

//        var dates = new List<DateOnly>
//        {
//            transactionDateTo.AddYears(-2),
//            transactionDateTo.AddYears(-1)
//        };

//        var disposalTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                UserAgreementId = 1,
//                TransactionDate = transactionDateTo.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddYears(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                AmountOfDays = -5m,
//                TransactionType = UserAgreementTransactionType.Disposal,
//                ValidTill = transactionDateTo.AddYears(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                RelatedTransactionId = 1,
//                UserInquiryId = null
//            },
//            new()
//            {
//                Id = 2,
//                UserId = 1,
//                UserAgreementId = 1,
//                TransactionDate = transactionDateTo.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                AmountOfDays = -5m,
//                TransactionType = UserAgreementTransactionType.Disposal,
//                ValidTill = transactionDateTo.AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                RelatedTransactionId = 2,
//                UserInquiryId = null
//            }
//        };

//        _dateCalculatorMock
//            .Setup(x => x.CalculatePassedYears(userAgreements[0].DateFrom, transactionDateTo))
//            .Returns(dates);

//        _disposeUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);

//        // Act
//        var result = await _composer.Compose(userAgreements, transactionDateTo, _cancellationToken);

//        // Assert
//        result.Should().HaveCount(2);
//        result.Should().BeEquivalentTo(disposalTransactions, options => options.Excluding(x => x.Id));
//        _dateCalculatorMock.Verify(x => x.CalculatePassedYears(userAgreements[0].DateFrom, transactionDateTo), Times.Once());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken), Times.Once());
//    }

//    [Fact]
//    public async Task ComposeWhenNonStackedAgreementsWithMonthlyAccrualExistReturnsCorrectDisposalTransactions()
//    {
//        // Arrange
//        var transactionDateTo = DateOnly.FromDateTime(DateTime.UtcNow);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                DateFrom = transactionDateTo.AddMonths(-2),
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                {
//                    Id = 1,
//                    InquiryTemplateId = 1,
//                    InquiryTemplate = new InquiryTemplateEntity
//                    {
//                        AccrualType = AccrualType.ProportianallyEachMonth,
//                        IsStaked = false,
//                        Id = 1,
//                        InquiryType = InquiryType.SickDay,
//                        Name = "test",
//                    }
//                }
//            }
//        };

//        var dates = new List<DateOnly>
//        {
//            transactionDateTo.AddMonths(-1),
//            transactionDateTo
//        };

//        var disposalTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                UserAgreementId = 1,
//                TransactionDate = transactionDateTo.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddMonths(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                AmountOfDays = -2m,
//                TransactionType = UserAgreementTransactionType.Disposal,
//                ValidTill = transactionDateTo.AddMonths(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                RelatedTransactionId = 1,
//                UserInquiryId = null
//            },
//            new()
//            {
//                Id = 2,
//                UserId = 1,
//                UserAgreementId = 1,
//                TransactionDate = transactionDateTo.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                AmountOfDays = -2m,
//                TransactionType = UserAgreementTransactionType.Disposal,
//                ValidTill = transactionDateTo.AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                RelatedTransactionId = 2,
//                UserInquiryId = null
//            }
//        };

//        _dateCalculatorMock
//            .Setup(x => x.CalculatePassedMonthsDates(userAgreements[0].DateFrom.AddMonths(1), transactionDateTo))
//            .Returns(dates);

//        _disposeUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);

//        // Act
//        var result = await _composer.Compose(userAgreements, transactionDateTo, _cancellationToken);

//        // Assert
//        result.Should().HaveCount(2);
//        result.Should().BeEquivalentTo(disposalTransactions, options => options.Excluding(x => x.Id));
//        _dateCalculatorMock.Verify(x => x.CalculatePassedMonthsDates(userAgreements[0].DateFrom.AddMonths(1), transactionDateTo), Times.Once());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken), Times.Once());
//    }

//    [Fact]
//    public async Task ComposeWhenAllAgreementsAreStackedReturnsEmptyList()
//    {
//        // Arrange
//        var transactionDateTo = DateOnly.FromDateTime(DateTime.UtcNow);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                DateFrom = transactionDateTo.AddYears(-2),
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                {
//                    Id = 1,
//                    InquiryTemplateId = 1,
//                    InquiryTemplate = new InquiryTemplateEntity
//                    {
//                        Name = "test",
//                        Id = 1,
//                        AccrualType = AccrualType.OnceInYear,
//                        IsStaked = true, // Stacked
//                        InquiryType = InquiryType.Vacation
//                    }
//                }
//            }
//        };

//        // Act
//        var result = await _composer.Compose(userAgreements, transactionDateTo, _cancellationToken);

//        // Assert
//        result.Should().BeEmpty();
//        _dateCalculatorMock.Verify(x => x.CalculatePassedYears(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never());
//        _dateCalculatorMock.Verify(x => x.CalculatePassedMonthsDates(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<List<DateOnly>>(), It.IsAny<DateOnly>(), _cancellationToken),
//            Times.Never());
//    }

//    [Fact]
//    public async Task ComposeWhenNoAgreementsProvidedReturnsEmptyList()
//    {
//        // Arrange
//        var transactionDateTo = DateOnly.FromDateTime(DateTime.UtcNow);
//        var userAgreements = new List<UserAgreementEntity>();

//        // Act
//        var result = await _composer.Compose(userAgreements, transactionDateTo, _cancellationToken);

//        // Assert
//        result.Should().BeEmpty();
//        _dateCalculatorMock.Verify(x => x.CalculatePassedYears(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never());
//        _dateCalculatorMock.Verify(x => x.CalculatePassedMonthsDates(It.IsAny<DateOnly>(), It.IsAny<DateOnly>()), Times.Never());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(It.IsAny<UserAgreementEntity>(), It.IsAny<List<DateOnly>>(), It.IsAny<DateOnly>(), _cancellationToken),
//            Times.Never());
//    }

//    [Fact]
//    public async Task ComposeWhenMixedStackedAndNonStackedAgreementsReturnsOnlyNonStackedTransactions()
//    {
//        // Arrange
//        var transactionDateTo = DateOnly.FromDateTime(DateTime.UtcNow);
//        var userAgreements = new List<UserAgreementEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                DateFrom = transactionDateTo.AddYears(-2),
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                {
//                    Id = 1,
//                    InquiryTemplateId = 1,
//                    InquiryTemplate = new InquiryTemplateEntity
//                    {
//                        Name="Test",
//                        Id = 1,
//                        AccrualType = AccrualType.OnceInYear,
//                        IsStaked = false,
//                        InquiryType = InquiryType.SickDay
//                    }
//                }
//            },
//            new()
//            {
//                Id = 2,
//                UserId = 1,
//                DateFrom = transactionDateTo.AddYears(-2),
//                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
//                {
//                    Id = 2,
//                    InquiryTemplateId = 2,
//                    InquiryTemplate = new InquiryTemplateEntity
//                    {
//                        Name="Test",
//                        Id = 2,
//                        AccrualType = AccrualType.OnceInYear,
//                        IsStaked = true,
//                        InquiryType = InquiryType.Vacation
//                    }
//                }
//            }
//        };

//        var dates = new List<DateOnly>
//        {
//            transactionDateTo.AddYears(-2),
//            transactionDateTo.AddYears(-1)
//        };

//        var disposalTransactions = new List<UserAgreementTransactionEntity>
//        {
//            new()
//            {
//                Id = 1,
//                UserId = 1,
//                UserAgreementId = 1,
//                TransactionDate = transactionDateTo.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddYears(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                AmountOfDays = -5m,
//                TransactionType = UserAgreementTransactionType.Disposal,
//                ValidTill = transactionDateTo.AddYears(-1).AddYears(PortalSharedConstants.PeriodUntilAgreementDaysResetInYears),
//                RelatedTransactionId = 1,
//                UserInquiryId = null
//            }
//        };

//        _dateCalculatorMock
//            .Setup(x => x.CalculatePassedYears(userAgreements[0].DateFrom, transactionDateTo))
//            .Returns(dates);

//        _disposeUserAgreementTransactionComposerMock
//            .Setup(x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken))
//            .ReturnsAsync(disposalTransactions);

//        // Act
//        var result = await _composer.Compose(userAgreements, transactionDateTo, _cancellationToken);

//        // Assert
//        result.Should().HaveCount(1);
//        result.Should().BeEquivalentTo(disposalTransactions, options => options.Excluding(x => x.Id));
//        _dateCalculatorMock.Verify(x => x.CalculatePassedYears(userAgreements[0].DateFrom, transactionDateTo), Times.Once());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(userAgreements[0], dates, transactionDateTo, _cancellationToken), Times.Once());
//        _disposeUserAgreementTransactionComposerMock.Verify(
//            x => x.Compose(userAgreements[1], It.IsAny<List<DateOnly>>(), transactionDateTo, _cancellationToken), Times.Never());
//    }
//}
