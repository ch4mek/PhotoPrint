using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.UseCases.BookingTests.Command.CreateMultipleManagerBooking;

public class CreateMultipleManagerBookingTests
{
    private readonly Mock<IBookingEntryComposer> _bookingComposerMock = new();
    private readonly Mock<IGetUserByIdDbQuery> _getUserByIdDbQueryMock = new();
    private readonly Mock<IIsCurrentUserInDepartmentBusinessRule> _isCurrentUserInDepartmentBusinessRuleMock = new();
    private readonly Mock<IIsCurrentUserInResourceGroupBusinessRule> _isCurrentUserInResourceGroupMock = new();
    private readonly Mock<IIsAllowedToBookBusinessRule> _isAllowedToBookBusinessRuleMock = new();
    private readonly Mock<IBookingEntryLoader> _bookingEntryLoaderMock = new();
    private readonly Mock<IBookingPortalSettingsAccessor> _settingsAccessor = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CreateMultipleManagerBookingTests()
    {
        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task CreateBookingByManagerWhenBookingIsMultipleAndDataIsCorrectWithCeoRoleForEmployeeFromNetDepShouldReturnCreatedBooking()
    {
        //Arrange
        var user = new BookingUserEntity
        {
            UserId = 2,
            DepartmentId = "Net",
            Email = "modsennet@modsen-software.com",
            FirstName = "User",
            LastName = "FromNet"
        };

        var request = new CreateBookingByManagerRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 2
        };

        var settings = await _settingsAccessor.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var bookingId = 1;

        var bookingDto = new BookingEntryCoreDto
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            BookingId = bookingId,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        var bookingEntry = new BookingEntry
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            Id = bookingId
        };

        var authenticatedCeo = AuthenticatedCeoMock();

        _bookingComposerMock.Setup(x => x.Compose(It.IsAny<BookingEntryComposerRequest>(), _cancellationToken))
            .ReturnsAsync(bookingEntry);

        _getUserByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(bookingDto);

        _isCurrentUserInDepartmentBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<string>()))
            .Returns(true);

        _isCurrentUserInResourceGroupMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int?>()))
            .Returns(true);

        _isAllowedToBookBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateBookingByManagerRequest(
            _bookingComposerMock.Object,
            _getUserByIdDbQueryMock.Object,
            _isCurrentUserInDepartmentBusinessRuleMock.Object,
            _isCurrentUserInResourceGroupMock.Object,
            _bookingEntryLoaderMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);

        //Act
        var result = await handler.Handle(request, _cancellationToken);

        //Assert
        user.UserId.Should().NotBe(authenticatedCeo.Object.UserId);
        request.UserId.Should().Be(user.UserId);
        result.Should().BeEquivalentTo(bookingDto);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task CreateBookingByManagerWhenBookingIsMultipleAndDataIsCorrectWithHeadOfNetRoleForEmployeeFromNetDepShouldReturnCreatedBooking()
    {
        //Arrange
        var user = new BookingUserEntity
        {
            UserId = 2,
            DepartmentId = "Net",
            Email = "modsennet@modsen-software.com",
            FirstName = "User",
            LastName = "FromNet"
        };

        var request = new CreateBookingByManagerRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 2
        };
        var settings = await _settingsAccessor.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var bookingId = 1;

        var bookingDto = new BookingEntryCoreDto
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            BookingId = bookingId,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        var bookingEntry = new BookingEntry
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            Id = bookingId
        };

        var authenticatedHead = AuthenticatedHeadOfNetMock();

        _bookingComposerMock.Setup(x => x.Compose(It.IsAny<BookingEntryComposerRequest>(), _cancellationToken))
            .ReturnsAsync(bookingEntry);

        _getUserByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(bookingDto);

        _isCurrentUserInDepartmentBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<string>()))
            .Returns(true);

        _isCurrentUserInResourceGroupMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int?>()))
            .Returns(true);

        _isAllowedToBookBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateBookingByManagerRequest(
            _bookingComposerMock.Object,
            _getUserByIdDbQueryMock.Object,
            _isCurrentUserInDepartmentBusinessRuleMock.Object,
            _isCurrentUserInResourceGroupMock.Object,
            _bookingEntryLoaderMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);

        //Act
        var result = await handler.Handle(request, _cancellationToken);

        //Assert
        user.UserId.Should().NotBe(authenticatedHead.Object.UserId);
        request.UserId.Should().Be(user.UserId);
        user.DepartmentId.Should().Be(authenticatedHead.Object.DepartmentId);
        result.Should().BeEquivalentTo(bookingDto);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    [Fact]
    public async Task CreateBookingByManagerWhenBookingIsMultipleAndDataIsCorrectWithHeadOfNetRoleForEmployeeFromJsDepShouldThrowActionNotAllowedException()
    {
        //Arrange
        var user = new BookingUserEntity
        {
            UserId = 4,
            DepartmentId = "Js",
            Email = "modsenjs@modsen-software.com",
            FirstName = "User",
            LastName = "FromJs"
        };

        var request = new CreateBookingByManagerRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 4
        };

        _isCurrentUserInDepartmentBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<string>()))
            .Returns(true);

        _getUserByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        var handler = new CreateBookingByManagerRequest(
           _bookingComposerMock.Object,
           _getUserByIdDbQueryMock.Object,
           _isCurrentUserInDepartmentBusinessRuleMock.Object,
           _isCurrentUserInResourceGroupMock.Object,
           _bookingEntryLoaderMock.Object,
           _isAllowedToBookBusinessRuleMock.Object);

        // Act
        var act = () => handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task CreateBookingByManagerWhenBookingIsMultipleAndDataIsCorrectWithGroupManagerRoleForEmployeeFromOtherGroupShouldThrowActionNotAllowedException()
    {
        //Arrange
        var user = new BookingUserEntity
        {
            UserId = 4,
            DepartmentId = "Net",
            ResourceGroupId = 1,
            Email = "modsenjs@modsen-software.com",
            FirstName = "User",
            LastName = "FromJs"
        };

        var request = new CreateBookingByManagerRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 4
        };

        _getUserByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        _isCurrentUserInDepartmentBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<string>()))
            .Returns(true);

        _isCurrentUserInResourceGroupMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int?>()))
            .Returns(false);

        var handler = new CreateBookingByManagerRequest(
           _bookingComposerMock.Object,
           _getUserByIdDbQueryMock.Object,
           _isCurrentUserInDepartmentBusinessRuleMock.Object,
           _isCurrentUserInResourceGroupMock.Object,
           _bookingEntryLoaderMock.Object,
           _isAllowedToBookBusinessRuleMock.Object);

        // Act
        var act = () => handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task CreateBookingByManagerWhenBookingIsMultipleAndDataIsCorrectWithCeoRoleForHimselfShouldReturnCreatedBooking()
    {
        //Arrange
        var user = new BookingUserEntity
        {
            UserId = 1,
            DepartmentId = null,
            Email = "modsenceo@modsen-software.com",
            FirstName = "Ceo",
            LastName = "Ceo"
        };

        var request = new CreateBookingByManagerRequest.Command
        {
            FromDate = new DateTime(2023, 06, 16),
            ToDate = new DateTime(2023, 06, 20),
            WeeksInInterval = 0,
            WorkplaceId = 1,
            UserId = 1
        };
        var settings = await _settingsAccessor.Object.GetSettings(_cancellationToken);
        var intervalInDays = request.WeeksInInterval > 0 && request.ToDate == request.FromDate ? 7 * request.WeeksInInterval : 1;
        var toDate = request.WeeksInInterval > 0 ? request.ToDate.AddMonths(settings.ReservationsMonthCount) : request.ToDate;
        var dates = DatesGenerator.GetDatesRangeTillDate(request.FromDate, toDate, intervalInDays).ToList();

        var bookingId = 1;

        var bookingDto = new BookingEntryCoreDto
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            BookingId = bookingId,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        var bookingEntry = new BookingEntry
        {
            UserId = user.UserId,
            WeeksInInterval = request.WeeksInInterval,
            WorkplaceId = request.WorkplaceId,
            Id = bookingId
        };

        var authenticatedCeo = AuthenticatedCeoMock();

        _bookingComposerMock.Setup(x => x.Compose(It.IsAny<BookingEntryComposerRequest>(), _cancellationToken))
            .ReturnsAsync(bookingEntry);

        _getUserByIdDbQueryMock.Setup(x => x.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(user);

        _bookingEntryLoaderMock.Setup(x => x.Load(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(bookingDto);

        _isCurrentUserInDepartmentBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<string>()))
            .Returns(true);

        _isCurrentUserInResourceGroupMock.Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int?>()))
            .Returns(true);

        _isAllowedToBookBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateBookingByManagerRequest(
            _bookingComposerMock.Object,
            _getUserByIdDbQueryMock.Object,
            _isCurrentUserInDepartmentBusinessRuleMock.Object,
            _isCurrentUserInResourceGroupMock.Object,
            _bookingEntryLoaderMock.Object,
            _isAllowedToBookBusinessRuleMock.Object);

        //Act
        var result = await handler.Handle(request, _cancellationToken);

        //Assert
        user.UserId.Should().Be(authenticatedCeo.Object.UserId);
        result.Should().BeEquivalentTo(bookingDto);
        intervalInDays.Should().Be(1);
        toDate.Should().Be(request.ToDate);
        dates.Count.Should().Be(5);
    }

    private static Mock<IAuthenticatedUserContext> AuthenticatedCeoMock()
    {
        var authenticatedUserContext = new Mock<IAuthenticatedUserContext>();
        authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Ceo]);
        authenticatedUserContext.Setup(x => x.DepartmentId).Returns(null as string);
        authenticatedUserContext.Setup(x => x.UserId).Returns(1);

        return authenticatedUserContext;
    }

    private static Mock<IAuthenticatedUserContext> AuthenticatedHeadOfNetMock()
    {
        var authenticatedUserContext = new Mock<IAuthenticatedUserContext>();
        authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        authenticatedUserContext.Setup(x => x.DepartmentId).Returns("Net");
        authenticatedUserContext.Setup(x => x.UserId).Returns(3);

        return authenticatedUserContext;
    }
}
