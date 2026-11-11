using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.UseCases.BookingTests.Command;

public class BlockWorkplaceRequestTests
{
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IBookingPortalSettingsAccessor> _bookingPortalSettingsAccessorMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IGetWorkspaceAvailableStatusDbQuery> _getWorkspaceAvailableStatusDbQueryMock = new();
    private readonly Mock<IBookingByDatesResolver> _bookingByDatesResolverMock = new();
    private readonly Mock<IBookingEntryLoader> _bookingEntryLoaderMock = new();
    private readonly Mock<IGetUserByIdDbQuery> _getUserByIdDbQueryMock = new();
    private readonly Mock<IGetWorkPlaceByIdDbQuery> _getWorkPlaceByIdDbQueryMock = new();
    private readonly Mock<IBookingDbContext> _bookingDbContextMock = new();
    private readonly Mock<IIsAllowedToBookBusinessRule> _isAllowedToBookBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly BlockWorkplaceRequest _handler;

    public BlockWorkplaceRequestTests()
    {
        _handler = new BlockWorkplaceRequest(
            _communicationBusMock.Object,
            _clockServiceMock.Object,
            _bookingPortalSettingsAccessorMock.Object,
            _authenticatedUserContextMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingByDatesResolverMock.Object,
            _bookingEntryLoaderMock.Object,
            _getUserByIdDbQueryMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _bookingDbContextMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);

        _bookingPortalSettingsAccessorMock
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task HandleWhenWorkplaceNotFoundThrowsEntityNotFoundException()
    {
        // Arrange
        var request = new BlockWorkplaceRequest.Command
        {
            WorkplaceId = 123,
            StartDate = DateTime.UtcNow
        };

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(request.WorkplaceId, _cancellationToken))
            .ReturnsAsync(null as Workplace);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserHasBookingsOnProvidedDatesBlockShoulBeCreated()
    {
        // Arrange
        var request = new BlockWorkplaceRequest.Command
        {
            WorkplaceId = 123,
            StartDate = DateTime.UtcNow
        };

        var workplace = new Workplace { Id = 1, OfficeId = 1 };

        var bookings = new List<BookingEntryCoreDto>
        {
            new()
            {
                UserId = 1,
                IsBlock = true,
                WorkplaceId = workplace.Id,
                BookingId = 1,
                FirstName = "Test",
                LastName = "Test"
            }
        };

        var booking = bookings.FirstOrDefault();

        var bookingEntriesIds = bookings
            .Select(y => y.BookingId)
            .ToList();

        var authenticatedUserId = 1;

        _authenticatedUserContextMock.Setup(x => x.UserId)
            .Returns(authenticatedUserId);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(request.WorkplaceId, _cancellationToken))
            .ReturnsAsync(workplace);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _bookingByDatesResolverMock.Setup(x => x.ResolveBookingIdsByDatesAsync(
            It.IsAny<List<DateTime>>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<int?>(),
            It.IsAny<string?>(),
            _cancellationToken))
            .ReturnsAsync(bookingEntriesIds.ToArray());

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int[]>(), _cancellationToken))
            .ReturnsAsync(bookings);

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(booking);

        _isAllowedToBookBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        MockDbSets();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(booking);
    }

    private void MockDbSets()
    {
        var reservationsMock = new List<Reservation>();
        var reservationDbSetMock = reservationsMock.BuildMockDbSet();

        var bookingsMock = new List<BookingEntry>();
        var bookingDbSetMock = bookingsMock.BuildMockDbSet();

        reservationDbSetMock.As<IAsyncEnumerable<Reservation>>()
            .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => reservationsMock.ToAsyncEnumerable().GetAsyncEnumerator(_cancellationToken));

        _bookingDbContextMock.Setup(x => x.Reservations).Returns(reservationDbSetMock.Object);

        bookingDbSetMock.As<IAsyncEnumerable<BookingEntry>>()
            .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => bookingsMock.ToAsyncEnumerable().GetAsyncEnumerator(_cancellationToken));

        _bookingDbContextMock.Setup(x => x.Bookings).Returns(bookingDbSetMock.Object);
    }
}
