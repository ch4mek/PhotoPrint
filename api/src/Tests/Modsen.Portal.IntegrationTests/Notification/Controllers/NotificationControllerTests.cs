using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Notification.Controllers;

[Collection("NotificationCollection")]
public class NotificationControllerTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public NotificationControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    [Fact]
    public async Task GetNotificationsWhenAdminCancelBookingReturnAllNotificationsOfCurrentUser()
    {
        // Arrange
        using var engineerClient = _factory.CreateClient();
        await engineerClient.AuthAsEmployee();

        using var ctoClient = _factory.CreateClient();
        await ctoClient.AuthAsCto();

        var bookingDate = DateTime.UtcNow.AddDays(3).Date;
        var workplaceId = _factory.WorkplaceIdInMinksOffice;
        var weeksInInterval = 0;

        var bookingId = await engineerClient.CreateSelfBooking(workplaceId, bookingDate, bookingDate, weeksInInterval);

        await ctoClient.DeleteBookings([bookingId]);

        // Act
        var notifications = await engineerClient.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0);

        var notificationOutput = notifications!.Items.ToList().ConvertAll(output =>
            JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson));

        // Assert
        notificationOutput.Should().NotBeNull();
        notificationOutput.Should().Contain(x => x.WorkplaceId == workplaceId);
    }

    [Fact]
    public async Task GetNotificationsWhenEmployeeDeleteBookingReturnEmptyListOfNotifications()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var bookingDate = DateTime.UtcNow.AddDays(14).Date;
        var workplaceId = 3;
        var weeksInInterval = 0;

        var bookingId = await client.CreateSelfBooking(3, bookingDate, bookingDate, weeksInInterval);

        await client.DeleteBookings([bookingId]);

        // Act
        var notifications = await client.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0);

        var notificationOutput = notifications!.Items.ToList().ConvertAll(output =>
            JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson));

        // Assert
        notificationOutput.Should().NotBeNull();
        notificationOutput.Should().NotContain(x => x.WorkplaceId == workplaceId);
    }

    [Fact]
    public async Task GetNotificationsWhenEmployeeReedThisNotificationsReturnEmptyListOfNotifications()
    {
        // Arrange
        using var engineerClient = _factory.CreateClient();
        await engineerClient.AuthAsEmployee();

        using var ctoClient = _factory.CreateClient();
        await ctoClient.AuthAsCto();

        var bookingDate = DateTime.UtcNow.AddDays(5).Date;
        var workplaceId = _factory.WorkplaceIdInMinksOffice;
        var weeksInInterval = 0;

        var bookingId = await engineerClient.CreateSelfBooking(workplaceId, bookingDate, bookingDate, weeksInInterval);

        await ctoClient.DeleteBookings([bookingId]);

        var unreadNotifications = await engineerClient.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0, "Read eq false");

        var unreadNotificationArgs = unreadNotifications.Items.ToList().ConvertAll(output => new
        {
            NotificationId = output.Id,
            Output = JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson)
        });

        var unreadNotification = unreadNotificationArgs.Find(x => x.Output.WorkplaceId == workplaceId);

        await engineerClient.ReadNotificationsAsync([unreadNotification!.NotificationId]);

        // Act
        var notifications = await engineerClient.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0, "Read eq false");

        var notificationsArgs = notifications!.Items.ToList().ConvertAll(output =>
            JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson));

        // Assert
        notificationsArgs.Should().NotBeNull();
        notificationsArgs.Should().NotContain(x => x.WorkplaceId == workplaceId);
    }

    [Fact]
    public async Task ReadNotificationsReadUserNotificationReturnSuccessStatusCode()
    {
        // Arrange
        using var engineerClient = _factory.CreateClient();
        await engineerClient.AuthAsEmployee();

        using var ctoClient = _factory.CreateClient();
        await ctoClient.AuthAsCto();

        var bookingDate = DateTime.UtcNow.AddDays(4).Date;
        var workplaceId = 1;
        var weeksInInterval = 0;

        await engineerClient.CreateSelfBooking(workplaceId, bookingDate, bookingDate, weeksInInterval);

        var unreadNotifications = await engineerClient.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0, "Read eq false");

        var unreadNotificationArgs = unreadNotifications!.Items.ToList().ConvertAll(output => new
        {
            NotificationId = output.Id,
            Args = JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson)
        });

        var unreadNotification = unreadNotificationArgs.Find(x => x.Args.WorkplaceNumber == workplaceId);

        // Act
        var response = await engineerClient.ReadNotificationsAsync([unreadNotification!.NotificationId]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReadNotificationsReadNotificationOfOtherUserReturnForbidden()
    {
        // Arrange
        using var engineerClient = _factory.CreateClient();
        await engineerClient.AuthAsEmployee();

        using var ctoClient = _factory.CreateClient();
        await ctoClient.AuthAsCto();

        var bookingDate = DateTime.UtcNow.AddDays(5).Date;
        var workplaceId = 1;
        var weeksInInterval = 0;

        var bookingId = await engineerClient.CreateSelfBooking(workplaceId, bookingDate, bookingDate, weeksInInterval);

        await ctoClient.DeleteBookings([bookingId]);

        var unreadNotifications = await engineerClient.GetNotificationsAsync((int)_factory.EmployeeUserId!, 100, 0);

        var unreadNotificationArgs = unreadNotifications!.Items.ToList().ConvertAll(output => new
        {
            NotificationId = output.Id,
            Args = JsonSerializer.Deserialize<WorkplaceBookingCancelledNotificationOutputDto>(output.OutputJson)
        });

        var unreadNotification = unreadNotificationArgs.Find(x => x.Args.WorkplaceNumber == workplaceId);

        // Act
        var response = await ctoClient.ReadNotificationsAsync([unreadNotification!.NotificationId]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetOdpNotificationsByUserIdWhenTimeTrackReportHaveBeenCreatedShouldResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        var format = nameof(DocumentType.Excel);
        var date = DateTime.UtcNow;

        var employeeUserId = (int)_factory.GroupManagerId!;

        await client.GetReportAsync(format, date.Year, employeeUserId);

        //Act
        var deserializedResponse = await client.GetNotificationsAsync(
            employeeUserId, 100, 0,
            "IsDeleted eq false and Read eq false&orderby=Id asc");

        //Assert
        deserializedResponse!.Items.Should().HaveCount(2);
        deserializedResponse.Items.First().NotificationType.Should().Be(NotificationTypes.TimeTrackReportGenerating);
        deserializedResponse.Items.Last().NotificationType.Should().Be(NotificationTypes.TimeTrackReportCreated);
    }

    [Fact]
    public async Task IsAnyUnreadNotificationExistingWhenTimeTrackReportHaveBeenCreatedShouldResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var format = nameof(DocumentType.Excel);
        var date = DateTime.UtcNow;

        var employeeUserId = _factory.EmployeeUserId;

        await client.GetReportAsync(format, date.Year, (int)employeeUserId!);

        //Act
        var responseBeforeDeletion = await client.GetUnreadPreviewNotificationsAsync((int)employeeUserId);

        await client.DeleteNotificationsByUserIdAsync((int)employeeUserId);

        var responseAfterDeletion = await client.GetUnreadPreviewNotificationsAsync((int)employeeUserId);

        //Assert
        responseBeforeDeletion.Should().Be(true);
        responseAfterDeletion.Should().Be(false);
    }

    [Fact]
    public async Task ReadNotificationWhenTimeTrackReportHaveBeenCreatedShouldResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var format = nameof(DocumentType.Excel);
        var date = DateTime.UtcNow.AddDays(7);

        var employeeUserId = (int)_factory.EmployeeUserId!;

        await client.GetReportAsync(format, date.Year, employeeUserId);

        var response = await client.GetNotificationsAsync(
            employeeUserId, 100, 0,
            "IsDeleted eq false and Read eq false");

        List<int> notificationIds = [response!.Items.First().Id, response.Items.Last().Id];

        //Act
        var responseBeforeRead = await client.GetUnreadPreviewNotificationsAsync(employeeUserId);

        await client.ReadNotificationsAsync(notificationIds);

        var responseAfterRead = await client.GetUnreadPreviewNotificationsAsync(employeeUserId);

        //Assert
        responseBeforeRead.Should().Be(true);
        responseAfterRead.Should().Be(false);
    }

    [Fact]
    public async Task DeleteNotificationWhenTimeTrackReportHaveBeenCreatedShouldResponseWithOkResult()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var format = nameof(DocumentType.Excel);
        var date = DateTime.UtcNow;

        var employeeUserId = (int)_factory.HeadOfDepartmentUserId!;

        await client.GetReportAsync(format, date.Year, employeeUserId);

        var notificationsResponse = await client.GetNotificationsAsync(
            employeeUserId, 100, 0,
            "IsDeleted eq false and Read eq false");

        var reportCreatedNotificationId = notificationsResponse.Items.Last().Id;

        //Act
        await client.DeleteNotificationsByIdAsync(reportCreatedNotificationId);

        var responseAfterDeletion = await client.GetNotificationsAsync(
            employeeUserId, 100, 0,
            "IsDeleted eq false and Read eq false");

        //Assert
        responseAfterDeletion!.Items.Should().HaveCount(1);

        responseAfterDeletion.Items.First().NotificationType.Should()
            .Be(NotificationTypes.TimeTrackReportGenerating);
    }

    [Fact]
    public async Task DeleteNotificationWhenThereAreExistingDeletedNotificationsShouldResponseWithOkResultWithRightDeletedCount()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var format = nameof(DocumentType.Excel);
        var firstDate = DateTime.UtcNow;
        var secondDate = DateTime.UtcNow.AddYears(-1);

        var employeeUserId = (int)_factory.EmployeeUserId!;

        await client.GetReportAsync(format, firstDate.Year, employeeUserId);

        await client.DeleteNotificationsByUserIdAsync(employeeUserId);

        await client.GetReportAsync(format, secondDate.Year, employeeUserId);

        await client.GetNotificationsAsync(
            employeeUserId, 100, 0,
            "IsDeleted eq false and Read eq false");

        //Act
        var deletedResponse = await client.DeleteNotificationsByUserIdAsOutputAsync(employeeUserId);

        //Assert
        deletedResponse!.DeletedCount.Should().NotBe(4);
        deletedResponse.DeletedCount.Should().Be(2);
    }
}
