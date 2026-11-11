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
public class EmployeeTryToMakeSingleBookingForMultipleBookedDeskScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeTryToMakeSingleBookingForMultipleBookedDeskScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TryToMakeSingleBookingForMultipleBookedDeskScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee – Verification of unavailability of a single booking creating for period where multiple booking for the same desk exists scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
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

        const int workplace = 15;
        var dateFrom = DateTime.UtcNow.Date;
        var dateTo = DateTime.UtcNow.Date.AddDays(7);
        var formattedDateFrom = dateFrom.ToString("yyyy'-'MM'-'dd'Z'");
        var formattedDateTo = dateTo.ToString("yyyy'-'MM'-'dd'Z'");

        await scenario.Step($"Add new multiple booking for workplace #{workplace} {formattedDateFrom} - {formattedDateTo} ", async () =>
        {
            var beforeBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: dateFrom,
                    toDate: dateTo);
            });

            await scenario.Step($"Check if workplace #{workplace} has no reservations for the current day", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplace))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplace}", async () =>
            {
                return await httpClient.GetCollisions(userId: _testFactory.EmployeeUserId,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplace,
                    dateFrom: dateFrom,
                    dateTo: dateTo);
            });

            await scenario.Step($"Check if workplace #{workplace} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateFrom,
                    ToDate = dateTo
                };

                return await httpClient.CreateSelfBooking(booking);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: dateFrom,
                    toDate: dateTo);
            });

            var currentUser = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step($"Verify if workplace #{workplace} was booked successfully", () =>
            {
                afterBookReservations!.Should().NotBeNull();
                afterBookReservations.Should().HaveCount((dateTo - dateFrom).Days + 1);
                var bookedWorkplace = afterBookReservations[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplace);
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplace);
                bookedWorkplace.IsBlocked.Should().Be(false);

                return Task.CompletedTask;
            });
        });

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDateFrom}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateFrom,
                    ToDate = dateFrom
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDateFrom} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        var dateInRange = dateFrom.AddDays(2);
        var formattedDateInRange = dateInRange.ToString("yyyy'-'MM'-'dd'Z'");

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDateInRange}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateInRange,
                    ToDate = dateInRange
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDateInRange} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDateTo}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateTo,
                    ToDate = dateTo
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDateTo} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
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
