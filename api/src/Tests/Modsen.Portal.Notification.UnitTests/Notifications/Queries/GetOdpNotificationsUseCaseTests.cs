using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Requests;
using Modsen.Portal.Notification.UseCases.Requests.Queries;
using Moq;

namespace Modsen.Portal.Notification.UnitTests.Notifications.Queries;

public class GetOdpNotificationsUseCaseTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IMarkNotificationsAsSentDbQuery> _markNotificationsAsSentDbQueryMock = new();
    private readonly Mock<INotificationDbContext> _notificationDbContext = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetOdpNotificationsRequest _useCase;

    public GetOdpNotificationsUseCaseTests()
    {
        _useCase = new GetOdpNotificationsRequest(
            _notificationDbContext.Object,
            _authenticatedUserContext.Object,
            _markNotificationsAsSentDbQueryMock.Object);
    }

    [Fact]
    public async Task HandleWhenSpecifiedWrongUserIdShouldThrowActionNotAllowedException()
    {
        //Arrange
        var userId = 1;

        var queryOptions = new Dictionary<string, string?>
        {
            ["top"] = "10",
            ["skip"] = "0"
        };

        var oDataQueryOptions = ODataQueryOptionsMock.BuildMock<OdpNotification>(queryOptions);

        var query = new GetOdpNotificationsRequest.Query(userId, oDataQueryOptions);

        _authenticatedUserContext.Setup(context => context.UserId).Returns(2);

        //Act
        var response = () => _useCase.Handle(query, _cancellationToken);

        //Assert
        var exception = await Assert.ThrowsAsync<ActionNotAllowedException>(response);

        exception.Message.Should().BeEquivalentTo("You can't get another user's notifications");

        _authenticatedUserContext.Verify(context => context.UserId, Times.Once);
    }
}
