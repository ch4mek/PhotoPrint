using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Controllers.Booking;

[Collection("GeneralCollection")]
public class CreateBookingIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _createBookingEmployeeWorkplaceId = 1214;
    private readonly int _createBookingSecondEmployeeWorkplaceId = 1215;
    private readonly int _createBookingThirdEmployeeWorkplaceId = 1216;
    private readonly int _createBookingForSelfWorkplaceId = 1217;
    private readonly int _createBookingForGuestWorkplaceId = 1218;

    public CreateBookingIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateBookingCreateSelfSingleBookingAsEmployeeWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(5).Date;

        // Act
        var createdBookingId = await client.CreateSelfBooking(workplaceId, createDate, createDate);

        // Assert
        var bookings = await client.GetBookings();

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateSingleBookingAsEmployeeForGuestReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(5).Date;
        var workplaceId = _factory.WorkplaceIdInMinksOffice;

        // Act
        var response = await client.CreateGuestBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBookingCreateForGuestSingleBookingAsCeoReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createDate = DateTime.UtcNow.AddDays(10).Date;
        var workplaceId = _factory.WorkplaceIdInMinksOffice;

        // Act
        var response = await client.CreateGuestBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBookingCreateForGuestWhenAlreadyHaveBookingOnDateReturnConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createDate = DateTime.UtcNow.AddDays(2).Date;
        var guestName = "test";

        await client.CreateGuestBookingAsHttpMessage(_createBookingEmployeeWorkplaceId, createDate, createDate,
            guestName: guestName);

        // Act
        var response = await client.CreateGuestBookingAsHttpMessage(_createBookingSecondEmployeeWorkplaceId, createDate, createDate,
            guestName: guestName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBookingCreateForSelfWhenCreatedForGuestReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createDate = DateTime.UtcNow.AddDays(1).Date;
        var guestName = "test";

        await client.CreateGuestBookingAsHttpMessage(_createBookingForGuestWorkplaceId, createDate, createDate,
            guestName: guestName);

        // Act
        var response = await client.CreateSelfBookingAsHttpMessage(_createBookingForSelfWorkplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBookingCreateSelfSingleBookingAndOneMoreCoveringBookingBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(1).Date;

        var createdSelfBookingId = await client.CreateSelfBooking(_createBookingThirdEmployeeWorkplaceId, createDate, createDate);

        // Act
        var createdCoveringSelfBookingId = await client.CreateSelfBooking(
            _createBookingThirdEmployeeWorkplaceId,
            fromDate: createDate,
            toDate: createDate.AddDays(4));

        // Assert
        var bookings = await client.GetBookings();

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotContain(x => x.BookingId == createdSelfBookingId);
        bookings.Bookings.Should().Contain(x => x.BookingId == createdCoveringSelfBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateSingleBookingForUserAsCeoWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(6).Date;
        var userId = _factory.EmployeeUserId.Value;

        // Act
        var createdEmployeeBookingId = await client.CreateUserBooking(userId, workplaceId, createDate, createDate);

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdEmployeeBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateSelfMultipleBookingAsEmployeeWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(7).Date;

        // Act
        var createdSelfBookingId = await client.CreateSelfBooking(workplaceId,
            fromDate: createDate,
            toDate: createDate.AddDays(1));

        // Assert
        var bookings = await client.GetBookings();

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdSelfBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateMultipleBookingForUserAsCeoWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(9).Date;
        var userId = _factory.EmployeeUserId.Value;

        // Act
        var createdEmployeeBookingId = await client.CreateUserBooking(userId, workplaceId,
            fromDate: createDate,
            toDate: createDate.AddDays(1));

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdEmployeeBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateRepetitiveSelfBookingAsEmployeeWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = 11;
        const int weeksInInterval = 1;
        var createDate = DateTime.UtcNow.Date;

        // Act
        var createdSelfBookingId = await client.CreateSelfBooking(workplaceId,
            fromDate: createDate,
            toDate: createDate,
            weeksInInterval: weeksInInterval);

        // Assert
        var bookings = await client.GetBookings();

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdSelfBookingId);
    }

    [Fact]
    public async Task CreateBookingCreateRepetitiveBookingForUserAsCeoWithoutCollisionsBookingCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int weeksInInterval = 1;
        var createDate = DateTime.UtcNow.Date.AddDays(5);
        var userId = _factory.EmployeeUserId.GetValueOrDefault();

        // Act
        var createdEmployeeBookingId = await client.CreateUserBooking(
            userId,
            _createBookingEmployeeWorkplaceId,
            fromDate: createDate,
            toDate: createDate,
            weeksInInterval: weeksInInterval);

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdEmployeeBookingId);
    }

    [Fact]
    public async Task CreateBlockCreateSelfBlockAsCeoBlockCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int workplaceId = 5;
        var createDate = DateTime.UtcNow.AddDays(40).Date;

        // Act
        var createdSelfBlockId = await client.CreateBlockBooking(workplaceId, createDate, createDate);

        // Assert
        var bookings = await client.GetBookings();

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdSelfBlockId);
    }

    [Fact]
    public async Task CreateBlockCreateBlockForUserAsCeoBlockCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int workplaceId = 6;
        var createDate = DateTime.UtcNow.AddDays(41).Date;
        var userId = _factory.EmployeeUserId.GetValueOrDefault();

        // Act
        var createdBlockBookingId = await client.CreateBlockBooking(workplaceId, createDate, createDate,
            userId: userId);

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdBlockBookingId);
    }

    [Fact]
    public async Task CreateBlockCreateBlockForUserAsHeadOfDepartmentBlockCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var createDate = DateTime.UtcNow.AddDays(1).Date;
        var userId = _factory.EmployeeUserId.GetValueOrDefault();

        // Act
        var createdBlockBookingId = await client.CreateBlockBooking(_createBookingSecondEmployeeWorkplaceId, createDate, createDate,
            userId: userId);

        // Assert
        var bookings = await client.GetBookings(includeEmployee: true);

        bookings.Should().NotBeNull();
        bookings.Bookings.Should().Contain(x => x.BookingId == createdBlockBookingId);
    }

    [Fact]
    public async Task CreateBlockCreateBlockForUserAsHeadOfDepartmentReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        const int workplaceId = 6;
        var createDate = DateTime.UtcNow.AddDays(41).Date;
        var userId = _factory.CtoUserId;

        // Act
        var response = await client.CreateBlockBookingAsHttpMessage(workplaceId, createDate, createDate,
            userId: userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBookingCreateSelfSingleBookingAsEmployeeWithCollisionsBookingNotCreated()
    {
        // Arrange
        using var ceoClient = _factory.CreateClient();
        using var employeeClient = _factory.CreateClient();
        await ceoClient.AuthAsCeo();
        await employeeClient.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(30).Date;

        await ceoClient.CreateSelfBooking(workplaceId, createDate, createDate);

        // Act
        var response = await employeeClient.CreateSelfBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBookingCreateSelfSingleBookingAsEmployeeWithSingleCollisionBookingNotCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(29).Date;

        await client.CreateSelfBooking(workplaceId, createDate, createDate);

        // Act
        var response = await client.CreateSelfBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBookingCreateSelfSingleBookingAsEmployeeWhenYouHaveBookingOnThisDateBookingNotCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(31).Date;

        await client.CreateSelfBooking(workplaceId, createDate, createDate);

        // Act
        var response = await client.CreateSelfBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBookingWhenBookWorkspaceWithNumberThatDoesNotExistInOfficeReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 10000;
        var createDate = DateTime.UtcNow.AddDays(32).Date;

        // Act
        var response = await client.CreateSelfBookingAsHttpMessage(workplaceId, createDate, createDate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
