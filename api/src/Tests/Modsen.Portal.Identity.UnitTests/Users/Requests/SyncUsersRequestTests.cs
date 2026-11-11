using Bogus;
using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class SyncUsersRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Faker<User> _userFaker = new();

    private readonly SyncUsersRequest _handler;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public SyncUsersRequestTests()
    {
        _handler = new SyncUsersRequest(
            _identityDbContextMock.Object,
            _communicationBusMock.Object);

        _userFaker.CustomInstantiator(_ => new User()
        {
            FirstName = "test",
            LastName = "test",
            UserRoles = new List<UserRole>
            { { new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Test" } } } }
        });
    }

    [Fact]
    public async Task HandleShouldPublishEvents()
    {
        // Arrange
        var users = new List<User>
        {
            _userFaker.Generate(),
            _userFaker.Generate(),
            _userFaker.Generate(),
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(
            new SyncUsersRequest.Command(),
            _cancellationToken);

        // Assert
        result.Should().Be(users.Count);

        _communicationBusMock.Verify(x => x.PublishAsync(
                It.Is<UserSyncEvent[]>(x => x.Length == users.Count),
                _cancellationToken),
            Times.Once);
    }
}
