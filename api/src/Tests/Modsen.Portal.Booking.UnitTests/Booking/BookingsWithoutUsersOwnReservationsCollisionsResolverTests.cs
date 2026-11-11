using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;

namespace Modsen.Portal.Booking.UnitTests.Booking;

public class BookingsWithoutUsersOwnReservationsCollisionsResolverTests
{
    private readonly IBookingsWithoutUsersOwnReservationsCollisionsResolver _bookingsWithoutUsersOwnReservationsCollisionsResolver = new BookingsWithoutUsersOwnReservationsCollisionsResolver();
    private readonly DateTime now = DateTime.UtcNow;

    [Fact]
    public async Task ResolveShouldReturnBookings()
    {
        // Arrange
        var bookings = new List<BookingEntryCoreDto>
        {
            new()
            {
                BookingId = 1,
                UserId = 1,
                DateList = { now, now.AddDays(1) },
                WeeksInInterval = 0,
                FirstName = "Test",
                LastName = "Test",
                WorkplaceId = 1
            },
            new()
            {
                BookingId = 2,
                UserId = 2,
                DateList = { now.AddDays(2) },
                WeeksInInterval = 0,
                FirstName = "Test2",
                LastName = "Test2",
                WorkplaceId = 1
            }
        };

        var weeksInInterval = 0;
        var workpalceId = 1;
        var bookedDates = bookings.SelectMany(booking => booking.DateList).ToList();
        var requestedDates = new List<DateTime> { now.AddDays(1), now.AddDays(2), now.AddDays(3) };

        // Act
        var bookingsResult = _bookingsWithoutUsersOwnReservationsCollisionsResolver.Resolve(
            bookings,
            bookedDates,
            requestedDates,
            weeksInInterval,
            null,
            null,
            workpalceId);

        // Assert
        bookingsResult.Should().HaveCount(2);
    }

    [Fact]
    public async Task ResolveShouldReturnNoBookings()
    {
        // Arrange
        var bookings = new List<BookingEntryCoreDto>
        {
            new()
            {
                BookingId = 1,
                UserId = 1,
                DateList = { now },
                WeeksInInterval = 0,
                FirstName = "Test",
                LastName = "Test",
                WorkplaceId = 1
            },
            new()
            {
                BookingId = 2,
                UserId = 2,
                DateList = { now.AddDays(1) },
                WeeksInInterval = 0,
                FirstName = "Test2",
                LastName = "Test2",
                WorkplaceId = 1
            }
        };

        var weeksInInterval = 0;
        var workpalceId = 1;
        var bookedDates = bookings.SelectMany(booking => booking.DateList).ToList();
        var requestedDates = new List<DateTime> { now, now.AddDays(1) };

        // Act
        var bookingsResult = _bookingsWithoutUsersOwnReservationsCollisionsResolver.Resolve(
            bookings,
            bookedDates,
            requestedDates,
            weeksInInterval,
            null,
            null,
            workpalceId);

        // Assert
        bookingsResult.Should().HaveCount(0);
    }
}
