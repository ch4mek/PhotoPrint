using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateMultipleBookingWhereMultipleBookingOfOtherDeskForTheSameDayExistsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateMultipleBookingWhereMultipleBookingOfOtherDeskForTheSameDayExistsScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task EmployeeCreateMultipleBookingWhereMultipleBookingOfOtherDeskForTheSameDayAlreadyExistsScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Verification of unavailability of the Multiple booking creating for the same day where multiple booking of other desk for the same User exists scenario");

        await scenario.Step("Login", async () =>
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
                return await httpClient.GetBookings(officeId: _testFactory.MinskOfficeId, includeEmployee: true);
            });

            if (userReservations?.Bookings != null)
            {
                foreach (var booking in userReservations.Bookings)
                {
                    await httpClient.DeleteBookingAsHttpMessage(booking.BookingId);
                }
            }
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            await httpClient.AuthAsCeo();

            return await scenario.Step(
                "Call 'Users' endpoint to create new user", async () =>
                {
                    var createdUser = await scenario.Step("Create new user", async () =>
                    {
                        return await httpClient.CreateEmployee();
                    });

                    return createdUser;
                });
        });

        var workplace = _testFactory.WorkplaceIdInMinskOffice + 10;
        var fromDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(4);
        var toDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(6);

        await scenario.Step("Add multiple booking", async () =>
        {
            await httpClient.AuthAs(createdUser!.Email);

            var beforeBookReservations = await scenario.Step("Get all reservations for this period", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step($"Check if workplace #{workplace} has no reservations for these days", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplace))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplace}", async () =>
            {
                return await httpClient.GetCollisions(
                    userId: createdUser.Id,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplace,
                    dateFrom: fromDate,
                    dateTo: toDate);
            });

            await scenario.Step($"Check if workplace #{workplace} has no collisions for these days", () =>
            {
                collisions.Should().BeEmpty();
            });

            await httpClient.AuthAs(createdUser.Email);

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplace, fromDate: fromDate, toDate: toDate);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplace #{workplace} was booked successfully for the specified period", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                var bookedWorkplace = afterBookReservations.SelectMany(r => r.UserWorkplaces)
                        .FirstOrDefault(x => x.WorkplaceId == workplace && x.UserId == currentUser.Id);
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplace);
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });

        await scenario.Step("Add multiple booking", async () =>
        {
            var localScopedWorkplace = _testFactory.WorkplaceIdInMinskOffice + 11;

            await httpClient.AuthAs(createdUser!.Email);

            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(
                    workplaceId: localScopedWorkplace,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step($"Verify if booking for workplace #{localScopedWorkplace} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });
    }
}
