using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateMultipleBookingWithSingleBookingOnTheSameDeskExisting
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateMultipleBookingWithSingleBookingOnTheSameDeskExisting(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateMultipleBookingScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);
        const int workplaceNumber = 14;

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee creates multiple booking for a period where single booking of the same desk for the same User exists scenario");

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

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Create booking", async () =>
        {
            var date = DateTime.UtcNow.Date;

            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceNumber} has no reservations for the current day", () =>
            {
                reservations!.Should().NotBeNull();
                reservations.Any(x => x.UserWorkplaces
                        .Any(uw => uw.WorkplaceNumber == workplaceNumber))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceNumber}", async () =>
            {
                return await httpClient.GetCollisions(userId: _testFactory. EmployeeUserId,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceNumber,
                    dateFrom: date,
                    dateTo: date);
            });

            await scenario.Step($"Check if workplace #{workplaceNumber} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplaceNumber,
                    fromDate: date,
                    toDate: date,
                    weeksInInterval: 2);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Verify if workplace #{workplaceNumber} was booked successfully", () =>
            {
                reservationsAfterBook!.Should().NotBeNull();
                reservationsAfterBook.Should().ContainSingle();

                var bookedWorkplace = reservationsAfterBook[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceNumber);

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceNumber);
                bookedWorkplace.FirstName.Should().Be("Test");
                bookedWorkplace.LastName.Should().Be("Employee");
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });

        await scenario.Step("Create multiple booking", async () =>
        {
            var fromDate = DateTime.UtcNow.AddDays(-1).Date;
            var toDate = DateTime.UtcNow.AddDays(1).Date;

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(workplaceId: workplaceNumber,
                    fromDate: fromDate,
                    toDate: toDate,
                    weeksInInterval: 2);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the period", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step($"Verify if workplace #{workplaceNumber} was booked successfully", () =>
            {
                reservationsAfterBook!.Should().NotBeNull();

                var bookedWorkplace = reservationsAfterBook[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceNumber);

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceNumber);
                bookedWorkplace.FirstName.Should().Be("Test");
                bookedWorkplace.LastName.Should().Be("Employee");
                bookedWorkplace.IsBlocked.Should().Be(false);
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
    }
}
