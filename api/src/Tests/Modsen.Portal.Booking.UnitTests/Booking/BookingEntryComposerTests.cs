using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Booking;

public class BookingEntryComposerTests
{
    private readonly Mock<IBookingPortalSettingsAccessor> _bookingPortalSettingsAccessorMock = new();
    private readonly Mock<IAreCollisionsExistBusinessRule> _areCollisionsExistBusinessRuleMock = new();
    private readonly Mock<IGetWorkPlaceByIdDbQuery> _getWorkPlaceByIdDbQueryMock = new();
    private readonly Mock<IGetWorkspaceAvailableStatusDbQuery> _getWorkspaceAvailableStatusDbQueryMock = new();
    private readonly Mock<IBookingDbContext> _bookingDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;


    public BookingEntryComposerTests()
    {
        _bookingPortalSettingsAccessorMock
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task ComposeByManagerWhenBookingIsMultipleAndDataIsCorrectWithCeoRoleForEmployeeFromNetDepShouldReturnCreatedBooking()
    {
        //Assert
        var user = new BookingUserEntity
        {
            UserId = 2,
            DepartmentId = "Net",
            Email = "modsennet@modsen-software.com",
            FirstName = "User",
            LastName = "FromNet"
        };

        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 2,
            IsForGuest = false
        };

        var settings = await _bookingPortalSettingsAccessorMock.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };


        var bookingEntry = new BookingEntry
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
        };

        var authenticatedCeo = AuthenticatedCeoMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedCeo.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var result = await bookingComposer.Compose(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(bookingEntry);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task ComposeByManagerWhenBookingIsMultipleAndDataIsCorrectWithCeoRoleForHimselfShouldReturnCreatedBooking()
    {
        //Assert
        var user = new BookingUserEntity
        {
            UserId = 1,
            DepartmentId = null,
            Email = "modsenceo@modsen-software.com",
            FirstName = "Ceo",
            LastName = "Ceo"
        };

        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 1,
            IsForGuest = false
        };

        var settings = await _bookingPortalSettingsAccessorMock.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };


        var bookingEntry = new BookingEntry
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
        };

        var authenticatedCeo = AuthenticatedCeoMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedCeo.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var result = await bookingComposer.Compose(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(bookingEntry);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task ComposeWhenBookingIsMultipleAndDataIsCorrectShouldReturnCreatedBooking()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var settings = await _bookingPortalSettingsAccessorMock.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var bookingEntry = new BookingEntry
        {
            UserId = 1,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var result = await bookingComposer.Compose(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(bookingEntry);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task ComposeForGuestWhenBookingIsMultipleAndDataIsCorrectShouldReturnCreatedBooking()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = true,
            GuestName = "test"
        };

        var settings = await _bookingPortalSettingsAccessorMock.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var bookingEntry = new BookingEntry
        {
            UserId = 1,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            IsForGuest = true,
            GuestName = request.GuestName
        };

        var authenticatedCeo = AuthenticatedCeoMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForGuestByDatesExist(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedCeo.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var result = await bookingComposer.Compose(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(bookingEntry);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task ComposeWhenBookingToDateLessThanFromDateShouldThrowInvalidDataAppException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 20),
            ToDate = new DateTime(2023, 06, 16),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var workplace = new Workplace { Id = 1, OfficeId = 1 };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<List<DateTime>>(),
                _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<List<DateTime>>(),
                _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
        );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ComposeWhenWorkplaceDoesNotExistShouldThrowEntityNotFoundException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(default(Workplace));

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task ComposeWhenUserAlreadyHaveBookingsShouldThrowInvalidDataException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ThrowsAsync(new InvalidDataException(PortalMessages.AttemptToCreateSeveralBookings));

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task ComposeWhenGuestAlreadyHaveBookingsShouldThrowInvalidDataException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = true,
            GuestName = "test"
        };

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForGuestByDatesExist(
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ThrowsAsync(new InvalidDataException(PortalMessages.AttemptToCreateSeveralBookingsForGuest));

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task ComposeWhenWorkplaceIsBookedShouldThrowInvalidDataException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ThrowsAsync(new InvalidDataException(PortalMessages.BookingExistsForWorkplace));

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataException>();
    }

    [Fact]
    public async Task ComposeWhenWeeksInIntervalAndDatesAreInvalidShouldThrowEntityConflictException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 1,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(true);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityConflictException>();
    }

    [Fact]
    public async Task ComposeWhenBookInWorkspaceIsUnavailableShouldThrowInvalidDataAppException()
    {
        //Assert
        var request = new BookingEntryComposerRequest
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            IsForGuest = false
        };

        var workplace = new Workplace
        {
            Id = 1,
            OfficeId = 1,
            WorkspaceId = 1
        };

        var authenticatedEmployee = AuthenticatedEmployeeMock();

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForUserByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _areCollisionsExistBusinessRuleMock.Setup(x => x.AreCollisionsForWorkplaceByDatesExist(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken))
            .ReturnsAsync(false);

        _getWorkspaceAvailableStatusDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(false);

        _getWorkPlaceByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken)).ReturnsAsync(workplace);

        MockDbSets();

        var bookingComposer = new BookingEntryComposer(
            _bookingPortalSettingsAccessorMock.Object,
            authenticatedEmployee.Object,
            _areCollisionsExistBusinessRuleMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object,
            _getWorkspaceAvailableStatusDbQueryMock.Object,
            _bookingDbContextMock.Object
            );

        // Act
        var act = () => bookingComposer.Compose(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<InvalidDataAppException>();
    }

    private static Mock<IAuthenticatedUserContext> AuthenticatedCeoMock()
    {
        var authenticatedUserContext = new Mock<IAuthenticatedUserContext>();
        authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        authenticatedUserContext.Setup(x => x.DepartmentId).Returns(null as string);
        authenticatedUserContext.Setup(x => x.UserId).Returns(1);
        authenticatedUserContext.Setup(x => x.GetRequiredUserId()).Returns(1);

        return authenticatedUserContext;
    }

    private static Mock<IAuthenticatedUserContext> AuthenticatedEmployeeMock()
    {
        var authenticatedUserContext = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
        authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Js");
        authenticatedUserContext.Setup(x => x.UserId).Returns(1);
        authenticatedUserContext.Setup(x => x.GetRequiredUserId()).Returns(1);

        return authenticatedUserContext;
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
