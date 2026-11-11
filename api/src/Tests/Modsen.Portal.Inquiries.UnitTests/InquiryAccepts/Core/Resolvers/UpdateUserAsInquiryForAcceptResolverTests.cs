using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Moq;

namespace Modsen.Portal.Inquiries.UnitTests.InquiryAccepts.Core.Resolvers;

public class UpdateUserAsInquiryForAcceptResolverTests
{
    private readonly UpdateUserAsInquiryForAcceptResolver _resolver;

    private readonly Mock<IInquiriesDbContext> _dbContext = new();
    private readonly Mock<IUpdateUserInfoInquiryAcceptsResolver> _updateUserInfoAcceptsResolver = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public UpdateUserAsInquiryForAcceptResolverTests()
    {
        _resolver = new(_dbContext.Object, _updateUserInfoAcceptsResolver.Object);
    }

    [Fact]
    public async Task ResolveWhenRequestIsValidShouldResolveValid()
    {
        // Arrange
        var prevPosition = PortalRoleNames.Employee;
        var prevDepartmentId = "Net";
        var prevResourceGroupId = (int?)null;
        var isActive = true;

        var user = new InquiriesUserEntity
        {
            Email = "Email",
            FirstName = "first",
            LastName = "last",
            DepartmentId = "Java",
            ResourceGroupId = 1,
            Position = prevPosition,
            IsActive = isActive
        };

        var openUserInquiries = new List<UserInquiryEntity>();

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(openUserInquiries.BuildMockDbSet().Object);

        _updateUserInfoAcceptsResolver.Setup(x => x.ResolveAsync(Array.Empty<int>(), _cancellationToken));

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevResourceGroupId = prevResourceGroupId,
            PrevPosition = prevPosition,
            IsPrevActive = isActive
        };

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _updateUserInfoAcceptsResolver.Verify(x => x.ResolveAsync(Array.Empty<int>(), _cancellationToken),
            Times.Once());
    }

    [Fact]
    public async Task ResolveWhenFieldsNotUpdatedShouldNotUpdate()
    {
        // Arrange
        var prevPosition = PortalRoleNames.Employee;
        var prevDepartmentId = "Net";
        var prevResourceGroupId = (int?)null;
        var isActive = true;

        var user = new InquiriesUserEntity
        {
            Email = "Email",
            FirstName = "first",
            LastName = "last",
            DepartmentId = prevDepartmentId,
            ResourceGroupId = prevResourceGroupId,
            Position = PortalRoleNames.SystemAdministrator,
            IsActive = isActive
        };

        var openUserInquiries = new List<UserInquiryEntity>();

        _dbContext.SetupGet(x => x.UserInquiries)
            .Returns(openUserInquiries.BuildMockDbSet().Object);

        _updateUserInfoAcceptsResolver.Setup(x => x.ResolveAsync(openUserInquiries.Select(x => x.Id).ToArray(),
            _cancellationToken));

        var request = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = prevDepartmentId,
            PrevResourceGroupId = prevResourceGroupId,
            PrevPosition = prevPosition,
            IsPrevActive = isActive
        };

        // Act
        var act = () => _resolver.ResolveAsync(request, _cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
        _updateUserInfoAcceptsResolver.Verify(x => x.ResolveAsync(openUserInquiries.Select(x => x.Id).ToArray(),
                _cancellationToken),
            Times.Never());
    }
}
