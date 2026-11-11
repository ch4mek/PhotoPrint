using MockQueryable.Moq;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Assigns.Core.Composers;

public class ResetUserAgreementsComposerTests
{
    private readonly IResetUserAgreementsComposer _composer;
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock;
    private readonly CancellationToken _cancellationToken;

    public ResetUserAgreementsComposerTests()
    {
        _inquiriesDbContextMock = new Mock<IInquiriesDbContext>();
        _composer = new ResetUserAgreementsComposer(_inquiriesDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ComposeMultipleUsersWhenMatchingAgreementsAndTransactionsExistReturnsCorrectDto()
    {
        // Arrange
        var userIds = new[] { 1, 2 };
        var agreementTemplate = new InquiryTemplateAgreementEntity
        {
            Id = 1,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                InquiryType = InquiryType.Vacation,
                Name = "test",
            }
        };

        var userAgreements = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                InquiryAgreementTemplateId = 2,
                InquiryTemplateAgreement =
                    new InquiryTemplateAgreementEntity
                    {
                        Id = 2,
                        InquiryTemplateId = 2,
                        InquiryTemplate =
                            new InquiryTemplateEntity { Id = 2, InquiryType = InquiryType.Vacation , Name = "test"}
                    }
            },
            new()
            {
                Id = 2,
                UserId = 2,
                InquiryAgreementTemplateId = 3,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    Id = 3,
                    InquiryTemplateId = 2,
                    InquiryTemplate =
                        new InquiryTemplateEntity { Id = 2, InquiryType = InquiryType.Vacation , Name = "test"}
                }
            }
        };

        var userAgreementTransactions = new List<UserAgreementTransactionEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                UserAgreementId = 1,
                UserAgreement = userAgreements[0],
                TransactionType = UserAgreementTransactionType.Accrual
            },
            new()
            {
                Id = 2,
                UserId = 2,
                UserAgreementId = 2,
                UserAgreement = userAgreements[1],
                TransactionType = UserAgreementTransactionType.Using
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreementTransactions)
            .Returns(userAgreementTransactions.BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userIds, agreementTemplate, _cancellationToken);

        // Assert
        result.UserAgreementsToRemove.Should().HaveCount(2);
        result.UserAgreementsToRemove.Should()
            .BeEquivalentTo(userAgreements, options => options.Excluding(x => x.Id));
        result.UserAgreementTransactionsToRemove.Should().HaveCount(2);
        result.UserAgreementTransactionsToRemove.Should()
            .BeEquivalentTo(userAgreementTransactions, options => options.Excluding(x => x.Id));
    }

    [Fact]
    public async Task ComposeMultipleUsersWhenNoMatchingAgreementsOrTransactionsExistReturnsEmptyDto()
    {
        // Arrange
        var userIds = new[] { 1, 2 };
        var agreementTemplate = new InquiryTemplateAgreementEntity
        {
            Id = 1,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                InquiryType = InquiryType.Vacation,
                Name = "test",
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userIds, agreementTemplate, _cancellationToken);

        // Assert
        result.UserAgreementsToRemove.Should().BeEmpty();
        result.UserAgreementTransactionsToRemove.Should().BeEmpty();
    }

    [Fact]
    public async Task ComposeMultipleUsersWhenSomeUsersHaveNoMatchingAgreementsReturnsOnlyMatchingAgreements()
    {
        // Arrange
        var userIds = new[] { 1, 2 };
        var agreementTemplate = new InquiryTemplateAgreementEntity
        {
            Id = 1,
            InquiryTemplateId = 1,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1,
                InquiryType = InquiryType.Vacation,
                Name = "test",
            }
        };

        var userAgreements = new List<UserAgreementEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                InquiryAgreementTemplateId = 2,
                InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
                {
                    Id = 2,
                    InquiryTemplateId = 2,
                    InquiryTemplate = new InquiryTemplateEntity { Id = 2, InquiryType = InquiryType.Vacation, Name = "test", }
                }
            }
        };

        var userAgreementTransactions = new List<UserAgreementTransactionEntity>
        {
            new()
            {
                Id = 1,
                UserId = 1,
                UserAgreementId = 1,
                UserAgreement = userAgreements[0],
                TransactionType = UserAgreementTransactionType.Accrual
            }
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreements)
            .Returns(userAgreements.BuildMockDbSet().Object);

        _inquiriesDbContextMock
            .Setup(x => x.UserAgreementTransactions)
            .Returns(userAgreementTransactions.BuildMockDbSet().Object);

        // Act
        var result = await _composer.Compose(userIds, agreementTemplate, _cancellationToken);

        // Assert
        result.UserAgreementsToRemove.Should().HaveCount(1);
        result.UserAgreementsToRemove.Should()
            .BeEquivalentTo(userAgreements, options => options.Excluding(x => x.Id));
        result.UserAgreementTransactionsToRemove.Should().HaveCount(1);
        result.UserAgreementTransactionsToRemove.Should()
            .BeEquivalentTo(userAgreementTransactions, options => options.Excluding(x => x.Id));
    }
}
