using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateSingleBookingWhenSingleBookingExistsForSameDayScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateSingleBookingWhenSingleBookingExistsForSameDayScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateSingleBookingWhenSingleBookingExistsForSameDayScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee cannot create single booking for the same day when another single booking already exists for other desk for this user scenario");

        var date = DateTime.UtcNow.Date.AddDays(1);
        const int workplaceNumber = 14;

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
                return await httpClient.GetBookings(officeId: _testFactory.MinskOfficeId,
                    includeEmployee: true);
            });

            if (userReservations?.Bookings != null)
            {

                foreach (var booking in userReservations.Bookings)
                {
                    await httpClient.DeleteBookingAsHttpMessage(booking.BookingId);
                }
            }
        });

        await scenario.Step("Login as Employee", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Create a single booking for user", async () =>
        {
            var beforeBookReservations = await scenario.Step("Get all reservations for the selected day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no reservations for the selected day", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplaceNumber))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplaceNumber #{workplaceNumber}", async () =>
            {
                return await httpClient.GetCollisions(userId: _testFactory.EmployeeUserId,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceNumber,
                    dateFrom: date,
                    dateTo: date);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no collisions for the selected day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplaceNumber,
                    fromDate: date,
                    toDate: date);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the selected day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplaceNumber #{workplaceNumber} was booked successfully", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                afterBookReservations.Should().ContainSingle();
                var bookedWorkplace = afterBookReservations[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceNumber);
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceNumber);
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });

        const int anotherWorkplaceNumber = 12;

        await scenario.Step("Attempt to create another single booking for the same day", async () =>
        {

            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(workplaceId: anotherWorkplaceNumber,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Verify if booking for workplace #{anotherWorkplaceNumber} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        await scenario.Step("Verify that the first booking still exists", async () =>
        {
            var reservations = await httpClient.GetReservation(
                officeId: _testFactory.MinskOfficeId,
                fromDate: date,
                toDate: date);

            reservations.Should().NotBeNull();
            reservations.Should().ContainSingle(res => res.UserWorkplaces.Any(x => x.WorkplaceId == workplaceNumber));
            reservations.Should().ContainSingle(res => res.UserWorkplaces.Any(x => x.UserId == _testFactory.EmployeeUserId));
        });
    }
}
