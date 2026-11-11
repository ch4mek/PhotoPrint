using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Controllers.Booking;

[Collection("GeneralCollection")]
public class GetBookingsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _getBookingEmployeeWorkplaceId = 1210;
    private readonly int _getBookingEmployeeSecondOffiseWorkplaceId = 1212;
    private readonly int _getBookingCeaWorkplaceId = 1211;
    private readonly int _getBookingOfficeId = 10;
    private readonly int _getBookingSecondOfficeId = 11;

    public GetBookingsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetBookingsWhenLoginAsCtoAndIncludeEmployeeReturnAllBookings()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var createDate = DateTime.UtcNow.AddDays(21).Date;

        await client.AuthAsEmployee();
        await client.CreateSelfBooking(_getBookingEmployeeWorkplaceId, createDate, createDate);

        await client.AuthAsCeo();
        await client.CreateSelfBooking(_getBookingCeaWorkplaceId, createDate, createDate);

        // Act
        var bookings = await client.GetBookings(includeEmployee: true);

        // Assert
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeEmpty();
        bookings.Bookings.Should().Contain(b => b.UserId == _factory.EmployeeUserId);
        bookings.Bookings.Should().Contain(b => b.UserId == _factory.CeoUserId);
    }

    [Fact]
    public async Task GetBookingsWhenLoginAsEmployeeReturnAllOwnBookings()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(2).Date;

        await client.CreateSelfBooking(_getBookingEmployeeWorkplaceId, createDate, createDate);

        // Act
        var bookings = await client.GetBookings();

        // Assert
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeEmpty();
        bookings.Bookings.ForEach(x => x.UserId.Should().Be(_factory.EmployeeUserId));
    }

    [Fact]
    public async Task GetBookingsWhenLoginAsGMReturnAllOwnBookings()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        var createDate = DateTime.UtcNow.AddDays(3).Date;

        await client.CreateSelfBooking(_getBookingEmployeeWorkplaceId, createDate, createDate);

        // Act
        var bookings = await client.GetBookings();

        // Assert
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeEmpty();
        bookings.Bookings.Where(x => !x.IsForGuest).ToList().ForEach(x => x.UserId.Should().Be(_factory.GroupManagerId));
    }

    [Fact]
    public async Task GetBookingsWhenCreateBookingInTwoOfficesAndTryToGetFromSpecificOfficeReturnOnlyBookingsForSpecifiedOffice()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var createDate = DateTime.UtcNow.AddDays(40).Date;

        var firstBookingId = await client.CreateSelfBooking(_getBookingEmployeeWorkplaceId, createDate, createDate);

        var secondBookingId = await client.CreateSelfBooking(_getBookingEmployeeSecondOffiseWorkplaceId, createDate, createDate);

        // Act
        var firstOfficeBookings = await client.GetBookings(officeId: _getBookingOfficeId);

        // Assert
        firstOfficeBookings!.Bookings.Should().Contain(b => b.BookingId == firstBookingId);
        firstOfficeBookings.Bookings.Should().NotContain(b => b.BookingId == secondBookingId);
    }

    [Fact]
    public async Task GetBookingsWhenLoginAsGmAndIncludeEmployeeReturnAllBookingsOfUsersFromHisGroup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createDate = DateTime.UtcNow.AddDays(1).Date;

        var createdEmployeeInGroup = await client.CreateEmployee(resourceGroupId: 1);
        var createdEmployeeNotInGroup = await client.CreateEmployee();

        await client.CreateUserBooking(createdEmployeeInGroup!.Id,
            _getBookingEmployeeWorkplaceId, createDate, createDate);

        await client.CreateUserBooking(createdEmployeeNotInGroup!.Id,
            _getBookingCeaWorkplaceId, createDate, createDate);

        await client.AuthAs(EmailConstants.GroupManagerEmail);

        // Act
        var bookings = await client.GetBookings(includeEmployee: true);

        // Assert
        bookings.Should().NotBeNull();
        bookings.Bookings.Should().NotBeEmpty();
        bookings.Bookings.Should().Contain(b => b.UserId == createdEmployeeInGroup.Id);
        bookings.Bookings.Should().NotContain(b => b.UserId == createdEmployeeNotInGroup.Id);
    }
}
