using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateRepetitiveBookingWhereSingleBookingExistsScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _helper;

    public EmployeeCreateRepetitiveBookingWhereSingleBookingExistsScenario(FunctionalTestFactory factory,
        ITestOutputHelper helper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _helper = helper;
    }

    [Fact]
    public async Task EmployeeCreateRepetitiveBookingForTheSameDayWhereSingleBookingOfOtherDeskForTheSameUserExistsScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_helper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Verification of unavailability of the repetitive booking creating for the same day where single booking of other desk for the same User exists scenario");

        await scenario.Step("Login as CEO ", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
            });
        });

        await scenario.Step("Delete user's reservations", async () =>
        {
            var userReservations = await scenario.Step("Get all reservations of current user", async () =>
            {
                return await httpClient.GetBookings(officeId: _factory.MinskOfficeId, includeEmployee: true);
            });

            if (userReservations?.Bookings != null)
            {

                foreach (var booking in userReservations.Bookings)
                {
                    await httpClient.DeleteBookingAsHttpMessage(booking.BookingId);
                }
            }
        });

        const int workplace = 10;
        var date = DateTime.UtcNow.Date.AddDays(10);

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            await httpClient.AuthAsCeo();

            return await scenario.Step("Call 'Users' endpoint to create new user", async () =>
            {
                var createdUser = await scenario.Step("Create new user", async () =>
                {
                    return await httpClient.CreateEmployee();
                });

                return createdUser;
            });
        });

        await scenario.Step("Add single booking", async () =>
        {
            await httpClient.AuthAs(createdUser!.Email);

            var beforeBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(
                    officeId: _factory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplace} has no reservations for the current day", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplace))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplace}", async () =>
            {
                return await httpClient.GetCollisions(userId: createdUser.Id,
                    officeId: _factory.MinskOfficeId,
                    workplaceId: workplace,
                    dateTo: date,
                    dateFrom: date);
            });

            await scenario.Step($"Check if workplace #{workplace} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplace, fromDate: date, toDate: date);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplace #{workplace} was booked successfully", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                afterBookReservations.Should().ContainSingle();
                var bookedWorkplace = afterBookReservations[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplace);
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplace);
                bookedWorkplace.IsBlocked.Should().Be(false);

                return Task.CompletedTask;
            });
        });

        await scenario.Step("Add repetitive booking", async () =>
        {
            await httpClient.AuthAs(createdUser!.Email);
            const int localScopedWorkplace = 11;
            const int weeksInInterval = 1;

            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(
                    workplaceId: localScopedWorkplace,
                    fromDate: date,
                    toDate: date,
                    weeksInInterval: weeksInInterval
                );
            });

            await scenario.Step($"Verify if booking for workplace #{localScopedWorkplace} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });
    }
}
