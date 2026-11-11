using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Booking;

public class CollisionsForDatesGeneratorTests
{
    private readonly Mock<IBookingsWithoutUsersOwnReservationsCollisionsResolver> _bookingsWithoutUsersOwnReservationsCollisionsResolverMock = new();
    private readonly Mock<IBookingByDatesResolver> _bookingsByDatesResolverMock = new();
    private readonly Mock<IBookingEntryLoader> _bookingLoaderMock = new();

    private readonly DateTime now = DateTime.UtcNow;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task GenerateShouldReturnCollisions()
    {
        //Assert
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
                UserId = 1,
                DateList = { now.AddDays(2) },
                WeeksInInterval = 0,
                FirstName = "Test2",
                LastName = "Test2",
                WorkplaceId = 1
            }
        };

        var weeksInInterval = 0;
        var workplaceId = 1;
        var userId = 1;

        var dates = new List<DateTime> { now, now.AddDays(1) };

        _bookingsWithoutUsersOwnReservationsCollisionsResolverMock.Setup(x => x.Resolve(
            It.IsAny<List<BookingEntryCoreDto>>(),
            It.IsAny<List<DateTime>>(),
            It.IsAny<List<DateTime>>(),
            It.IsAny<int>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>()))
            .Returns(bookings);

        _bookingsByDatesResolverMock.Setup(x => x.ResolveBookingIdsByDatesAsync(
            It.IsAny<List<DateTime>>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            _cancellationToken))
            .ReturnsAsync([1, 2]);

        _bookingLoaderMock.Setup(x => x.Load(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(bookings);

        var collisionsForDatesGenerator = new CollisionsForDatesGenerator(
            _bookingsWithoutUsersOwnReservationsCollisionsResolverMock.Object,
            _bookingsByDatesResolverMock.Object,
            _bookingLoaderMock.Object
            );

        // Act
        var result = await collisionsForDatesGenerator.GenerateCollisionsForDates(
            weeksInInterval,
            workplaceId,
            userId,
            dates,
            _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GenerateShouldReturnNoCollisions()
    {
        //Assert
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
                UserId = 1,
                DateList = { now.AddDays(2) },
                WeeksInInterval = 0,
                FirstName = "Test2",
                LastName = "Test2",
                WorkplaceId = 1
            }
        };

        var weeksInInterval = 0;
        var workplaceId = 1;
        var userId = 1;

        var dates = new List<DateTime> { now.AddDays(3), now.AddDays(4) };

        _bookingsWithoutUsersOwnReservationsCollisionsResolverMock.Setup(x => x.Resolve(
            It.IsAny<List<BookingEntryCoreDto>>(),
            It.IsAny<List<DateTime>>(),
            It.IsAny<List<DateTime>>(),
            It.IsAny<int>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            It.IsAny<int?>()))
            .Returns(bookings);

        _bookingsByDatesResolverMock.Setup(x => x.ResolveBookingIdsByDatesAsync(
            It.IsAny<List<DateTime>>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            _cancellationToken))
            .ReturnsAsync([1, 2]);

        _bookingLoaderMock.Setup(x => x.Load(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(bookings);

        var collisionsForDatesGenerator = new CollisionsForDatesGenerator(
            _bookingsWithoutUsersOwnReservationsCollisionsResolverMock.Object,
            _bookingsByDatesResolverMock.Object,
            _bookingLoaderMock.Object
            );

        // Act
        var result = await collisionsForDatesGenerator.GenerateCollisionsForDates(
            weeksInInterval,
            workplaceId,
            userId,
            dates,
            _cancellationToken);

        // Assert
        result.Should().HaveCount(0);
    }
}
