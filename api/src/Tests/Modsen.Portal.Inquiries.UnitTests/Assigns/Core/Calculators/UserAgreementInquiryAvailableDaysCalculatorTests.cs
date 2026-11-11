using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Core.Calculators;

public class UserAgreementInquiryAvailableDaysCalculatorTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();

    private readonly UserAgreementInquiryAvailableDaysCalculator _calculator;
    private readonly CancellationToken _token = CancellationToken.None;
    public UserAgreementInquiryAvailableDaysCalculatorTests()
    {
        _calculator = new UserAgreementInquiryAvailableDaysCalculator(_dbContextMock.Object);
    }

    [Fact]
    public async Task CalculateWhenNoOpenRequestsReturnsOnlyTransactionsDays()
    {
        //Arrange
        var userId = 1;

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            InquiryType = InquiryType.Vacation,
            Name = "test",
        };

        var transactionsDays = new[] {20, 30, -10};

        _dbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[0],
                    InquiryTemplate = inquiryTemplate,
                },
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[1],
                    InquiryTemplate = inquiryTemplate,
                },
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[2],
                    InquiryTemplate = inquiryTemplate,
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);

        //Act
        var result =  await _calculator.CalculateAsync(userId,
            inquiryTemplate.Id, _token);

        //Assert
        result.Should().Be(transactionsDays.Sum());

    }

    [Fact]
    public async Task CalculateWhenUserHasOpenRequestsReturnsTransactionsDaysWithoutRequestDays()
    {
        //Arrange
        var userId = 1;

        var inquiryTemplate = new InquiryTemplateEntity
        {
            Id = 1,
            InquiryType = InquiryType.Vacation,
            Name = "test",
        };

        var transactionsDays = new[] {20, 30, -10};
        var requestsDays = new[] { 5, 12 };

        _dbContextMock
            .Setup(context => context.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>
            {
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[0],
                    InquiryTemplate = inquiryTemplate
                },
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[1],
                    InquiryTemplate = inquiryTemplate
                },
                new()
                {
                    UserId = userId,
                    AmountOfDays = transactionsDays[2],
                    InquiryTemplate = inquiryTemplate
                }
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.UserInquiries)
            .Returns(new List<UserInquiryEntity>
            {
                new()
                {
                    UserId = userId,
                    InquiryTemplate = inquiryTemplate,
                    Status = UserInquiryStatus.Rejected,
                    TotalAccountantDays = requestsDays[0]
                },
                new()
                {
                    UserId = userId,
                    InquiryTemplate = inquiryTemplate,
                    Status = UserInquiryStatus.Open,
                    TotalAccountantDays = requestsDays[1]
                }
            }.BuildMockDbSet().Object);

        var expectedResult = transactionsDays.Sum() - requestsDays[1];

        //Act
        var result =  await _calculator.CalculateAsync(userId,
            inquiryTemplate.Id, _token);

        //Assert
        result.Should().Be(expectedResult);

    }
}
