using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
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

public class AcceptInquiryRequestTests
{
    private readonly Mock<IInquiriesDbContext> _inquiriesDbContextMock = new();
    private readonly Mock<IUserInquiryAcceptStatusComposer> _userInquiryAcceptStatusComposer = new();
    private readonly Mock<INotificationSender> _notificationSenderMock = new();
    private readonly Mock<IInquiryAcceptLoader> _inquiryAcceptLoaderMock = new();
    private readonly Mock<IInquiryAcceptedStatusResolver> _inquiryAcceptedStatusResolver = new();
    private readonly Mock<INextApproversToNotifyResolver> _nextApproversToNotifyResolver = new();
    private readonly Mock<IUserInquiryAcceptToApproveRejectResolver> _userInquiryAcceptToApproveRejectResolver = new();

    private readonly AcceptInquiryRequest _handler;

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<INotifySubstitutesOnApproverAbsenceService> _notifySubstitutesOnApproverAbsenceServiceMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    public AcceptInquiryRequestTests()
    {
        _handler = new AcceptInquiryRequest(
            _inquiriesDbContextMock.Object,
            _userInquiryAcceptStatusComposer.Object,
            _notificationSenderMock.Object,
            _inquiryAcceptLoaderMock.Object,
            _inquiryAcceptedStatusResolver.Object,
            _userInquiryAcceptToApproveRejectResolver.Object,
            _nextApproversToNotifyResolver.Object,
            _notifySubstitutesOnApproverAbsenceServiceMock.Object,
            _clockServiceMock.Object
            );
    }

