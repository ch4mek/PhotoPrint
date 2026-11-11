using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Composers;
using Modsen.Portal.Booking.Requests.Workplaces.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Generators;
using Modsen.Portal.Booking.Requests.Workplaces.Requests;
using Modsen.Portal.Core.Services;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.UseCases.ReservationTests.Commands;

public class UpdateReservationsForBlockedWorkplacesUseCaseTests
{
    private readonly Mock<IBookingDbContext> _dbContext = new();

    private readonly Mock<IDatesBetweenExcludingStartGenerator> _datesGenerator = new();

    private readonly Mock<IBlockedBookingEntriesWithLastReservationDateComposer>
        _blockedBookingEntriesWithLastReservationDateComposerMock = new();

    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly Mock<IBookingPortalSettingsAccessor> _settingsAccessor = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateReservationsForBlockedWorkplacesRequest _useCase;

    public UpdateReservationsForBlockedWorkplacesUseCaseTests()
    {
        _useCase = new UpdateReservationsForBlockedWorkplacesRequest(
            _clockServiceMock.Object,
            _blockedBookingEntriesWithLastReservationDateComposerMock.Object,
            _settingsAccessor.Object,
            _dbContext.Object,
            _datesGenerator.Object);

        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task HandleShouldCallUpdateReservationsDbQuery()
    {
        // Arrange
        var currentTime = DateTime.Now;

        _clockServiceMock.Setup(x => x.UtcNow).Returns(currentTime);

        var bookingEntries = new List<BlockedBookingEntryWithLastReservationDateDto>
        {
            new(BookingEntryId: 1, LastReservationDate: currentTime.Date.AddMonths(3).AddDays(-1)),
            new(BookingEntryId: 2, LastReservationDate: currentTime.Date.AddMonths(3).AddMonths(-2))
        };

        _blockedBookingEntriesWithLastReservationDateComposerMock.Setup(x =>
                x.Compose(currentTime.Date, _cancellationToken))
            .ReturnsAsync(bookingEntries);

        var firstBookingEntryReservations = new List<Reservation>
        {
            new()
            {
                BookingEntryId = 1,
                ReservationDate = currentTime.Date.AddMonths(3)
            }
        };

        var secondBookingEntryReservations = new List<Reservation>
        {
            new()
            {
                BookingEntryId = 2,
                ReservationDate = currentTime.Date.AddMonths(3).AddDays(-1)
            },
            new()
            {
                BookingEntryId = 2,
                ReservationDate = currentTime.Date.AddMonths(3)
            }
        };

        _dbContext.Setup(x => x.Reservations.AddRange(firstBookingEntryReservations));
        _dbContext.Setup(x => x.Reservations.AddRange(secondBookingEntryReservations));

        // Act
        await _useCase.Handle(new UpdateReservationsForBlockedWorkplacesRequest.Command(), _cancellationToken);

        // Assert
        _dbContext.Verify(x => x.Reservations.AddRange(It.IsAny<List<Reservation>>()),
            Times.Exactly(2));
    }
}
