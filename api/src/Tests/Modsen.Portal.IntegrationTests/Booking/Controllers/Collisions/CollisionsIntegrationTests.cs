using System.Text.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.Booking.Contracts;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Controllers.Collisions;

[Collection("GeneralCollection")]
public class CollisionsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _collisionsWorkplaceId = 1207;
    private readonly int _collisionsWorkplaceForSeveralBookingsId = 1209;
    private readonly int _collisionsWorkplaceForSameBookingsId = 1208;
    private readonly int _collisionsOfficeForSeveralBookingsUsersId = 9;
    private readonly int _collisionsOfficeForSameUsersId = 8;
    private readonly int _collisionsOfficeId = 7;

    public CollisionsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetCollisionsWithBookedDatesReturnBookedDate()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceId;
        var officeId = _collisionsOfficeId;
        var weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.Date;

        await client.CreateSelfBooking(
            workplaceId,
            currentDate,
            currentDate.AddDays(2),
            weeksInInterval);

        await client.AuthAsAdmin();

        // Act
        var collisions = await client.GetCollisions(
            _factory.AdminUserId,
            workplaceId,
            officeId,
            currentDate,
            currentDate);

        // Assert
        collisions.Should().NotBeNullOrEmpty();
        collisions.Should().HaveCount(1);
        collisions.Should().Contain(currentDate);
    }

    [Fact]
    public async Task GetCollisionsWithBookedDatesBySameUserReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceForSameBookingsId;
        var officeId = _collisionsOfficeForSameUsersId;
        var weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.Date.AddDays(3);

        await client.CreateSelfBooking(
            workplaceId,
            currentDate,
            currentDate.AddDays(4),
            weeksInInterval);

        // Act
        var collisions = await client.GetCollisions(
            _factory.EmployeeUserId,
            workplaceId,
            officeId,
            currentDate.AddDays(5),
            currentDate.AddDays(11));

        // Assert
        collisions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCollisionsOnUnbookedDaysReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceId;
        var officeId = _collisionsOfficeId;
        var weeksInInterval = 0;
        var fromDate = DateTime.UtcNow.AddDays(9).Date;
        var toDate = DateTime.UtcNow.AddDays(11).Date;
        var getCollisionsDate = DateTime.UtcNow.AddDays(12).Date;

        await client.CreateSelfBooking(
            workplaceId,
            fromDate,
            toDate,
            weeksInInterval);

        // Act
        var collisions = await client.GetCollisions(
            _factory.EmployeeUserId,
            workplaceId,
            officeId,
            getCollisionsDate,
            getCollisionsDate);

        // Assert
        collisions.Should().NotBeNull();
        collisions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCollisionsForAnotherUserOnUnbookedDaysReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceId;
        var officeId = _collisionsOfficeId;
        var weeksInInterval = 0;
        var fromDate = DateTime.UtcNow.AddDays(5).Date;
        var toDate = DateTime.UtcNow.AddDays(7).Date;
        var getCollisionsDate = DateTime.UtcNow.AddDays(8).Date;

        await client.CreateSelfBooking(
            workplaceId,
            fromDate,
            toDate,
            weeksInInterval);

        await client.AuthAsCto();

        // Act
        var collisions = await client.GetCollisions(
            _factory.EmployeeUserId,
            workplaceId,
            officeId,
            getCollisionsDate,
            getCollisionsDate);

        // Assert
        collisions.Should().NotBeNull();
        collisions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCollisionsWithHalfOfBookedDaysByAnotherUserReturnListWithBookedDates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceId;
        var weeksInInterval = 0;
        var officeId = _collisionsOfficeId;
        var currentDate = DateTime.UtcNow.AddDays(29).Date;
        var endDayOfBooking = DateTime.UtcNow.AddDays(31).Date;
        var getCollisionsDate = DateTime.UtcNow.AddDays(32).Date;

        await client.CreateSelfBooking(
            workplaceId,
            currentDate,
            endDayOfBooking,
            weeksInInterval);

        await client.AuthAsAdmin();

        // Act
        var collisions = await client.GetCollisions(
            _factory.AdminUserId,
            workplaceId,
            officeId,
            endDayOfBooking,
            getCollisionsDate);

        // Assert
        collisions.Should().NotBeNullOrEmpty();
        collisions.Should().HaveCount(1);
        collisions.Should().Contain(endDayOfBooking);
    }

    [Fact]
    public async Task GetCollisionsWithHalfOfBookedDaysBySameUserReturnListWithBookedDates()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceId;
        var weeksInInterval = 0;
        var officeId = _collisionsOfficeId;
        var currentDate = DateTime.UtcNow.AddDays(47).Date;
        var endDayOfBooking = DateTime.UtcNow.AddDays(48).Date;
        var getCollisionsDate = DateTime.UtcNow.AddDays(49).Date;

        await client.CreateSelfBooking(
            workplaceId,
            currentDate,
            endDayOfBooking,
            weeksInInterval);

        // Act
        var collisions = await client.GetCollisions(
            _factory.EmployeeUserId,
            workplaceId,
            officeId,
            endDayOfBooking,
            getCollisionsDate);

        // Assert
        collisions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetCollisionsWhenDateIsBookedInOneOfficeAndGetCollisionsForAnotherOfficeReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _factory.WorkplaceIdInMinksOffice;
        var warsawWorkplaceId = _factory.WorkplaceIdInWarsawOffice;
        var weeksInInterval = 0;
        var dateOfBooking = DateTime.UtcNow.AddDays(9).Date;

        await client.CreateSelfBooking(
            workplaceId,
            dateOfBooking,
            dateOfBooking,
            weeksInInterval);

        // Act
        var collisions = await client.GetCollisions(
            _factory.EmployeeUserId,
            warsawWorkplaceId,
            _factory.WarsawOfficeId,
            dateOfBooking,
            dateOfBooking);

        // Assert
        collisions.Should().NotBeNull();
        collisions.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCollisionsWhenSeveralBookingsOnOneDeskCreatedInReverseOrderInTimeShouldReturnCollisionsInCorrectOrder()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _collisionsWorkplaceForSeveralBookingsId;
        var weeksInInterval = 0;
        var officeId = _collisionsOfficeForSeveralBookingsUsersId;

        var createSelfBooknigInputs = new List<CreateSelfBookingTestInput>
        {
            new()
            {
                RequestType = CreateBookingBaseInput.CreateSelf,
                FromDate = DateTime.UtcNow.AddDays(17).Date,
                ToDate = DateTime.UtcNow.AddDays(19).Date,
                WeeksInInterval = weeksInInterval,
                WorkplaceId = workplaceId
            },
            new()
            {
                RequestType = CreateBookingBaseInput.CreateSelf,
                FromDate = DateTime.UtcNow.AddDays(14).Date,
                ToDate = DateTime.UtcNow.AddDays(16).Date,
                WeeksInInterval = weeksInInterval,
                WorkplaceId = workplaceId
            },
            new()
            {
                RequestType = CreateBookingBaseInput.CreateSelf,
                FromDate = DateTime.UtcNow.AddDays(12).Date,
                ToDate = DateTime.UtcNow.AddDays(13).Date,
                WeeksInInterval = weeksInInterval,
                WorkplaceId = workplaceId
            }
        };

        var bookingIds = new List<int>();

        foreach (var input in createSelfBooknigInputs)
        {
            var bookingId = await client.CreateSelfBooking(
                input.WorkplaceId,
                input.FromDate,
                input.ToDate,
                weeksInInterval);

            bookingIds.Add(bookingId);
        }

        var fromDate  = DateTime.UtcNow.AddDays(11).Date;
        var toDate = DateTime.UtcNow.AddDays(20).Date;
        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var collisions = await client.GetCollisions(
            userId,
            workplaceId,
            officeId,
            fromDate,
            toDate);

        // Assert
        collisions.Should().NotBeNullOrEmpty();
        collisions.First().Should().Be(createSelfBooknigInputs[2].FromDate);
        collisions.Last().Should().Be(createSelfBooknigInputs[0].ToDate);
    }
}
