using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Users;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Users.Requests;

public class GetMerchUsersRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IMerchDbContext> _merchDbContextMock = new();
    private readonly CancellationToken _cancellationToken;
    private readonly GetMerchUsersRequest _handler;

    public GetMerchUsersRequestTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetMerchUsersRequest(
            _authenticatedUserContextMock.Object,
            _merchDbContextMock.Object);
    }

    [Fact]
    public async Task GetMerchUsersLookupWhenUserDontHaveManageTransactionsClaimShouldReturnEmptyArray()
    {
        //Arrange
        var users = new List<MerchUserEntity>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                Email = "example@example.com",
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                Email = "example@example.com",
            }
        };

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([]);

        _merchDbContextMock.Setup(x => x.MerchUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetMerchUsersRequest.Query(), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetMerchUsersLookupWhenUserHaveManageTransactionsClaimShouldReturnAllUsers()
    {
        //Arrange
        var users = new List<MerchUserEntity>
        {
            new()
            {
                UserId = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                Email = "example@example.com",
                IsActive = true,
            },
            new()
            {
                UserId = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                Email = "example@example.com",
                IsActive = true,
            }
        };

        _authenticatedUserContextMock.Setup(x => x.HasClaim(MerchClaimTypeNames.ManageMerchTransactions.Name)).Returns(true);

        _merchDbContextMock.Setup(x => x.MerchUsers)
            .Returns(users.BuildMockDbSet().Object);

        //Act
        var result = await _handler.Handle(
            new GetMerchUsersRequest.Query(), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        result.Count.Should().Be(2);
    }
}
