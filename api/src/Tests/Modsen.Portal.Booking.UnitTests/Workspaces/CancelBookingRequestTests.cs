using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Workplaces.Requests;
using Modsen.Portal.Booking.Requests.Workplaces.Requests.DbQueries;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workspaces;

public class CancelBookingRequestTests
{
    private readonly Mock<IBookingDbContext> _booingDbContextMock = new();
    private readonly Mock<IRemoveBookingsRangeDbQuery> _removeBookingsRangeDbQueryMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<ICommunicationBus> _communicationBusMock = new();

    private readonly CancelBookingRequest _cancelBookingRequest;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CancelBookingRequestTests()
    {
        var canCurrentUserDeleteBooking = new CanCurrentUserDeleteBooking(_authenticatedUserContextMock.Object);

        _cancelBookingRequest = new CancelBookingRequest(
            _booingDbContextMock.Object,
            canCurrentUserDeleteBooking,
            _removeBookingsRangeDbQueryMock.Object,
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object,
            _communicationBusMock.Object);
    }

    [Fact]
    public async Task HandlerBookingDoesNotExistsShouldEntityNotFoundException()
    {
        // Arrange
        var bookingId = 123;
        var command = new CancelBookingRequest.Command(bookingId);

        SetupAuthenticatedUserContextDepartment("Net");
        SetupBookings([]);

        // Act
        var action = () => _cancelBookingRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>().WithMessage("Booking with id = 123 does not exists");
    }

    [Fact]
    public async Task HandlerHeadOfDepartmentTryDeleteBookingOfUserFromOtherDepartmentShouldThrowActionNotAllowedException()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = true,
            UserId = userId,
            User = new()
            {
                DepartmentId = "Mobile",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextRoles([PortalRoleNames.HeadOfDepartment]);
        SetupAuthenticatedUserContextUserId(15);
        SetupBookings([bookingToReturn]);

        // Act
        var action = () => _cancelBookingRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>().WithMessage("User with id = 15 does not have permissions to delete booking of Users from other department!");
    }

    [Fact]
    public async Task HandlerUserDoesNotHavePermissionToDeleteBookingOfOtherUserShouldThrowActionNotAllowedException()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = true,
            UserId = 2,
            User = new()
            {
                DepartmentId = "Mobile",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextRoles([PortalRoleNames.Employee]);
        SetupAuthenticatedUserContextUserId(userId);
        SetupBookings([bookingToReturn]);

        // Act
        var action = () => _cancelBookingRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>().WithMessage("User with id = 1 does not have permissions to delete booking of other Users!");
    }

    [Fact]
    public async Task HandlerUserDoesNotHavePermissionToDeleteBlockShouldThrowActionNotAllowedException()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = true,
            UserId = userId,
            User = new()
            {
                DepartmentId = "Mobile",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextRoles([PortalRoleNames.Employee]);
        SetupAuthenticatedUserContextUserId(userId);
        SetupBookings([bookingToReturn]);

        // Act
        var action = () => _cancelBookingRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>().WithMessage("User with id = 1 does not have permissions to delete block booking!");
    }

    [Fact]
    public async Task HandlerSelfDeleteBookingDeleteBooking()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = false,
            UserId = userId,
            User = new()
            {
                DepartmentId = "Net",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextUserId(userId);
        SetupBookings([bookingToReturn]);

        // Act
        var result = await _cancelBookingRequest.Handle(command, _cancellationToken);

        //Assert
        result.Should().Be(bookingId);
    }

    [Fact]
    public async Task HandlerManagerDeleteSelfBookingDeleteBooking()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = false,
            UserId = userId,
            User = new()
            {
                DepartmentId = "Net",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextRoles([PortalRoleNames.Ceo]);
        SetupAuthenticatedUserContextUserId(userId);
        SetupBookings([bookingToReturn]);

        // Act
        var result = await _cancelBookingRequest.Handle(command, _cancellationToken);

        //Assert
        result.Should().Be(bookingId);
    }

    [Fact]
    public async Task HandlerManagerDeleteNotSelfBookingDeleteBookingAndCreateNotification()
    {
        //Arrange
        var bookingId = 123;
        var userId = 1;
        var managerId = 2;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);

        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = false,
            UserId = userId,
            User = new()
            {
                DepartmentId = "Net",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        var workplace = new Workplace
        {
            Id = workplaceId,
            BookingEntries = [bookingToReturn]
        };

        SetupClockServiceUtcNow(DateTime.UtcNow);
        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextUserId(managerId);
        SetupAuthenticatedUserContextRoles([PortalRoleNames.Ceo]);
        SetupAuthenticatedUserContextIsManager(true);
        SetupBookings([bookingToReturn]);
        SetupWorkplaces([workplace]);
        SetupReservations([]);

        // Act
        var result = await _cancelBookingRequest.Handle(command, _cancellationToken);

        //Assert
        result.Should().Be(bookingId);
    }

    [Fact]
    public async Task HandlerManagerDeleteBookingWithoutPermissionsShouldApplicationException()
    {
        //Arrange
        var bookingId = 123;
        var currentUserId = 1;
        var bookingUserId = 1832;
        var workplaceId = 1;
        var command = new CancelBookingRequest.Command(bookingId);
        var bookingToReturn = new BookingEntry
        {
            Id = bookingId,
            IsBlock = false,
            UserId = bookingUserId,
            User = new()
            {
                DepartmentId = "Net",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test"
            },
            WorkplaceId = workplaceId
        };

        SetupAuthenticatedUserContextDepartment("Net");
        SetupAuthenticatedUserContextUserId(currentUserId);
        SetupAuthenticatedUserContextRoles([PortalRoleNames.Employee]);
        SetupBookings([bookingToReturn]);

        // Act
        var action = () => _cancelBookingRequest.Handle(command, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>();
    }

    private void SetupBookings(List<BookingEntry> bookings)
    {
        _booingDbContextMock
            .Setup(mock => mock.Bookings)
            .Returns(bookings.BuildMockDbSet().Object);
    }

    private void SetupWorkplaces(List<Workplace> workplaces)
    {
        _booingDbContextMock
            .Setup(mock => mock.Workplaces)
            .Returns(workplaces.BuildMockDbSet().Object);
    }

    private void SetupReservations(List<Reservation> reservations)
    {
        _booingDbContextMock
            .Setup(mock => mock.Reservations)
            .Returns(reservations.BuildMockDbSet().Object);
    }

    private void SetupAuthenticatedUserContextDepartment(string? departmentId)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.DepartmentId)
            .Returns("Net");
    }

    private void SetupAuthenticatedUserContextRoles(string[] roles) 
    {
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns(roles);
    }

    private void SetupAuthenticatedUserContextUserId(int? userId)
    {
        _authenticatedUserContextMock
            .Setup(x => x.UserId)
            .Returns(userId);
    }

    private void SetupClockServiceUtcNow(DateTime dateTime)
    {
        _clockServiceMock
            .Setup(x => x.UtcNow)
            .Returns(dateTime);
    }

    private void SetupAuthenticatedUserContextIsManager(bool isManager)
    {
        _authenticatedUserContextMock
            .Setup(x => x.IsManager)
            .Returns(isManager);
    }
}
