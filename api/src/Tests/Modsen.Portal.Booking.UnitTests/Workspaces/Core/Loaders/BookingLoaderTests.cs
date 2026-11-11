using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Modsen.Portal.Core.Context;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workspaces.Core.Loaders;

public class BookingLoaderTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IBookingStartEndDateLoader> _bookingStartEndDateLoaderMock = new();
    private readonly Mock<IBookingDbContext> _bookingDbContextMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact]
    public async Task GetBookingsWhenEmptyBookingsReturnEmptyList()
    {
        // Arrange
        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        SetupBookings([]);
        SetupFirstEndDateLoader(DateTime.UtcNow, DateTime.UtcNow.AddMonths(2));

        // Act
        var result = await loader.LoadAsync(null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBookingsWhenMultipleReservationsReturnValidList()
    {
        // Arrange
        var bookingEntries = new List<BookingEntry>
        {
            new()
            {
                Id = 99,
                IsBlock = false,
                UserId = 99,
                WorkplaceId = 99,
                Notifications = [],
                WeeksInInterval = 0,
                User = new()
                {
                    UserId = 99,
                    DepartmentId = "Net",
                    Email = "test@modsen-software.com",
                    FirstName = "first",
                    LastName = "last"
                },
                Workplace = new()
                {
                    Id = 99,
                    OfficeId = 99,
                    WorkplaceNumber = 99
                },
                Reservations =
                [
                    new Reservation { ReservationDate = new DateTime(2023, 05, 24), BookingEntryId = 99 },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 25), BookingEntryId = 99  },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 28), BookingEntryId = 99  },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 29), BookingEntryId = 99  }
                ]
            }
        };

        SetupBookings(bookingEntries);
        SetupFirstEndDateLoader(new DateTime(2023, 04, 23), new DateTime(2023, 06, 23));

        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        // Act
        var result = await loader.LoadAsync(null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().ContainSingle();
        result.Bookings[0].DateList
            .Should()
            .BeEquivalentTo(new List<DateTime> { new(2023, 05, 24), new(2023, 05, 29) }, opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task GetBookingsWhenOneReservationReturnValidList()
    {
        // Arrange
        var bookingEntries = new List<BookingEntry>
        {
            new()
            {
                Id = 99,
                IsBlock = false,
                UserId = 99,
                WorkplaceId = 99,
                Notifications = [],
                WeeksInInterval = 0,
                User = new()
                {
                    UserId = 99,
                    DepartmentId = "Net",
                    Email = "test@modsen-software.com",
                    FirstName = "first",
                    LastName = "last"
                },
                Workplace = new()
                {
                    Id = 99,
                    OfficeId = 99,
                    WorkplaceNumber = 99
                },
                Reservations =
                [ 
                    new Reservation { ReservationDate = new DateTime(2023, 05, 24), BookingEntryId = 99  }
                ]
            }
        };

        SetupBookings(bookingEntries);
        SetupFirstEndDateLoader(new DateTime(2023, 04, 23), new DateTime(2023, 06, 23));

        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        // Act
        var result = await loader.LoadAsync(null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().ContainSingle();
        result.Bookings[0].DateList
            .Should()
            .BeEquivalentTo(new List<DateTime> { new(2023, 05, 24) });
    }

    [Fact]
    public async Task GetBookingsByUserIdWhenEmptyBookingsReturnEmptyList()
    {
        // Arrange
        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        SetupBookings([]);
        SetupFirstEndDateLoader(DateTime.UtcNow, DateTime.UtcNow.AddMonths(2));

        // Act
        var result = await loader.LoadAsync(1, null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBookingsByUserIdWhenMultipleReservationsReturnValidList()
    {
        // Arrange
        var bookingEntries = new List<BookingEntry>
        {
            new()
            {
                Id = 99,
                IsBlock = false,
                UserId = 99,
                WorkplaceId = 99,
                Notifications = [],
                WeeksInInterval = 0,
                User = new()
                {
                    UserId = 99,
                    DepartmentId = "Net",
                    Email = "test@modsen-software.com",
                    FirstName = "first",
                    LastName = "last"
                },
                Workplace = new Workplace
                {
                    Id = 99,
                    OfficeId = 99,
                    WorkplaceNumber = 99
                },
                Reservations =
                [
                    new Reservation { ReservationDate = new DateTime(2023, 05, 24), BookingEntryId = 99 },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 25), BookingEntryId = 99 },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 28), BookingEntryId = 99 },

                    new Reservation { ReservationDate = new DateTime(2023, 05, 29), BookingEntryId = 99 }
                ]
            }
        };

        SetupBookings(bookingEntries);
        SetupFirstEndDateLoader(new DateTime(2023, 04, 23), new DateTime(2023, 06, 23));

        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        // Act
        var result = await loader.LoadAsync(99, null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().ContainSingle();
        result.Bookings[0].DateList
            .Should()
            .BeEquivalentTo(new List<DateTime> { new(2023, 05, 24), new(2023, 05, 29) }, opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task GetBookingsByUserIdWhenOneReservationReturnValidList()
    {
        // Arrange
        var bookingEntries = new List<BookingEntry>
        {
            new()
            {
                Id = 99,
                IsBlock = false,
                UserId = 99,
                WorkplaceId = 99,
                Notifications = [],
                WeeksInInterval = 0,
                User = new()
                {
                    UserId = 99,
                    DepartmentId = "Net",
                    Email = "test@modsen-software.com",
                    FirstName = "first",
                    LastName = "last"
                },
                Workplace = new Workplace
                {
                    Id = 99,
                    OfficeId = 99,
                    WorkplaceNumber = 99
                },
                Reservations =
                [
                    new Reservation { ReservationDate = new DateTime(2023, 05, 24), BookingEntryId = 99 }
                ]
            }
        };

        SetupBookings(bookingEntries);
        SetupFirstEndDateLoader(new DateTime(2023, 04, 23), new DateTime(2023, 06, 23));

        var loader = new BookingLoader(
            _authenticatedUserContextMock.Object,
            _bookingStartEndDateLoaderMock.Object,
            _bookingDbContextMock.Object);

        // Act
        var result = await loader.LoadAsync(99, null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Bookings.Should().ContainSingle();
        result.Bookings[0].DateList
            .Should()
            .BeEquivalentTo(new List<DateTime> { new(2023, 05, 24) });
    }

    private void SetupBookings(List<BookingEntry> bookings)
    {
        _bookingDbContextMock
            .Setup(mock => mock.Bookings)
            .Returns(bookings.BuildMockDbSet().Object);
    }

    private void SetupFirstEndDateLoader(DateTime firstDate, DateTime endDate)
    {
        _bookingStartEndDateLoaderMock
            .Setup(mock => mock.LoadAsync(_cancellationToken))
            .ReturnsAsync((firstDate, endDate));
    }
}
