using MockQueryable.Moq;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Core;
using Modsen.Portal.Notification.UseCases.Requests;
using Moq;

namespace Modsen.Portal.Notification.UnitTests.Notifications.Commands;

public class DeleteNotificationUseCaseTests
{
    private readonly Mock<ICanUserInteractWithNotificationsBusinessRule> _canUserInteractiWithNotificationsMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>(); 
    private readonly Mock<INotificationDbContext> _notificationDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly DeleteNotificationRequest _useCase;

    public DeleteNotificationUseCaseTests()
    {

        _useCase = new DeleteNotificationRequest(
            _canUserInteractiWithNotificationsMock.Object,
            _authenticatedUserContextMock.Object,
            _notificationDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenNotificationIsNotExistingShouldThrowEntityNotFoundException()
    {
        //Arrange
        var userId = 1;

        List<int> notificationIds = [322];

        var failReason = "The notification hasn't been found";

        var command = new DeleteNotificationRequest.Command(notificationIds.First());

        _notificationDbContextMock
            .Setup(x => x.Notifications)
            .Returns(new List<NotificationEntity>() { new() { Id = 323, Arguments = "", UserId = userId } }.BuildMockDbSet().Object);

        //Act
        var response = () => _useCase.Handle(command, _cancellationToken);

        //Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(response);

        exception.Message.Should().BeEquivalentTo(failReason);
    }

    [Fact]
    public async Task HandleWhenUserCannotInteractWithNotificationsShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        List<int> notificationIds = [322];

        _notificationDbContextMock
            .Setup(x => x.Notifications)
            .Returns(new List<NotificationEntity>() { new() { Id = 322, Arguments = "", UserId = userId } }.BuildMockDbSet().Object);

        var failReason = "The notification doesn't belong to user";

        var command = new DeleteNotificationRequest.Command(notificationIds.First());

        _authenticatedUserContextMock.Setup(context => context.UserId)
            .Returns(2);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(2);

        _canUserInteractiWithNotificationsMock.Setup(businessRule => businessRule.IsCorrespondsToBusiness(
            It.Is<CanUserInteractWithNotificationsModel>(model => model.UserId == 2 &&
                model.NotificationIds.Count == 1), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail(failReason));

        //Act
        var response = () => _useCase.Handle(command, _cancellationToken);

        //Assert
        var exception = await Assert.ThrowsAsync<ActionNotAllowedException>(response);

        exception.Message.Should().BeEquivalentTo(failReason);

        _authenticatedUserContextMock.Verify(context => context.GetRequiredUserId(), Times.Once);

        _canUserInteractiWithNotificationsMock.Verify(businessRule =>
            businessRule.IsCorrespondsToBusiness(It.Is<CanUserInteractWithNotificationsModel>(
                model => model.UserId == 2 && model.NotificationIds.Count == 1), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnResponse()
    {
        //Arrange
        var userId = 1;

        List<int> notificationIds = [322];

        _notificationDbContextMock
            .Setup(x => x.Notifications)
            .Returns(new List<NotificationEntity>() { new() { Id = 322, Arguments = "", UserId = userId} }.BuildMockDbSet().Object);

        var command = new DeleteNotificationRequest.Command(notificationIds.First());

        _authenticatedUserContextMock.Setup(context => context.UserId)
            .Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _canUserInteractiWithNotificationsMock.Setup(businessRule => businessRule.IsCorrespondsToBusiness(
            It.Is<CanUserInteractWithNotificationsModel>(model => model.UserId == userId &&
                model.NotificationIds.Count == 1), _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success());

        //Act
        var response = await _useCase.Handle(command, _cancellationToken);

        //Assert
        response.Id.Should().Be(notificationIds.First());
        response.UserId.Should().Be(userId);

        _authenticatedUserContextMock.Verify(context => context.GetRequiredUserId(), Times.Once);

        _canUserInteractiWithNotificationsMock.Verify(businessRule => businessRule.IsCorrespondsToBusiness(
            It.Is<CanUserInteractWithNotificationsModel>(model =>
                model.UserId == userId && model.NotificationIds.Count == 1), _cancellationToken), Times.Once);
    }
}
