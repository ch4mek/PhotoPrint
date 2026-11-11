using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.Queries;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Requests;

public class NotifyInquiryApproverRequestTests
{
    private readonly NotifyInquiryApproverRequest _handler;

    private readonly Mock<IInquiriesDbContext> _inquiriesDbContext = new();
    private readonly Mock<IGetInquiryAcceptsByInquiryIdDbQuery> _getInquiryAcceptsByIdQuery = new();
    private readonly Mock<IGetAbsentApproversResolver> _getAbsentApproversResolver = new();
    private readonly Mock<IGetSubstituteResolver> _getSubstituteResolver = new();
    private readonly Mock<IHasUserAccessBusinessRule> _hasUserAccessBusinessRule = new();
    private readonly Mock<INotificationSender> _notificationSender = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly AccessOptions _accessOptions =
        AccessOptions.Manager | AccessOptions.Approver | AccessOptions.Substitute | AccessOptions.Hr | AccessOptions.RequestViewer;

    public NotifyInquiryApproverRequestTests()
    {
        _handler = new(
            _inquiriesDbContext.Object,
            _getInquiryAcceptsByIdQuery.Object,
            _getAbsentApproversResolver.Object,
            _getSubstituteResolver.Object,
            _hasUserAccessBusinessRule.Object,
            _notificationSender.Object);
    }

    [Fact]
    public async Task HandleWhenInquiryAcceptIsNotFoundThrowEntityNotFound()
    {
        // Arrange
        var userInquiryId = 1;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Id = 2,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken)).ReturnsAsync(accepts);
        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 4);

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserHasNoAccessToInquiryThrowActionNotAllowed()
    {
        // Arrange
        var userInquiryId = 1;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Id = 2,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        var userInquiry = new UserInquiryEntity
        {
            Id = userInquiryId,
            UserId = 145,
            InquiryTemplateId = 1234,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1234,
                Name = "Test",
                InquiryType = InquiryType.OtherType
            },
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken))
            .ReturnsAsync(accepts);

        _hasUserAccessBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userInquiryId, It.IsAny<int>(), _accessOptions, _cancellationToken))
            .ReturnsAsync(false);

        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 2);

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Theory]
    [InlineData(UserInquiryAcceptStatus.Rejected)]
    [InlineData(UserInquiryAcceptStatus.Accepted)]
    public async Task HandleWhenAcceptStatusIsApprovedOrRejectedThrowActionNotAllowed(UserInquiryAcceptStatus status)
    {
        // Arrange
        var userInquiryId = 1;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
                Id = 2,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        var userInquiry = new UserInquiryEntity
        {
            Id = userInquiryId,
            UserId = 145,
            InquiryTemplateId = 1234,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1234,
                Name = "Test",
                InquiryType = InquiryType.OtherType
            },
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken))
            .ReturnsAsync(accepts);

        _hasUserAccessBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userInquiryId, It.IsAny<int>(), _accessOptions, _cancellationToken))
            .ReturnsAsync(true);

        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 2);

        // Act
        var act = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenApproveIsNotAbsentShouldNotifyOnlyApprover()
    {
        // Arrange
        var userInquiryId = 1;
        var approverId = 12;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
                Id = 2,
                UserInquiryId = userInquiryId,
                AcceptUserId = approverId
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken))
            .ReturnsAsync(accepts);

        _hasUserAccessBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userInquiryId, It.IsAny<int>(), _accessOptions, _cancellationToken))
            .ReturnsAsync(true);

        var userInquiry = new UserInquiryEntity
        {
            Id = userInquiryId,
            UserId = 145,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Name = "Test",
                InquiryType = InquiryType.OtherType
            },
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(new[] { approverId }, _cancellationToken))
            .ReturnsAsync([]);

        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 2);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(1);

        _notificationSender
            .Verify(x => x.SendInquiryAcceptNotification(userInquiry.UserId,
                userInquiryId, new[] { approverId }, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenApproveIsAbsentAndSubstituteNotFoundShouldNotifyOnlyApprover()
    {
        // Arrange
        var userInquiryId = 1;
        var approverId = 12;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
                Id = 2,
                UserInquiryId = userInquiryId,
                AcceptUserId = approverId,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken))
            .ReturnsAsync(accepts);

        _hasUserAccessBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userInquiryId, It.IsAny<int>(), _accessOptions, _cancellationToken))
            .ReturnsAsync(true);

        var userInquiry = new UserInquiryEntity
        {
            Id = userInquiryId,
            UserId = 145,
            InquiryTemplateId = 1234,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1234,
                Name = "Test",
                InquiryType = InquiryType.OtherType
            },
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(new[] { approverId }, _cancellationToken))
            .ReturnsAsync([approverId]);

        _getSubstituteResolver
            .Setup(x => x.ResolveAsync(userInquiry.InquiryTemplate.Id, userInquiry.UserId,
                accepts[1].AcceptPolicyScope, accepts[1].AcceptUserId, _cancellationToken))
            .ReturnsAsync((int?)null);

        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 2);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(1);

        _notificationSender
            .Verify(x => x.SendInquiryAcceptNotification(userInquiry.UserId,
                userInquiryId, new[] { approverId }, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenApproveIsAbsentAndSubstituteExistsShouldNotifyApproverAndSubstitute()
    {
        // Arrange
        var userInquiryId = 1;
        var approverId = 12;
        var substituteId = 13;
        var accepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Id = 1,
                UserInquiryId = userInquiryId,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
                Id = 2,
                UserInquiryId = userInquiryId,
                AcceptUserId = approverId,
                AcceptPolicyScope = InquiryAcceptPolicyScopes.ProjectManager,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                UserInquiryId = userInquiryId,
            }
        };

        _getInquiryAcceptsByIdQuery.Setup(x => x.ExecuteAsync(userInquiryId, _cancellationToken))
            .ReturnsAsync(accepts);

        _hasUserAccessBusinessRule.Setup(x => x.IsCorrespondsToBusiness(userInquiryId, It.IsAny<int>(), _accessOptions, _cancellationToken))
            .ReturnsAsync(true);

        var userInquiry = new UserInquiryEntity
        {
            Id = userInquiryId,
            UserId = 145,
            InquiryTemplateId = 1234,
            InquiryTemplate = new InquiryTemplateEntity
            {
                Id = 1234,
                Name = "Test",
                InquiryType = InquiryType.OtherType
            },
        };

        _inquiriesDbContext.SetupGet(x => x.UserInquiries)
            .Returns(new List<UserInquiryEntity> { userInquiry }.BuildMockDbSet().Object);

        _getAbsentApproversResolver.Setup(x => x.ResolveAsync(new[] { approverId }, _cancellationToken))
            .ReturnsAsync([approverId]);

        _getSubstituteResolver
            .Setup(x => x.ResolveAsync(userInquiry.InquiryTemplate.Id, userInquiry.UserId,
                accepts[1].AcceptPolicyScope, accepts[1].AcceptUserId, _cancellationToken))
            .ReturnsAsync(substituteId);

        var command = new NotifyInquiryApproverRequest.Command(userInquiryId, 2);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().Be(1);

        _notificationSender
            .Verify(x => x.SendInquiryAcceptNotification(userInquiry.UserId,
                userInquiryId, new[] { approverId, substituteId }, _cancellationToken),
            Times.Once);
    }
}
