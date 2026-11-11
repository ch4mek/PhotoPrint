using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.Inquiries.Core.Resolvers;

public class RejectInquiriesAfterUnassignResolverTests
{
    private readonly Mock<IInquiriesDbContext> _dbContextMock;
    private readonly Mock<IInquiryRejectedStatusResolver> _inquiryRejectedStatusResolverMock;
    private readonly Mock<IUserInquiryAcceptStatusComposer> _inquiryAcceptStatusComposerMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly RejectInquiriesAfterUnassignResolver _resolver;

    public RejectInquiriesAfterUnassignResolverTests()
    {
        _dbContextMock = new Mock<IInquiriesDbContext>();
        _inquiryRejectedStatusResolverMock = new Mock<IInquiryRejectedStatusResolver>();
        _inquiryAcceptStatusComposerMock = new Mock<IUserInquiryAcceptStatusComposer>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();

        _resolver = new RejectInquiriesAfterUnassignResolver(
            _dbContextMock.Object,
            _inquiryRejectedStatusResolverMock.Object,
            _inquiryAcceptStatusComposerMock.Object,
            _authenticatedUserContextMock.Object
        );
    }

    [Fact]
    public async Task ResolveAsyncWithEmptyUnassignUserIdsReturnsEmptyList()
    {
        // Arrange
        var unassignUserIds = Array.Empty<int>();
        var inquiryTemplateId = 1;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _resolver.ResolveAsync(unassignUserIds, inquiryTemplateId, cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveAsyncNoInquiriesToRejectReturnsEmptyList()
    {
        // Arrange
        var unassignUserIds = new[] { 1, 2 };
        var inquiryTemplateId = 1;
        var cancellationToken = CancellationToken.None;

        _dbContextMock
            .Setup(db => db.UserInquiries)
            .Returns(new List<UserInquiryEntity>().BuildMockDbSet().Object);

        // Act
        var result = await _resolver.ResolveAsync(unassignUserIds, inquiryTemplateId, cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ResolveAsyncWithInquiriesToRejectUpdatesInquiryStatus()
    {
        // Arrange
        var unassignUserIds = new[] { 1 };
        var inquiryTemplateId = 1;
        var cancellationToken = CancellationToken.None;
        var unassignUserId = 99;

        var inquiries = new List<UserInquiryEntity>
        {
            new() {
                UserId = 1,
                InquiryTemplateId = inquiryTemplateId,
                Status = UserInquiryStatus.Open,
                TotalAccountantDays = 1,
                UserInquiryAccepts =
                [
                    new UserInquiryAcceptEntity { Status = UserInquiryAcceptStatus.Pending },
                    new UserInquiryAcceptEntity { Status = UserInquiryAcceptStatus.WaitingForApprove }
                ]
            }
        };

        _dbContextMock
            .Setup(db => db.UserInquiries)
            .Returns(inquiries.BuildMockDbSet().Object);
        _authenticatedUserContextMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(unassignUserId);

        // Act
        var result = await _resolver.ResolveAsync(unassignUserIds, inquiryTemplateId, cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result[0].UserId.Should().Be(1);
        _inquiryAcceptStatusComposerMock
            .Verify(c => c.Compose(It.IsAny<UserInquiryAcceptEntity>(), unassignUserId, UserInquiryAcceptStatus.Rejected), Times.Once);
        _inquiryRejectedStatusResolverMock
            .Verify(r => r.ResolveAsync(It.IsAny<UserInquiryEntity>(), cancellationToken), Times.Once);
        _dbContextMock
            .Verify(db => db.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
