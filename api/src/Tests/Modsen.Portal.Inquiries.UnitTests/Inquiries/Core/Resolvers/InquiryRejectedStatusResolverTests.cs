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

public class InquiryRejectedStatusResolverTests
{
    private readonly InquiryRejectedStatusResolver _resolver;
    private readonly Mock<IHistoricalRemainingBalanceDaysHoursResolver> _remainingBalanceDaysHoursResolver = new();
    private readonly Mock<IIdentityContract> _identityContract = new();
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IClockService> _clockService = new();

    public InquiryRejectedStatusResolverTests()
    {
        _resolver = new(_remainingBalanceDaysHoursResolver.Object, _identityContract.Object,
            _inquiriesDbContext.Object, _clockService.Object);

        _clockService.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task ResolveShouldResolveValid()
    {
        // Arrange
        var userInquiry = GetUserInquiryEntity();

        var managerId = 1;

        var expected = GetUserInquiryEntity(UserInquiryStatus.Rejected, managerId);

        _identityContract.Setup(x => x.GetHigherManagerIdsIncludingChiefs(userInquiry.User.UserId, CancellationToken.None))
            .ReturnsAsync([managerId]);

        _inquiriesDbContext.SetupGet(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity
                {
                    UserId = managerId,
                    LastName = "LastManager",
                    FirstName = "FirstManager",
                    Email = "EmailManager",
                    Position = PortalRoleNames.HeadOfDepartment,
                }
            }.BuildMockDbSet().Object);

        _remainingBalanceDaysHoursResolver.Setup(x => x.ResolveAsync(userInquiry, CancellationToken.None))
            .Callback(() => userInquiry.HistoricalRemainingDays = 0);

        // Act
        await _resolver.ResolveAsync(userInquiry, CancellationToken.None);

        // Assert
        userInquiry.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ResolveWhenPrevStatusIsApprovedShouldCreateAccrualTransaction()
    {
        // Arrange
        var userInquiry = GetUserInquiryEntity(UserInquiryStatus.Approved, totalAccountantDays: 123, inquiryTemplateId: 1,
            inquiryTemplate: new InquiryTemplateEntity
            {
                Id = 1,
                InquiryType = InquiryType.Vacation,
                Name = "Vacation"
            });        

        var managerId = 1;

        var expected = GetUserInquiryEntity(UserInquiryStatus.Rejected, managerId, totalAccountantDays: 123, inquiryTemplateId: 1,
            inquiryTemplate: new InquiryTemplateEntity
            {
                Id = 1,
                InquiryType = InquiryType.Vacation,
                Name = "Vacation"
            });        

        _identityContract.Setup(x => x.GetHigherManagerIdsIncludingChiefs(userInquiry.User.UserId, CancellationToken.None))
            .ReturnsAsync([managerId]);

        var mockDbSet = Array.Empty<UserAgreementTransactionEntity>().BuildMockDbSet();
        _inquiriesDbContext.SetupGet(x => x.UserAgreementTransactions)
            .Returns(mockDbSet.Object);

        _inquiriesDbContext.SetupGet(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity
                {
                    UserId = managerId,
                    LastName = "LastManager",
                    FirstName = "FirstManager",
                    Email = "EmailManager",
                    Position = PortalRoleNames.HeadOfDepartment,
                }
            }.BuildMockDbSet().Object);

        _remainingBalanceDaysHoursResolver.Setup(x => x.ResolveAsync(userInquiry, CancellationToken.None))
            .Callback(() => userInquiry.HistoricalRemainingDays = 0);

        // Act
        await _resolver.ResolveAsync(userInquiry, CancellationToken.None);

        // Assert
        userInquiry.Should().BeEquivalentTo(expected);
        mockDbSet.Verify(x => x.Add(It.IsAny<UserAgreementTransactionEntity>()), Times.Once);
    }

    [Fact]
    public async Task ResolveOwnInquiryShouldResolveValid()
    {
        // Arrange
        var userInquiry = GetUserInquiryEntity();

        var managerId = 1;

        var expected = GetUserInquiryEntity(UserInquiryStatus.Canceled, managerId);

        _identityContract.Setup(x => x.GetHigherManagerIdsIncludingChiefs(userInquiry.User.UserId, CancellationToken.None))
            .ReturnsAsync([managerId]);

        _inquiriesDbContext.SetupGet(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity>
            {
                new InquiriesUserEntity
                {
                    UserId = managerId,
                    LastName = "LastManager",
                    FirstName = "FirstManager",
                    Email = "EmailManager",
                    Position = PortalRoleNames.HeadOfDepartment,
                }
            }.BuildMockDbSet().Object);

        _remainingBalanceDaysHoursResolver.Setup(x => x.ResolveAsync(userInquiry, CancellationToken.None))
            .Callback(() => userInquiry.HistoricalRemainingDays = 0);

        // Act
        await _resolver.ResolveOwnInquiryAsync(userInquiry, CancellationToken.None);

        // Assert
        userInquiry.Should().BeEquivalentTo(expected);
    }

    private UserInquiryEntity GetUserInquiryEntity(UserInquiryStatus status = UserInquiryStatus.Open, int managerId = 0,
        int totalAccountantDays = 0, int inquiryTemplateId = 0, InquiryTemplateEntity inquiryTemplate = null)
    {
        return new UserInquiryEntity
        {
            Id = 1,
            Status = status,
            User = new InquiriesUserEntity
            {
                Position = PortalRoleNames.Employee,
                LastName = "Last",
                FirstName = "First",
                Email = "Email"
            },
            CreatedByUser = new InquiriesUserEntity
            {
                Position = PortalRoleNames.GroupManager,
                LastName = "Created",
                FirstName = "By",
                Email = "EmailBy"
            },
            HistoricalManagerId = managerId,
            HistoricalManagerPosition = PortalRoleNames.HeadOfDepartment,
            HistoricalCreatedByUserPosition = PortalRoleNames.GroupManager,
            HistoricalUserPosition = PortalRoleNames.Employee,
            HistoricalRemainingDays = 0,
            TotalAccountantDays = totalAccountantDays,
            InquiryTemplateId = inquiryTemplateId,
            InquiryTemplate = inquiryTemplate
        };
    }
}
