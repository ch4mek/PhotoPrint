using System.Net;
using System.Text.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.Booking.Contracts;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("UnstableFuncScenarioCollection")]
public class EmployeeRemoveBookingScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeRemoveBookingScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task RemoveBookingScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee delete his booking scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => { await httpClient.AuthAsEmployee(); });

            var user = await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });

            return user;
        });

        var userReservations = await scenario.Step("Create booking", async () =>
        {
            var workplaceId = _testFactory.WorkplaceIdInWarsawOffice;
            var date = DateTime.UtcNow.Date.AddDays(1);

            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.WarsawOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no reservations for the current day",
                () =>
                {
                    reservations!.Should().NotBeNull();
                    reservations.Any(x => x.UserWorkplaces.Any(f => f.WorkplaceId == workplaceId))
                        .Should().BeFalse();
                });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceId}",
                async () =>
                {
                    return await httpClient.GetCollisions(userId: _testFactory.EmployeeUserId,
                        officeId: _testFactory.WarsawOfficeId,
                        workplaceId: workplaceId,
                        dateFrom: date,
                        dateTo: date);
                });

            await scenario.Step($"Check if workplace #{workplaceId} has no collisions for the current day",
                () => collisions.Should().BeEmpty());

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplaceId,
                    FromDate = date,
                    ToDate = date
                };

                await httpClient.CreateSelfBooking(booking);
            });

            var userReservations = await scenario.Step("Get all reservations of current user", async () =>
            {
                return await httpClient.GetBookings(officeId: _testFactory.WarsawOfficeId,
                    userId: currentUser.Id,
                    includeEmployee: true);
            });

            await scenario.Step("Get all reservations for the current day", async () =>
            {
                await httpClient.GetReservation(officeId: _testFactory.WarsawOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            return userReservations;
        });

        await scenario.Step("Delete booking", async () =>
        {
            var date = DateTime.UtcNow.Date;
            var reservationToDelete = userReservations.Bookings[0];

            var deleteResponse = await scenario.Step("Call booking DELETE endpoint",
                async () =>
                {
                    return await httpClient.DeleteBookingAsHttpMessage(reservationToDelete.BookingId);
                });

            var reservations = await scenario.Step("Get all reservations of current user", async () =>
            {
                return await httpClient.GetBookings(officeId: _testFactory.ModsenClientId,
                    userId: currentUser.Id,
                    includeEmployee: true);
            });

            var bookingsForDate = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.WarsawOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step("Verify if booking was successfully deleted", () =>
            {
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

                reservations.Bookings.Should()
                    .NotContain(reservation => reservation.BookingId == reservationToDelete.BookingId);
                bookingsForDate.Should().NotContain(booking =>
                    booking.UserWorkplaces.Any(
                        workplace => workplace.WorkplaceId == reservationToDelete.WorkplaceId));
            });
        });
    }

}
