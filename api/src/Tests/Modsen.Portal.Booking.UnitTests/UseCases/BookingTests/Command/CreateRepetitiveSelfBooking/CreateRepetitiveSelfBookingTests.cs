using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core.Utils;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.UseCases.BookingTests.Command.CreateRepetitiveSelfBooking;

public class CreateRepetitiveSelfBookingTests
{
    private readonly Mock<IBookingEntryComposer> _bookingComposerMock = new();
    private readonly Mock<IBookingEntryLoader> _bookingEntryLoaderMock = new();
    private readonly Mock<IBookingPortalSettingsAccessor> _settingsAccessor = new();
    private readonly Mock<IIsAllowedToBookBusinessRule> _isAllowedToBookBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateRepetitiveSelfBookingTests()
    {
        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task CreateBookingShouldReturnCreatedBookingWhenBookingIsRepetitiveAndDataIsCorrect()
    {
        //Arrange
        var request = new CreateSelfBookingRequest.Command
        {
            FromDate = DateTime.UtcNow.Date,
            ToDate = DateTime.UtcNow.Date,
            WeeksInInterval = 2,
            WorkplaceId = 1
        };
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var settings = await _settingsAccessor.Object.GetSettings(_cancellationToken);
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.ToDate, toDate, intervalInDays).ToList();

        var bookingId = 1;

        var bookingDto = new BookingEntryCoreDto
        {
            BookingId = bookingId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            UserId = 1,
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

        var handler = new CreateSelfBookingRequest(
            _bookingComposerMock.Object,
            _bookingEntryLoaderMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);

        //Act
        var result = await handler.Handle(request, _cancellationToken);

        //Assert
        result.Should().BeEquivalentTo(bookingDto);
        intervalInDays.Should().Be(14);
        toDate.Should().Be(request.ToDate.AddMonths(3));
        dates.Count.Should().Be(7);
    }
}
