using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases.Requests;
using Moq;

namespace Modsen.Portal.Notification.UnitTests.Notifications.Commands;

public class DeleteNotificationsByUserIdUseCaseTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly DeleteNotificationsByUserIdRequest _useCase;

    public DeleteNotificationsByUserIdUseCaseTests()
    {
        _useCase = new DeleteNotificationsByUserIdRequest(
            new Mock<INotificationDbContext>().Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenNotificationUserIdIsNotEqualToCurrentUserIdShouldThrowActionNotAllowedException()
    {
        //Arrange
        var command = new DeleteNotificationsByUserIdRequest.Command(UserId: 1, NotificationTypes: null);

        _authenticatedUserContextMock.Setup(context => context.UserId)
            .Returns(2);

        //Act
        var response = () => _useCase.Handle(command, _cancellationToken);

        //Assert
        await Assert.ThrowsAsync<ActionNotAllowedException>(response);
    }
}
