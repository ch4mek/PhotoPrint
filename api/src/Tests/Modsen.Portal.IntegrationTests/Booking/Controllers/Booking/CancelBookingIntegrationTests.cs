using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Controllers.Booking;

[Collection("GeneralCollection")]
public class CancelBookingIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _cancelBookingEmployeeWorkplaceId = 1213;

    public CancelBookingIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CancelBookingCancelOwnBookingAsEmployeeBookingDeleted()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(1).Date;

        var createdBookingId = await client.CreateSelfBooking(_cancelBookingEmployeeWorkplaceId, createDate, createDate);

        // Act
        await client.DeleteBookings([createdBookingId]);

        var bookings = await client.GetBookings();

        // Assert
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeNull();
        bookings.Bookings.ForEach(x => x.BookingId.Should().NotBe(createdBookingId));
    }

    [Fact]
    public async Task CancelBookingByEmployeeCancelBookingOfOtherUserReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var createDate = DateTime.UtcNow.AddDays(2).Date;

        var createdBookingId = await client.CreateSelfBooking(_cancelBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAsEmployee();

        // Act
        var response = await client.DeleteBookingAsHttpMessage(createdBookingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CancelBookingAuthAsAdminCancelBookingOfOtherUserReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var createDate = DateTime.UtcNow.AddDays(3).Date;

        var createdBookingId = await client.CreateSelfBooking(_cancelBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAsAdmin();

        // Act
        var response = await client.DeleteBookingAsHttpMessage(createdBookingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CancelBookingAuthAsGmCancelBookingOfUserFromOtherGroupReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(4).Date;

        var createdBookingId = await client.CreateSelfBooking(_cancelBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await client.DeleteBookingAsHttpMessage(createdBookingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CancelBookingAuthAsGmCancelBookingOfUserFromHisGroupReturnsOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createDate = DateTime.UtcNow.AddDays(5).Date;

        var createdEmployeeInGroup = await client.CreateEmployee(resourceGroupId: 1);

        var createdEmployeeBookingId = await client.CreateUserBooking(createdEmployeeInGroup!.Id,
            _cancelBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var response = await client.DeleteBookingAsHttpMessage(createdEmployeeBookingId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelBookingCancelUserBookingAsCtoBookingDeleted()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(6).Date;

        var createdBookingId = await client.CreateSelfBooking(_cancelBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAsCto();

        // Act
        await client.DeleteBookingAsHttpMessage(createdBookingId);

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeNull();
        bookings.Bookings.ForEach(x => x.BookingId.Should().NotBe(createdBookingId));
    }
}
