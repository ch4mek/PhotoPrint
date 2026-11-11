using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateSingleBookingWithRepetitiveBookingOnTheOtherDeskScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateSingleBookingWithRepetitiveBookingOnTheOtherDeskScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateSingleBookingWithRepetitiveBookingOnTheOtherDeskScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
        scenarioPrefix: "[Employee]",
        title: "Employee cannot create single booking for the day where repetitive booking already exists for other desk for this user scenario");

        const int workplaceNumber = 14;
        const int weeksInInterval = 2;
        var date = DateTime.UtcNow.Date.AddDays(1);
        var weeksFromNowDate = date.AddDays(weeksInInterval * 7).Date;

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

        await scenario.Step("Login as Employee ", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Create a repetitive booking for user", async () =>
        {
            var beforeBookReservations = await scenario.Step("Get all reservations for the period", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: weeksFromNowDate);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no reservations for the period", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces
                    .Any(uw => uw.WorkplaceId == workplaceNumber)).Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplaceNumber #{workplaceNumber}", async () =>
            {
                return await httpClient.GetCollisions(userId: _testFactory.EmployeeUserId,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceNumber,
                    dateFrom: date,
                    dateTo: weeksFromNowDate);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no collisions for the period", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplaceNumber,
                    fromDate: date,
                    toDate: date,
                    weeksInInterval: weeksInInterval);
            });

            var afterBookReservations = await scenario.Step($"Get all reservations a {weeksInInterval} weeks from now", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: weeksFromNowDate);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplace #{workplaceNumber} was booked successfully {weeksInInterval} from now", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                var bookedWorkplace = afterBookReservations.SelectMany(r => r.UserWorkplaces)
                    .FirstOrDefault(x => x.WorkplaceId == workplaceNumber && x.UserId == currentUser.Id);
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceNumber);
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });

        const int anotherWorkplaceNumber = 12;

        await scenario.Step("Attempt to create single booking for the same day where repetitive booking exists for other desk", async () =>
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
                toDate: weeksFromNowDate);

            reservations.Should().NotBeNull();
            var bookedWorkplaces = reservations
                .SelectMany(res => res.UserWorkplaces)
                .Where(uw => uw.UserId == _testFactory.EmployeeUserId)
                .ToList();
            bookedWorkplaces.All(uw => uw.WorkplaceId == workplaceNumber)
                .Should().BeTrue();
        });
    }
}
