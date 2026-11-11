using System.Text.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Controllers.Reservations;

[Collection("GeneralCollection")]
public class ReservationsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly int _reservationWorkplaceId = 1205;
    private readonly int _reservationOfficeId = 6;
    private readonly int _reservationWorkplaceForBlockId = 1206;

    public ReservationsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetReservationsWithoutReservationsOnThisDatesReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var fromDate = DateTime.UtcNow.Date.AddDays(-5).Date;
        var toDate = DateTime.UtcNow.AddDays(-3).Date;

        // Act
        var reservationForDates = await client.GetReservation(fromDate, toDate, null);

        // Assert
        reservationForDates.Should().NotBeNull();
        reservationForDates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReservationsWhenBookingExistInOneOfficeAndTryToGetReservationsInAnotherOfficeReturnEmptyList()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = 4;
        var weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.AddDays(20).Date;
        var lastDayOfBooking = DateTime.UtcNow.AddDays(23).Date;
        var lastDayOfReservationRequest = DateTime.UtcNow.AddDays(23).Date;
        var officeId = 1000;

        await client.CreateSelfBooking(workplaceId, currentDate, lastDayOfBooking, weeksInInterval);

        // Act
        var reservationForDates = await client.GetReservation(currentDate, lastDayOfReservationRequest, officeId);

        // Assert
        reservationForDates.Should().NotBeNull();
        reservationForDates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReservationsWhenBookingExistInOneOfficeAndTryToGetReservationsInThisOfficeReturnListWithBookedDatesAndWorkplaces()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var workplaceId = _reservationWorkplaceId;
        var weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.Date;
        var lastDayOfBooking = DateTime.UtcNow.AddDays(3).Date;
        var lastDayOfReservationRequest = DateTime.UtcNow.AddDays(2).Date;
        var officeId = _reservationOfficeId;

        await client.CreateSelfBooking(workplaceId, currentDate, lastDayOfBooking, weeksInInterval);

        // Act
        var reservationForDates = await client.GetReservation(currentDate, lastDayOfReservationRequest, officeId);

        // Assert
        reservationForDates.Should().NotBeNullOrEmpty();
        reservationForDates.Should().HaveCount(3);
        reservationForDates.TrueForAll(reservationForDate => reservationForDate.Date >= currentDate &&
            reservationForDate.Date <= lastDayOfReservationRequest).Should().BeTrue();
    }

    [Fact]
    public async Task GetReservationsWithExistBookingReturnListWithBookedDatesAndWorkplaces()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.Date.AddDays(30).Date;
        var lastDayOfBooking = DateTime.UtcNow.AddDays(33).Date;
        var lastDayOfReservationRequest = DateTime.UtcNow.AddDays(32).Date;
        var workplaceId = _reservationWorkplaceId;

        await client.CreateSelfBooking(workplaceId, currentDate, lastDayOfBooking, weeksInInterval);

        // Act
        var reservationForDates = await client.GetReservation(currentDate, currentDate, null);

        // Assert
        reservationForDates.Should().NotBeNullOrEmpty();
        reservationForDates.Should().HaveCount(1);
        reservationForDates.TrueForAll(reservationForDate => reservationForDate.Date >= currentDate &&
            reservationForDate.Date <= lastDayOfReservationRequest).Should().BeTrue();
    }

    [Fact]
    public async Task GetReservationsWhenExistBlockReturnListWithBookedDatesAndWorkplaces()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var workplaceId = _reservationWorkplaceForBlockId;
        var weeksInInterval = 0;
        var currentDate = DateTime.UtcNow.Date.AddDays(4).Date;
        var lastDayOfBooking = DateTime.UtcNow.AddDays(5).Date;

        await client.CreateBlockBooking(workplaceId, currentDate, lastDayOfBooking, null, weeksInInterval);

        var expectedWorkplaces = new List<UserWorkplace>
        {
            new()
            {
                WorkplaceId = workplaceId,
                WorkplaceOfficeId = _reservationOfficeId,
                UserId = _factory.CeoUserId,
                FirstName = "FirstName",
                LastName = "LastName",
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                DepartmentId = null,
                IsBlocked = true
            }
        };

        await client.AuthAsEmployee();

        // Act
        var reservationForDates = await client.GetReservation(currentDate, currentDate, null);

        // Assert
        reservationForDates.Should().NotBeNullOrEmpty();
        reservationForDates.Should().Contain(r => r.Date == currentDate);
    }
}
