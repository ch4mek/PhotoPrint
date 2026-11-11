using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Composers;

public class UserInquiryAcceptStatusComposerTests
{
    private readonly UserInquiryAcceptStatusComposer _composer;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IClockService> _clockService = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly int _authenticatedUserId = 1000;
    private readonly DateTime _utcNow = DateTime.UtcNow;

    public UserInquiryAcceptStatusComposerTests()
    {
        _composer = new(_authenticatedUserContext.Object, _clockService.Object);
        _clockService.Setup(x => x.UtcNow).Returns(_utcNow);
        _authenticatedUserContext.Setup(x => x.UserId).Returns(_authenticatedUserId);
        _authenticatedUserContext.Setup(x => x.GetRequiredUserId()).Returns(_authenticatedUserId);
    }

    [Fact]
    public void ComposeSingleWhenStatusIsAcceptedReturnAcceptedFieldsFilled()
    {
        // Arrange
        var status = UserInquiryAcceptStatus.Accepted;
        var userInquiryAccept = new UserInquiryAcceptEntity
        {
            Status = UserInquiryAcceptStatus.WaitingForApprove,
        };

        var expected = new UserInquiryAcceptEntity
        {
            Status = status,
            AcceptedAtUtc = _utcNow,
            AcceptedById = _authenticatedUserId,
        };

        // Act
        _composer.Compose(userInquiryAccept, status);

        // Assert
        userInquiryAccept.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ComposeSingleWhenStatusIsRejectedReturnRejectedFieldsFilled()
    {
        // Arrange
        var status = UserInquiryAcceptStatus.Rejected;
        var userInquiryAccept = new UserInquiryAcceptEntity
        {
            Status = UserInquiryAcceptStatus.WaitingForApprove,
        };

        var expected = new UserInquiryAcceptEntity
        {
            Status = status,
            RejectedAtUtc = _utcNow,
            RejectedById = _authenticatedUserId,
        };

        // Act
        _composer.Compose(userInquiryAccept, status);

        // Assert
        userInquiryAccept.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(UserInquiryAcceptStatus.Pending)]
    [InlineData(UserInquiryAcceptStatus.WaitingForApprove)]
    public void ComposeSingleWhenStatusIsOtherReturnStatusFilled(UserInquiryAcceptStatus status)
    {
        // Arrange
        var userInquiryAccept = new UserInquiryAcceptEntity
        {
            Status = UserInquiryAcceptStatus.WaitingForApprove,
        };

        var expected = new UserInquiryAcceptEntity
        {
            Status = status,
        };

        // Act
        _composer.Compose(userInquiryAccept, status);

        // Assert
        userInquiryAccept.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ComposeMultipleWhenStatusIsAcceptedReturnAcceptedFieldsFilled()
    {
        // Arrange
        var status = UserInquiryAcceptStatus.Accepted;
        var userInquiryAccepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            }
        };

        var expected = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = status,
                AcceptedAtUtc = _utcNow,
                AcceptedById = _authenticatedUserId,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
                AcceptedAtUtc = _utcNow,
                AcceptedById = _authenticatedUserId,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
                AcceptedAtUtc = _utcNow,
                AcceptedById = _authenticatedUserId,
            },
        };

        // Act
        _composer.Compose(userInquiryAccepts, status);

        // Assert
        userInquiryAccepts.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ComposeMultipleWhenStatusIsRejectedReturnRejectedFieldsFilled()
    {
        // Arrange
        var status = UserInquiryAcceptStatus.Rejected;
        var userInquiryAccepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            }
        };

        var expected = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = status,
                RejectedAtUtc = _utcNow,
                RejectedById = _authenticatedUserId,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
                RejectedAtUtc = _utcNow,
                RejectedById = _authenticatedUserId,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
                RejectedAtUtc = _utcNow,
                RejectedById = _authenticatedUserId,
            },
        };

        // Act
        _composer.Compose(userInquiryAccepts, status);

        // Assert
        userInquiryAccepts.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(UserInquiryAcceptStatus.Pending)]
    [InlineData(UserInquiryAcceptStatus.WaitingForApprove)]
    public void ComposeMultipleWhenStatusIsOtherReturnStatusFilled(UserInquiryAcceptStatus status)
    {
        // Arrange
        var userInquiryAccepts = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.WaitingForApprove,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Accepted,
            },
            new UserInquiryAcceptEntity
            {
                Status = UserInquiryAcceptStatus.Rejected,
            }
        };

        var expected = new List<UserInquiryAcceptEntity>
        {
            new UserInquiryAcceptEntity
            {
                Status = status,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
            },
            new UserInquiryAcceptEntity
            {
                Status = status,
            }
        };

        // Act
        _composer.Compose(userInquiryAccepts, status);

        // Assert
        userInquiryAccepts.Should().BeEquivalentTo(expected);
    }
}
