using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class UserInquiryAcceptToApproveRejectResolverTests
{
    private readonly UserInquiryAcceptToApproveRejectResolver _resolver;
    private readonly Mock<IAvailableInquiryAcceptsResolver> _availableInquiryAcceptsResolver = new();
    private readonly Mock<IGetInquiriesCanBeApprovedAsSubstituteResolver> _getInquiriesCanBeApprovedAsSubstituteResolver = new();
    private readonly Mock<IGetAbsentApproversResolver> _getAbsentApproversResolver = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly UserInquiryEntity s_userInquiry = new UserInquiryEntity
    {
        Id = 1,
        UserInquiryAccepts =
        [
            new UserInquiryAcceptEntity
            {
                Id = 1,
                Status = UserInquiryAcceptStatus.Accepted,
                AcceptUserId = 1,
            },
            new UserInquiryAcceptEntity
            {
                Id = 2,
                Status = UserInquiryAcceptStatus.WaitingForApprove,
                AcceptUserId = 2,
            },
            new UserInquiryAcceptEntity
            {
                Id = 3,
                Status = UserInquiryAcceptStatus.Pending,
                AcceptUserId = 3
            },
        ]
    };

    public UserInquiryAcceptToApproveRejectResolverTests()
    {
        _resolver = new(_availableInquiryAcceptsResolver.Object,
            _getInquiriesCanBeApprovedAsSubstituteResolver.Object,
            _getAbsentApproversResolver.Object);
    }

    [Fact]
    public async Task ResolveWhenCanBeAcceptedAsMainApproverShouldReturnInquiryAccept()
    {
        // Arrange
        _availableInquiryAcceptsResolver.Setup(x => x.ResolveForApproval(s_userInquiry))
            .Returns([s_userInquiry.UserInquiryAccepts[1]]);

        // Act
        var result = await _resolver.ResolveAsync(s_userInquiry, UserInquiryAcceptStatus.Accepted, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo([s_userInquiry.UserInquiryAccepts[1]]);
    }

    [Fact]
    public async Task ResolveWhenCanBeRejectedAsMainApproverShouldReturnInquiryAccept()
    {
        // Arrange
        _availableInquiryAcceptsResolver.Setup(x => x.ResolveForRejection(s_userInquiry))
            .Returns(([s_userInquiry.UserInquiryAccepts[1]], true));

        // Act
        var result = await _resolver.ResolveAsync(s_userInquiry, UserInquiryAcceptStatus.Rejected, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo([s_userInquiry.UserInquiryAccepts[1]]);
    }

    [Fact]
    public async Task ResolveWhenCannotBeAcceptedAsMainApproverAndThereIsNotSubstituteShouldThrowActionNotAllowed()
    {
        // Arrange
        _availableInquiryAcceptsResolver.Setup(x => x.ResolveForApproval(s_userInquiry))
            .Returns([]);

        _getInquiriesCanBeApprovedAsSubstituteResolver
            .Setup(x => x.ResolveAsync(new List<int> { s_userInquiry.Id }, _cancellationToken))
            .ReturnsAsync([]);

        // Act
        var act = () => _resolver.ResolveAsync(s_userInquiry, UserInquiryAcceptStatus.Accepted, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task ResolveWhenCanBeAcceptedAsSubstituteShouldReturnAccept()
    {
        // Arrange
        _availableInquiryAcceptsResolver.Setup(x => x.ResolveForApproval(s_userInquiry))
            .Returns([]);

        _getInquiriesCanBeApprovedAsSubstituteResolver
            .Setup(x => x.ResolveAsync(new List<int> { s_userInquiry.Id }, _cancellationToken))
            .ReturnsAsync([s_userInquiry.Id]);

        _getAbsentApproversResolver
            .Setup(x => x.ResolveAsync(new List<int> { s_userInquiry.UserInquiryAccepts[1].AcceptUserId }, _cancellationToken))
            .ReturnsAsync([s_userInquiry.UserInquiryAccepts[1].AcceptUserId]);

        // Act
        var result = await _resolver.ResolveAsync(s_userInquiry, UserInquiryAcceptStatus.Accepted, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo([s_userInquiry.UserInquiryAccepts[1]]);
    }

    [Theory]
    [InlineData(UserInquiryAcceptStatus.WaitingForApprove)]
    [InlineData(UserInquiryAcceptStatus.Pending)]
    public async Task ResolveWhenInvalidActionShouldThrowArgumentException(UserInquiryAcceptStatus action)
    {
        // Act
        var act = () => _resolver.ResolveAsync(s_userInquiry, action, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
