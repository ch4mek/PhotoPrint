using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.UseCases.BookingTests.Command.CreateSingleGuestBooking;

public class CreateSingleGuestBookingTests
{
    private readonly Mock<IBookingEntryComposer> _bookingComposerMock = new();
    private readonly Mock<IBookingEntryLoader> _bookingEntryLoaderMock = new();
    private readonly Mock<IBookingPortalSettingsAccessor> _settingsAccessor = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Mock<IIsAllowedToBookBusinessRule> _isAllowedToBookBusinessRuleMock = new();

    private readonly CreateGuestBookingRequest _handler;

    public CreateSingleGuestBookingTests()
    {
        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });

        _handler = new CreateGuestBookingRequest(
            _bookingComposerMock.Object,
            _bookingEntryLoaderMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenBookingForGuestAndDataValidReturnCreatedBooking()
    {
        var bookingId = 10;

        var request = new CreateGuestBookingRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 16),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            GuestName = "test"
        };
        var bookingDto = new BookingEntryCoreDto
        {
            BookingId = bookingId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            UserId = 1,
            GuestName = request.GuestName,
            IsForGuest = true,
            FirstName = null,
            LastName = null
        };

        var bookingEntry = new BookingEntry
        {
            UserId = 1,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            Id = bookingId
        };

        _bookingComposerMock.Setup(x => x.Compose(It.IsAny<BookingEntryComposerRequest>(), _cancellationToken))
           .ReturnsAsync(bookingEntry);

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(bookingDto);

        _isAllowedToBookBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        //Act
        var result = await _handler.Handle(request, _cancellationToken);

        //Assert
        result.BookingId.Should().Be(bookingId);
        result.GuestName.Should().Be(request.GuestName);
        result.IsForGuest.Should().Be(true);
    }
}
