using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Resolvers;

public class InquiryAcceptedStatusResolverTests
{
    private readonly InquiryAcceptedStatusResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IHistoricalRemainingBalanceDaysHoursResolver> _remainingBalanceDaysHoursResolver = new();
    private readonly Mock<IIdentityContract> _identityContract = new();
    private readonly Mock<IClockService> _clockService = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public InquiryAcceptedStatusResolverTests()
    {
        _resolver = new InquiryAcceptedStatusResolver(_inquiriesDbContext.Object,
            _remainingBalanceDaysHoursResolver.Object, _identityContract.Object,
            _clockService.Object);
    }

    [Fact]
    public async Task ResolveWhenInquiryWithAgreementShouldAddTransaction()
    {
        // Arrange
        var inquiry = new UserInquiryEntity
        {
            Id = 1,
            AgreementTemplateId = 1,
            InquiryTemplateId = 1,
            TotalAccountantDays = 100,
            Status = UserInquiryStatus.Open,
            CreatedById = 1,
            CreatedByUser = new InquiriesUserEntity
            {
                Email = "email",
                FirstName = "Creator",
                LastName = "inquiry",
                Position = PortalRoleNames.Cto
            },
            UserId = 2,
            User = new InquiriesUserEntity
            {
                Email = "email",
                FirstName = "Receiver",
                LastName = "Inquiry",
                Position = PortalRoleNames.Employee
            },
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "test",
                InquiryType = InquiryType.Vacation,
            }
        };

        var agreement = new UserAgreementEntity
        {
            Id = 1,
            UserId = 2,
            IsDeleted = false,
            InquiryAgreementTemplateId = inquiry.AgreementTemplateId.Value
        };

        var manager = new InquiriesUserEntity
        {
            UserId = 3,
            Email = "email",
            FirstName = "Manager",
            LastName = "Inquiry",
            Position = PortalRoleNames.GroupManager
        };

        _inquiriesDbContext.SetupGet(x => x.UserAgreements)
            .Returns(new List<UserAgreementEntity> { agreement }.BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserAgreementTransactions)
            .Returns(Array.Empty<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        _remainingBalanceDaysHoursResolver.Setup(x => x.ResolveAsync(inquiry, _cancellationToken));

        _identityContract.Setup(x => x.GetHigherManagerIdsIncludingChiefs(inquiry.UserId, _cancellationToken))
            .ReturnsAsync([manager.UserId]);

        _inquiriesDbContext.Setup(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity> { manager }.BuildMockDbSet().Object);

        _clockService.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        // Act
        await _resolver.ResolveAsync(inquiry, _cancellationToken);

        // Assert
        inquiry.Status.Should().Be(UserInquiryStatus.Approved);
        inquiry.HistoricalManagerId.Should().Be(manager.UserId);
        inquiry.HistoricalManagerPosition.Should().Be(manager.Position);
        inquiry.HistoricalUserPosition.Should().Be(inquiry.HistoricalUserPosition);
        inquiry.HistoricalCreatedByUserPosition.Should().Be(inquiry.CreatedByUser.Position);
    }

    [Fact]
    public async Task ResolveWhenInquiryWithoutAgreementShouldNotAddTransaction()
    {
        // Arrange
        var inquiry = new UserInquiryEntity
        {
            Id = 1,
            InquiryTemplateId = 1,
            TotalAccountantDays = 100,
            Status = UserInquiryStatus.Open,
            CreatedById = 1,
            CreatedByUser = new InquiriesUserEntity
            {
                Email = "email",
                FirstName = "Creator",
                LastName = "inquiry",
                Position = PortalRoleNames.Cto
            },
            UserId = 2,
            User = new InquiriesUserEntity
            {
                Email = "email",
                FirstName = "Receiver",
                LastName = "Inquiry",
                Position = PortalRoleNames.Employee
            },
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "test",
                InquiryType = InquiryType.SickLeave,
            }
        };

        var manager = new InquiriesUserEntity
        {
            UserId = 3,
            Email = "email",
            FirstName = "Manager",
            LastName = "Inquiry",
            Position = PortalRoleNames.GroupManager
        };

        _inquiriesDbContext.SetupGet(x => x.UserAgreements)
            .Returns(Array.Empty<UserAgreementEntity>().BuildMockDbSet().Object);

        _inquiriesDbContext.SetupGet(x => x.UserAgreementTransactions)
            .Returns(Array.Empty<UserAgreementTransactionEntity>().BuildMockDbSet().Object);

        _remainingBalanceDaysHoursResolver.Setup(x => x.ResolveAsync(inquiry, _cancellationToken));

        _identityContract.Setup(x => x.GetHigherManagerIdsIncludingChiefs(inquiry.UserId, _cancellationToken))
            .ReturnsAsync([manager.UserId]);

        _inquiriesDbContext.Setup(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity> { manager }.BuildMockDbSet().Object);

        _clockService.Setup(x => x.UtcNow)
            .Returns(DateTime.UtcNow);

        // Act
        await _resolver.ResolveAsync(inquiry, _cancellationToken);

        // Assert
        inquiry.Status.Should().Be(UserInquiryStatus.Approved);
        inquiry.HistoricalManagerId.Should().Be(manager.UserId);
        inquiry.HistoricalManagerPosition.Should().Be(manager.Position);
        inquiry.HistoricalUserPosition.Should().Be(inquiry.HistoricalUserPosition);
        inquiry.HistoricalCreatedByUserPosition.Should().Be(inquiry.CreatedByUser.Position);
    }
}
