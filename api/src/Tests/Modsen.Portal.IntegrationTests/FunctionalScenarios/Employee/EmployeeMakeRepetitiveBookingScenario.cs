using System.Text.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.Booking.Contracts;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeMakeRepetitiveBookingScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _helper;

    public EmployeeMakeRepetitiveBookingScenario(FunctionalTestFactory factory,
        ITestOutputHelper helper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _helper = helper;
    }

    [Fact]
    public async Task EmployeeMakeRepetitiveBookingForSingleDeskScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_helper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee makes repetitive booking for an available desk scenario");

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
                return await httpClient.GetBookings(officeId: _factory.MinskOfficeId,
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

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
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

        await scenario.Step("Add new booking", async () =>
        {
            const int workplace = 10;
            var date = DateTime.UtcNow.Date.AddMonths(2);
            const int weeksInInterval = 1;

            await httpClient.AuthAs(createdUser.Email);

            var beforeBookReservations = await scenario.Step("Get all reservations for this period", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplace} has no reservations for these days", () =>
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

            await scenario.Step($"Check if workplace #{workplace} has no collisions for these days", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = date,
                    ToDate = date,
                    WeeksInInterval = weeksInInterval
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            var weeksFromNowDate = date.AddDays(weeksInInterval * 7).Date;

            var afterBookReservations = await scenario.Step($"Get all reservations a {weeksInInterval} weeks from now", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
                    fromDate: DateTime.UtcNow.Date,
                    toDate: weeksFromNowDate);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplace #{workplace} was booked successfully {weeksInInterval} from now", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                var bookedWorkplace = afterBookReservations.SelectMany(r => r.UserWorkplaces)
                        .FirstOrDefault(x => x.WorkplaceId == workplace && x.UserId == currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.IsBlocked.Should().Be(false);

                return Task.CompletedTask;
            });
        });
    }
}
