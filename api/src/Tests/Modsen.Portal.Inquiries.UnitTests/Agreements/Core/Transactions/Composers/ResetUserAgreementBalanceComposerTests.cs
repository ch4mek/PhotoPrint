using MockQueryable.Moq;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Agreements.Core.Transactions.Composers;

public class ResetUserAgreementBalanceComposerTests
{
    private readonly Mock<IDisposeUserAgreementTransactionComposer> _disposeUserAgreementTransactionComposerMock = new();
    private readonly Mock<IClockService> _clockMock = new();
    private readonly Mock<IInquiriesDbContext> _dbContextMock = new();

    private readonly ResetUserAgreementBalanceComposer _composer;

    public ResetUserAgreementBalanceComposerTests()
    {
        _clockMock.Setup(x => x.UtcNow).Returns(new DateTime(2024, 1, 1));

        _composer = new ResetUserAgreementBalanceComposer(
            _disposeUserAgreementTransactionComposerMock.Object,
            _clockMock.Object,
            _dbContextMock.Object);

        _dbContextMock.Setup(x => x.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);
    }

    private static async IAsyncEnumerable<UserAgreementTransactionEntity> CreateStream(
        params UserAgreementTransactionEntity[] items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }

    private static UserAgreementEntity CreateAgreement(int userId, int templateId)
    {
        return new UserAgreementEntity
        {
            UserId = userId,
            InquiryTemplateAgreement = new InquiryTemplateAgreementEntity
            {
                InquiryTemplateId = templateId
            }
        };
    }

    [Fact]
    public async Task ComposeShouldCreateTransactionsWhenAgreementsExist()
    {
        var agreements = new List<UserAgreementEntity>
        {
            CreateAgreement(1, 10)
        };

        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        var transaction = new UserAgreementTransactionEntity();

        _disposeUserAgreementTransactionComposerMock.Setup(x => x.Compose(
                10,
                1,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>(),
                true,
                null))
            .Returns(CreateStream(transaction));

        _dbContextMock.Setup(x => x.UserAgreementTransactions.AddRange(It.IsAny<List<UserAgreementTransactionEntity>>()));

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _composer.Compose(1, CancellationToken.None);

        result.Should().ContainSingle();
        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ComposeShouldReturnEmptyWhenNoAgreements()
    {
        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(new List<UserAgreementEntity>().BuildMockDbSet().Object);

        var result = await _composer.Compose(1, CancellationToken.None);

        result.Should().BeEmpty();

        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ComposeShouldFilterByTemplateId()
    {
        var agreements = new List<UserAgreementEntity>
        {
            CreateAgreement(1, 10),
            CreateAgreement(1, 20)
        };

        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.UserAgreementTransactions)
            .Returns(new List<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        _disposeUserAgreementTransactionComposerMock.Setup(x => x.Compose(
                10,
                1,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>(),
                true,
                null))
            .Returns(CreateStream(new UserAgreementTransactionEntity()));

        var result = await _composer.Compose(new[] { 1 }, 10, CancellationToken.None);

        result.Should().HaveCount(1);

        _disposeUserAgreementTransactionComposerMock.Verify(x => x.Compose(
            20,
            1,
            It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>(),
            true,
            null), Times.Never);
    }

    [Fact]
    public async Task ComposeShouldHandleMultipleUsers()
    {
        var agreements = new List<UserAgreementEntity>
        {
            CreateAgreement(1, 10),
            CreateAgreement(2, 10)
        };

        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        _disposeUserAgreementTransactionComposerMock.Setup(x => x.Compose(
                10,
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>(),
                true,
                null))
            .Returns(CreateStream(new UserAgreementTransactionEntity()));

        var result = await _composer.Compose([1, 2], 10, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ComposeShouldPassIgnoreTransactionsAfter()
    {
        var agreements = new List<UserAgreementEntity>
        {
            CreateAgreement(1, 10)
        };

        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        var date = new DateTime(2025, 1, 1);

        _disposeUserAgreementTransactionComposerMock.Setup(x => x.Compose(
                10,
                1,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>(),
                true,
                date))
            .Returns(CreateStream(new UserAgreementTransactionEntity()));

        await _composer.Compose(1, DateTime.UtcNow, CancellationToken.None, date);

        _disposeUserAgreementTransactionComposerMock.VerifyAll();
    }

    [Fact]
    public async Task ComposeShouldAggregateMultipleTransactionsFromStream()
    {
        var agreements = new List<UserAgreementEntity>
        {
            CreateAgreement(1, 10)
        };

        _dbContextMock.Setup(x => x.UserAgreements)
            .Returns(agreements.BuildMockDbSet().Object);

        var tx1 = new UserAgreementTransactionEntity();
        var tx2 = new UserAgreementTransactionEntity();

        _disposeUserAgreementTransactionComposerMock.Setup(x => x.Compose(
                10,
                1,
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>(),
                true,
                null))
            .Returns(CreateStream(tx1, tx2));

        var result = await _composer.Compose(1, CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
