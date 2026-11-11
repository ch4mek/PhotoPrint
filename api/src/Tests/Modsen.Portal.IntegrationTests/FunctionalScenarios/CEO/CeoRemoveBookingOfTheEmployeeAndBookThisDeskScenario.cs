using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoRemoveBookingOfTheEmployeeAndBookThisDeskScenario : IClassFixture<FunctionalTestFactory>
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoRemoveBookingOfTheEmployeeAndBookThisDeskScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task RemoveBookingOfTheEmployeeAndBookThisDeskScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO remove booking of the employee and book this desk scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            return await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        var workplaceId = _testFactory.WorkplaceIdInMinskOffice;
        var date = DateTime.UtcNow.Date.AddDays(1);

        var bookingForEmployeeId = await scenario.Step("Book desk for employee", async () =>
        {
            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no reservations for the current day",
                () =>
                {
                    reservations!.Should().NotBeNull();
                    reservations.Any(x => x.UserWorkplaces.Any(workplace => workplace.WorkplaceId == workplaceId))
                        .Should().BeFalse();
                });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceId}",
                async () =>
                {
                    return await httpClient.GetCollisions(officeId: _testFactory.MinskOfficeId,
                        workplaceId: workplaceId,
                        dateFrom: date,
                        dateTo: date);
                });

            await scenario.Step($"Check if workplace #{workplaceId} has no collisions for the current day",
                () => collisions.Should().BeEmpty());

            var createdBooking = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateUserBooking(workplaceId: workplaceId, fromDate: date, toDate: date,
                    userId: 4);
            });

            await scenario.Step("Get all reservations of current user", async () =>
            {
                await httpClient.GetBookings(userId: currentUser.Id, includeEmployee: true,
                    officeId: _testFactory.MinskOfficeId);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId, fromDate: date,
                    toDate: date);
            });

            var bookedWorkplace = reservationsAfterBook![0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceId);

            await scenario.Step($"Verify if workplace #{workplaceId} was booked successfully", () =>
            {
                reservationsAfterBook.Should().NotBeNull();
                reservationsAfterBook.Should().ContainSingle();

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceId);
                bookedWorkplace.IsBlocked.Should().Be(false);
            });

            return createdBooking;
        });

        await scenario.Step("Remove booking for employee", async () =>
        {
            var reservationsForDate = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step("Check if workplace is booked for the current day",
                () =>
                {
                    reservationsForDate!.Should().NotBeNull();
                    reservationsForDate.Any(x =>
                            x.UserWorkplaces.Any(workplace => workplace.WorkplaceId == workplaceId))
                        .Should().BeTrue();
                });

            var reservations = await scenario.Step("Get all reservations", async () =>
            {
                return await httpClient.GetBookings(officeId: _testFactory.MinskOfficeId, userId: currentUser.Id,
                    includeEmployee: true);
            });

            await scenario.Step("Check if reservation for employee exist",
                () =>
                {
                    reservations!.Should().NotBeNull();
                    reservations.Bookings.Should().Contain(reservation => reservation.BookingId == bookingForEmployeeId!);
                });

            var deletingResponse = await scenario.Step("Call DELETE endpoint", async () =>
            {
                return await httpClient.DeleteBookingAsHttpMessage(bookingForEmployeeId);
            });

            var reservationsAfterDeletingBooking = await scenario.Step("Get all reservations", async () =>
            {
                return await httpClient.GetBookings(officeId: _testFactory.MinskOfficeId, userId: currentUser.Id,
                    includeEmployee: true);
            });

            await scenario.Step("Get all reservations for the current day", async () =>
            {
                await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId, toDate: date,
                    fromDate: date);
            });

            await scenario.Step("Verify if booking was delete successfully", () =>
            {
                deletingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

                reservationsAfterDeletingBooking!.Bookings.Should()
                    .NotContain(reservation => reservation.BookingId == bookingForEmployeeId!);
            });
        });

        await scenario.Step("Create self booking for this date and workplace", async () =>
        {
            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no reservations for the current day", () =>
            {
                reservations!.Should().NotBeNull();
                reservations.Any(x => x.UserWorkplaces.Any(workplace => workplace.WorkplaceId == workplaceId))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceId}", async () =>
            {
                return await httpClient.GetCollisions(officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceId,
                    dateFrom: date,
                    dateTo: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                await httpClient.CreateSelfBooking(workplaceId: workplaceId, fromDate: date, toDate: date);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId, fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Verify if workplace #{workplaceId} was booked successfully", () =>
            {
                reservationsAfterBook!.Should().NotBeNull();
                reservationsAfterBook.Should().ContainSingle();

                var bookedWorkplace = reservationsAfterBook[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceId);

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceId);
                bookedWorkplace.FirstName.Should().Be("Test");
                bookedWorkplace.LastName.Should().Be("CEO");
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });
    }
}
