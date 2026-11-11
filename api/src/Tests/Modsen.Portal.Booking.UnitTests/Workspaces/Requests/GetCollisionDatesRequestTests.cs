using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workspaces.Requests;

public class GetCollisionDatesRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IBookingPortalSettingsAccessor> _settingsAccessorMock = new();
    private readonly Mock<ICollisionsForDatesGenerator> _collisionsForDatesGeneratorMock = new();
    private readonly Mock<IGetWorkPlaceByIdDbQuery> _getWorkPlaceByIdDbQueryMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetCollisionDatesRequest _getCollisionDatesRequest;

    public GetCollisionDatesRequestTests()
    {
        _getCollisionDatesRequest = new GetCollisionDatesRequest(
            _collisionsForDatesGeneratorMock.Object,
            _authenticatedUserContextMock.Object,
            _settingsAccessorMock.Object,
            _getWorkPlaceByIdDbQueryMock.Object);

        _settingsAccessorMock
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingPortalSettingsDto
            {
                ReservationsMonthCount = 3
            });
    }

    [Fact]
    public async Task GetCollisionsWhenWorkplaceNotFoundThrowEntityNotFoundException()
    {
        // arrange
        var userId = 1;
        var workplaceId = 1;

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _getWorkPlaceByIdDbQueryMock.Setup(db => db.Get(workplaceId, _cancellationToken))
            .ReturnsAsync(default(Workplace));

        var query = new GetCollisionDatesRequest.Query(DateTime.Now, DateTime.Now, workplaceId, 1, userId);

        // act
        var result = () => _getCollisionDatesRequest.Handle(query, _cancellationToken);

        // assert
        await result.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.NotExistingWorkspace);
    }

    [Fact]
    public async Task GetCollisionsWhenNonRepetitiveReturnOneCollision()
    {
        // arrange
        var userId = 99;

        var workplace = new Workplace
        {
            Id = 99,
            OfficeId = 99,
            WorkplaceNumber = 99
        };

        var collisions = new List<DateTime> { new(2023, 01, 05) };

        var query = new GetCollisionDatesRequest.Query(
            new DateTime(2023, 01, 01),
            new DateTime(2023, 01, 15),
            workplace.Id,
            0,
            userId);


        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);


        _getWorkPlaceByIdDbQueryMock.Setup(db => db.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(workplace);

        _collisionsForDatesGeneratorMock.Setup(db => db.GenerateCollisionsForDates(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken
            ))
            .ReturnsAsync(collisions);

        // act
        var result = await _getCollisionDatesRequest.Handle(query, _cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collisions, opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task GetCollisionsWhenNonRepetitiveReturnAllCollisions()
    {
        // arrange
        var userId = 99;

        var workplace = new Workplace
        {
            Id = 99,
            OfficeId = 99,
            WorkplaceNumber = 99
        };

        var collisions = new List<DateTime> {
                new(2023, 01, 05),
                new(2023, 01, 06),
                new(2023, 01, 09),
            };

        var query = new GetCollisionDatesRequest.Query(
            new DateTime(2023, 01, 01),
            new DateTime(2023, 01, 15),
            workplace.Id,
            0,
            userId);


        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _getWorkPlaceByIdDbQueryMock.Setup(db => db.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(workplace);

        _collisionsForDatesGeneratorMock.Setup(db => db.GenerateCollisionsForDates(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken
            ))
            .ReturnsAsync(collisions);

        // act
        var result = await _getCollisionDatesRequest.Handle(query, _cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collisions, opt => opt.WithStrictOrdering());
    }

    [Fact]
    public async Task GetCollisionsWhenRepetitiveReturnOneCollision()
    {
        // arrange
        var userId = 99;

        var workplace = new Workplace
        {
            Id = 99,
            OfficeId = 99,
            WorkplaceNumber = 99
        };

        var collisions = new List<DateTime> { new(2023, 01, 01) };

        var query = new GetCollisionDatesRequest.Query(
            new DateTime(2023, 01, 01),
            new DateTime(2023, 01, 01),
            workplace.Id,
            1,
            userId);

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId())
            .Returns(userId);

        _getWorkPlaceByIdDbQueryMock.Setup(db => db.Get(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(workplace);

        _collisionsForDatesGeneratorMock.Setup(db => db.GenerateCollisionsForDates(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<List<DateTime>>(),
            _cancellationToken
            ))
            .ReturnsAsync(collisions);

        // act
        var result = await _getCollisionDatesRequest.Handle(query, _cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collisions, opt => opt.WithStrictOrdering());
    }
}

