using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Core;
using Modsen.Portal.Notification.UseCases.Requests;
using Moq;

namespace Modsen.Portal.Notification.UnitTests.Notifications.Commands;

public class ReadNotificationsUseCaseTests
{
    private readonly Mock<INotificationDbContext> _notificationDbContext = new();
    private readonly Mock<ICanUserInteractWithNotificationsBusinessRule> _canUserInteractiWithNotificationsMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly ReadNotificationsRequest _useCase;

    public ReadNotificationsUseCaseTests()
    {
        _useCase = new ReadNotificationsRequest(
            _canUserInteractiWithNotificationsMock.Object,
            _authenticatedUserContextMock.Object,
            _notificationDbContext.Object);
    }

    [Fact]
    public async Task HandleWhenNotificationIsNotExistingShouldThrowEntityNotFoundException()
    {
        List<int> notificationIds = [322];

        var failReason = "Notifications haven't been found";
        var command = new ReadNotificationsRequest.Command(notificationIds);

        _notificationDbContext
            .Setup(x => x.Notifications)
            .Returns(new List<NotificationEntity>().BuildMockDbSet().Object);

        //Act
        var response = () => _useCase.Handle(command, _cancellationToken);

        //Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(response);

        exception.Message.Should().BeEquivalentTo(failReason);

        _authenticatedUserContextMock.Verify(context => context.UserId, Times.Never);

        _canUserInteractiWithNotificationsMock.Verify(businessRule => businessRule.IsCorrespondsToBusiness(
            It.IsAny<CanUserInteractWithNotificationsModel>(), _cancellationToken), Times.Never);
    }
}
