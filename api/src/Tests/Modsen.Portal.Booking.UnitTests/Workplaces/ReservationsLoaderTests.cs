using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workplaces;

public class ReservationsLoaderTests
{
    private readonly ReservationsLoader _reservationsLoader;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ReservationsLoaderTests()
    {
        var bookingDbContextMock = new Mock<IBookingDbContext>();

        var reservations = new List<Reservation> { _reservation, _deletedReservation};

        bookingDbContextMock
            .Setup(x => x.Reservations)
            .Returns(reservations.BuildMockDbSet().Object);

        _reservationsLoader = new ReservationsLoader(bookingDbContextMock.Object);
    }

    [Fact]
    public async Task LoadAsyncWhenIdsArrayEmptyReturnEmptyList()
    {
        // Arrange
        var ids = Array.Empty<int>();

        //Act
        var result = await _reservationsLoader.LoadAsync(ids, _cancellationToken);

        //Assert
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task LoadAsyncWhenIdsArrayContainsSingleReservationIdReturnSingleReservation()
    {
        // Arrange
        var ids = new[] { 1 };

        //Act
        var result = await _reservationsLoader.LoadAsync(ids, _cancellationToken);

        //Assert
        result.Count.Should().Be(1);
        result.FirstOrDefault()!.Date.Should().Be(_reservation.ReservationDate);

        result.FirstOrDefault()!.UserWorkplaces.FirstOrDefault()!.UserId.Should().Be(_reservation.BookingEntry!.UserId);
        result.FirstOrDefault()!.UserWorkplaces.FirstOrDefault()!.WorkplaceNumber.Should().Be(_reservation.BookingEntry.Workplace!.WorkplaceNumber);
    }

    [Fact]
    public async Task LoadAsyncWhenIdsArrayContainsSeveralIdsReturnBothDeletedAndNotDeletedReservations()
    {
        // Arrange
        var ids = new[] { 1, 2 };

        //Act
        var result = await _reservationsLoader.LoadAsync(ids, _cancellationToken);

        //Assert
        result.Count.Should().Be(2);
    }

    private readonly Reservation _reservation = new()
    {
        Id = 1,
        ReservationDate = DateTime.UtcNow,
        ReservationYear = DateTime.UtcNow.Year,
        ReservationForMonth = DateTime.UtcNow.Month,
        ReservationForWeek = 1,
        ReservationForDay = 1,
        IsDeleted = false,
        BookingEntry = new BookingEntry
        {
            UserId = 1,
            User = new BookingUserEntity
            {
                UserId = 1,
                FirstName = "FirstName",
                LastName = "LastName",
                DepartmentId = "Net",
                Email = "test@modsen-software.com"
            },
            Workplace = new Workplace
            {
                Id = 1,
                WorkplaceNumber = 1,
                OfficeId = 1
            },
            GuestName = null,
            IsForGuest = false,
            IsBlock = false,
            CreatedByUser = new BookingUserEntity
            {
                FirstName = "Ceo",
                LastName = "Test",
                Email = "test@modsen-software.com"
            }
        },
        BookingEntryId = 1,
        CreatedByUser = null,
        UpdatedById = null
    };

    private readonly Reservation _deletedReservation = new()
    {
        Id = 2,
        ReservationDate = DateTime.UtcNow,
        ReservationYear = DateTime.UtcNow.Year,
        ReservationForMonth = DateTime.UtcNow.Month,
        ReservationForWeek = 1,
        ReservationForDay = 1,
        IsDeleted = false,
        BookingEntry = new BookingEntry
        {
            UserId = 2,
            User = new BookingUserEntity
            {
                UserId = 2,
                FirstName = "FirstName",
                LastName = "LastName",
                DepartmentId = "Net",
                Email = "test@modsen-software.com"
            },
            Workplace = new Workplace
            {
                Id = 2,
                WorkplaceNumber = 2,
                OfficeId = 2
            },
            GuestName = null,
            IsForGuest = false,
            IsBlock = false,
            CreatedByUser = new BookingUserEntity
            {
                FirstName = "Ceo",
                LastName = "Test",
                Email = "test@modsen-software.com"
            }
        },
        BookingEntryId = 2,
        CreatedByUser = null,
        UpdatedById = null
    };
}