    [Fact]
    public async Task HandleShouldAcceptInquiry()
    {
        // Arrange
        var inquiryId = 1;
        var userId = 999;
        var approverId = 1;
        var nextApproverId = 2;
        var inquiryUser = 10;

        var userInquiryAccepts = new List<UserInquiryAcceptEntity>
        {
            new() {
                Id = 1,
                UserInquiryId = inquiryId,
                AcceptUserId = approverId,
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new() {
                Id = 2,
                UserInquiryId = inquiryId,
                AcceptUserId = nextApproverId,
                Status = UserInquiryAcceptStatus.Pending,
            },
            new() {
                Id = 3,
                UserInquiryId = inquiryId,
                AcceptUserId = 3,
                Status = UserInquiryAcceptStatus.Pending,
            },
        };

        var userInquiry = new UserInquiryEntity
        {
            Id = inquiryId,
            UserId = inquiryUser,
            Status = UserInquiryStatus.Open,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
            },
            UserInquiryAccepts = userInquiryAccepts,
        };

        var dto = new UserInquiryAcceptCoreDto
        {
            Id = approverId,
            Status = UserInquiryAcceptStatus.Accepted,
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        var userInquiryAcceptToApprove = new UserInquiryAcceptEntity
        {
            Id = 1,
            UserInquiryId = inquiryId,
            AcceptUserId = approverId,
            Status = UserInquiryAcceptStatus.Accepted,
            AcceptedAtUtc = DateTime.UtcNow,
            AcceptedById = userId
        };

        _userInquiryAcceptStatusComposer
            .Setup(x => x.Compose(userInquiryAccepts.First(), UserInquiryAcceptStatus.Accepted))
            .Callback(() => userInquiryAccepts[0] = userInquiryAcceptToApprove);

        _inquiryAcceptLoaderMock
            .Setup(x => x.Load(new[] { inquiryId }, _token))
            .ReturnsAsync([dto]);

        _inquiryAcceptedStatusResolver
            .Setup(x => x.ResolveAsync(userInquiry, _token));

        _userInquiryAcceptToApproveRejectResolver
            .Setup(x => x.ResolveAsync(It.IsAny<UserInquiryEntity>(), UserInquiryAcceptStatus.Accepted, _token))
            .ReturnsAsync([userInquiryAccepts[0]]);

        _nextApproversToNotifyResolver
            .Setup(x => x.ResolveAsync(new List<UserInquiryAcceptEntity> { userInquiryAccepts[1] }, userInquiry, _token))
            .ReturnsAsync([userInquiryAccepts[1].AcceptUserId]);

        var command = new AcceptInquiryRequest.Command(inquiryId);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Should().BeEquivalentTo([dto]);

        userInquiryAccepts[1].Status.Should().BeOneOf(UserInquiryAcceptStatus.WaitingForApprove);
        userInquiryAccepts[2].Status.Should().BeOneOf(UserInquiryAcceptStatus.Pending);

        _notificationSenderMock.Verify(x =>
                x.SendInquiryAcceptNotification(
                    inquiryUser,
                    inquiryId,
                    new[] { nextApproverId },
                    _token),
            Times.Once);
    }

    [Fact]
    public async Task HandleShouldThrowEntityNotFoundExceptionWhenInquiryNotFound()
    {
        // Arrange
        var inquiryId = 1;

        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);


        var command = new AcceptInquiryRequest.Command(inquiryId);

        //Act
        var actionUnderTest = () => _handler.Handle(command, _token);

        //Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }


    [Fact]
    public async Task HandleShouldAcceptInquiryWhenProjectManagerIsApprover()
    {
        // Arrange
        var inquiryId = 1;

        var inquiryUser = 10;

        var userInquiryAccepts = new List<UserInquiryAcceptEntity>
        {
            new() {
                Id = 1,
                UserInquiryId = inquiryId,
                AcceptUserId = 1,
                Status = UserInquiryAcceptStatus.Accepted,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new() {
                Id = 2,
                UserInquiryId = inquiryId,
                AcceptUserId = 2,
                Status = UserInquiryAcceptStatus.WaitingForApprove,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new() {
                Id = 3,
                UserInquiryId = inquiryId,
                AcceptUserId = 3,
                Status = UserInquiryAcceptStatus.Pending,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.GroupManager,
            },
        };

        var userInquiry = new UserInquiryEntity
        {
            Id = inquiryId,
            UserId = inquiryUser,
            Status = UserInquiryStatus.Open,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Vacation",
            },
            UserInquiryAccepts = userInquiryAccepts,
        };

        var dto = new UserInquiryAcceptCoreDto
        {
            Id = 3,
            Status = UserInquiryAcceptStatus.Accepted,
        };

        _inquiriesDbContextMock
            .Setup(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        var userInquiryAcceptToApprove = new UserInquiryAcceptEntity
        {
            Id = 2,
            UserInquiryId = inquiryId,
            AcceptUserId = 2,
            Status = UserInquiryAcceptStatus.Accepted,
            AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            AcceptedById = 3,
            AcceptedAtUtc = DateTime.UtcNow
        };

        _userInquiryAcceptStatusComposer
            .Setup(x => x.Compose(new List<UserInquiryAcceptEntity> { userInquiryAccepts[1] }, UserInquiryAcceptStatus.Accepted))
            .Callback(() => userInquiryAccepts[1] = userInquiryAcceptToApprove);

        _inquiryAcceptLoaderMock
            .Setup(x => x.Load(new[] { 2 }, _token))
            .ReturnsAsync([dto]);

        _inquiryAcceptedStatusResolver
            .Setup(x => x.ResolveAsync(userInquiry, _token));

        _userInquiryAcceptToApproveRejectResolver
            .Setup(x => x.ResolveAsync(It.IsAny<UserInquiryEntity>(), UserInquiryAcceptStatus.Accepted, _token))
            .ReturnsAsync([userInquiryAccepts[1]]);

        _nextApproversToNotifyResolver
            .Setup(x => x.ResolveAsync(new[] { userInquiryAccepts[2] }, userInquiry, _token))
            .ReturnsAsync([userInquiryAccepts[2].AcceptUserId]);

        var command = new AcceptInquiryRequest.Command(inquiryId);

        //Act
        var result = await _handler.Handle(command, _token);

        //Assert
        result.Should().BeEquivalentTo([dto]);

        userInquiryAccepts[0].Status.Should().Be(UserInquiryAcceptStatus.Accepted);
        userInquiryAccepts[1].Status.Should().Be(UserInquiryAcceptStatus.Accepted);
        userInquiryAccepts[2].Status.Should().Be(UserInquiryAcceptStatus.WaitingForApprove);

        _notificationSenderMock.Verify(x =>
                x.SendInquiryAcceptNotification(
                    inquiryUser,
                    inquiryId,
                    new[] { 3 },
                    _token),
            Times.Once);
    }
}
