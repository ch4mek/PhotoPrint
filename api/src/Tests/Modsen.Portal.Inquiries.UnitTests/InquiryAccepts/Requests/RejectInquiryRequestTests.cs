using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Requests;

public class RejectInquiryRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IUserInquiryAcceptStatusComposer> _userInquiryAcceptStatusComposerMock = new();
    private readonly Mock<IInquiryAcceptLoader> _inquiryAcceptLoaderMock = new();
    private readonly Mock<IUserInquiryAcceptToApproveRejectResolver> _userInquiryAcceptToApproveRejectResolver = new();
    private readonly Mock<INotificationSender> _notificationSender = new();
    private readonly Mock<IInquiryRejectedStatusResolver> _inquiryRejectedStatusResolverMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly RejectInquiryRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;

    public RejectInquiryRequestTests()
    {
        _handler = new RejectInquiryRequest(
            _notificationSender.Object,
            _inquiriesDbContextMock.Object,
            _userInquiryAcceptStatusComposerMock.Object,
            _inquiryAcceptLoaderMock.Object,
            _inquiryRejectedStatusResolverMock.Object,
            _userInquiryAcceptToApproveRejectResolver.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task RejectInquiryRequestShouldRejectInquiry()
    {
        // Arrange
        var inquiryId = 1;
        var approverId = 999;

        var userInquiryAccepts = GetUserInquiryAccepts(inquiryId, approverId);

        var userInquiry = GetUserInquiryEntity(inquiryId, 1000, userInquiryAccepts, UserInquiryStatus.Open);

        var dto = new UserInquiryAcceptCoreDto
        {
            Id = approverId,
            Status = UserInquiryAcceptStatus.Rejected
        };

        var manager = GetInquiriesUserEntity();

        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _userInquiryAcceptStatusComposerMock
            .Setup(x => x.Compose(userInquiryAccepts.First(), UserInquiryAcceptStatus.Rejected))
            .Callback(() =>
            {
                userInquiryAccepts[0].Status = UserInquiryAcceptStatus.Rejected;
                userInquiryAccepts[0].RejectedById = approverId;
                userInquiryAccepts[0].RejectedAtUtc = DateTime.UtcNow;
            });

        _inquiryAcceptLoaderMock
            .Setup(x => x.Load(new[] { inquiryId }, _token))
            .ReturnsAsync([dto]);

        var rejectedUserInquiry = GetUserInquiryEntity(inquiryId, 1000, userInquiryAccepts, UserInquiryStatus.Rejected);

        _inquiryRejectedStatusResolverMock
            .Setup(x => x.ResolveAsync(userInquiry, _token))
            .ReturnsAsync(rejectedUserInquiry);

        _inquiriesDbContextMock
            .SetupGet(x => x.InquiriesUsers)
            .Returns(new List<InquiriesUserEntity> { manager }.BuildMockDbSet().Object);

        _userInquiryAcceptToApproveRejectResolver
            .Setup(x => x.ResolveAsync(It.IsAny<UserInquiryEntity>(), UserInquiryAcceptStatus.Rejected, _token))
            .ReturnsAsync([userInquiryAccepts[0]]);

        var command = new RejectInquiryRequest.Command(inquiryId);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Should().BeEquivalentTo([dto]);

        _inquiryRejectedStatusResolverMock
            .Verify(x => x.ResolveAsync(userInquiry, It.IsAny<CancellationToken>()), Times.Once);

        userInquiryAccepts[1].Status.Should().BeOneOf(UserInquiryAcceptStatus.Cancelled);
        userInquiryAccepts[2].Status.Should().BeOneOf(UserInquiryAcceptStatus.Cancelled);
    }

    [Fact]
    public async Task RejectInquiryRequestWhenRejectOwnInquiryShouldCancellInquiry()
    {
        // Arrange
        var inquiryId = 1;
        var approverId = 999;

        var userInquiryAccepts = GetUserInquiryAccepts(inquiryId, approverId);

        var initiator = GetInquiriesUserEntity();

        var userInquiry = GetUserInquiryEntity(inquiryId, initiator.UserId, userInquiryAccepts, UserInquiryStatus.Open);
        var cancelledUserInquiry = GetUserInquiryEntity(inquiryId, 1000, userInquiryAccepts, UserInquiryStatus.Canceled);

        var dto = new UserInquiryAcceptCoreDto
        {
            Id = approverId,
            Status = UserInquiryAcceptStatus.Cancelled
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiator.UserId);

        _userInquiryAcceptToApproveRejectResolver
            .Setup(x => x.ResolveAsync(It.IsAny<UserInquiryEntity>(), UserInquiryAcceptStatus.Rejected, _token))
            .ReturnsAsync([userInquiryAccepts[0]]);

        _userInquiryAcceptStatusComposerMock
            .Setup(x => x.Compose(userInquiryAccepts.First(), UserInquiryAcceptStatus.Cancelled))
            .Callback(() =>
            {
                userInquiryAccepts[0].Status = UserInquiryAcceptStatus.Cancelled;
                userInquiryAccepts[0].RejectedById = approverId;
                userInquiryAccepts[0].RejectedAtUtc = DateTime.UtcNow;
            });

        _inquiryRejectedStatusResolverMock
            .Setup(x => x.ResolveOwnInquiryAsync(userInquiry, _token))
            .ReturnsAsync(cancelledUserInquiry);

        _inquiryAcceptLoaderMock
            .Setup(x => x.Load(new[] { inquiryId }, _token))
            .ReturnsAsync([dto]);

        var command = new RejectInquiryRequest.Command(inquiryId);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Should().BeEquivalentTo([dto]);
        _inquiryRejectedStatusResolverMock
            .Verify(x => x.ResolveOwnInquiryAsync(userInquiry, It.IsAny<CancellationToken>()), Times.Once);
        userInquiryAccepts[1].Status.Should().BeOneOf(UserInquiryAcceptStatus.Cancelled);
        userInquiryAccepts[2].Status.Should().BeOneOf(UserInquiryAcceptStatus.Cancelled);
    }

    private List<UserInquiryAcceptEntity> GetUserInquiryAccepts(int inquiryId, int firstAcceptUserId)
    {
        return new List<UserInquiryAcceptEntity>
        {
            new() {
                Id = 1,
                UserInquiryId = inquiryId,
                AcceptUserId = firstAcceptUserId,
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new() {
                Id = 2,
                UserInquiryId = inquiryId,
                AcceptUserId = 2,
                Status = UserInquiryAcceptStatus.Pending,
            },
            new() {
                Id = 3,
                UserInquiryId = inquiryId,
                AcceptUserId = 3,
                Status = UserInquiryAcceptStatus.Pending,
            },
        };
    }

    private UserInquiryEntity GetUserInquiryEntity(int inquiryId, int userId, List<UserInquiryAcceptEntity> userInquiryAccepts, UserInquiryStatus status)
    {
        return new UserInquiryEntity
        {
            Id = inquiryId,
            UserId = userId,
            Status = status,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
            },
            UserInquiryAccepts = userInquiryAccepts,
        };
    }

    private InquiriesUserEntity GetInquiriesUserEntity()
    {
        return new InquiriesUserEntity
        {
            UserId = 11,
            Email = "email",
            FirstName = "Manager",
            LastName = "Manager",
            Position = PortalRoleNames.GroupManager
        };
    }
}
