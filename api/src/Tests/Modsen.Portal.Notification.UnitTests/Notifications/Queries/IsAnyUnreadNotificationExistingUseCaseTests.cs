using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Requests;
using Moq;

namespace Modsen.Portal.Notification.UnitTests.Notifications.Queries;

public class IsAnyUnreadNotificationExistingUseCaseTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IsAnyUnreadNotificationExistingRequest _useCase;

    public IsAnyUnreadNotificationExistingUseCaseTests()
    {
        _useCase = new IsAnyUnreadNotificationExistingRequest(
            new Mock<INotificationDbContext>().Object,
            _authenticatedUserContext.Object);
    }

    [Fact]
    public async Task HandleWhenSpecifiedWrongUserIdShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;
        var query = new IsAnyUnreadNotificationExistingRequest.Query(userId, NotificationTypes: null);

        _authenticatedUserContext.Setup(context => context.UserId).Returns(2);

        //Act
        var response = () => _useCase.Handle(query, _cancellationToken);

        //Assert
        var exception = await Assert.ThrowsAsync<ActionNotAllowedException>(response);

        exception.Message.Should().BeEquivalentTo("You can't get another user's notifications");

        _authenticatedUserContext.Verify(context => context.UserId, Times.Once);
    }
}
