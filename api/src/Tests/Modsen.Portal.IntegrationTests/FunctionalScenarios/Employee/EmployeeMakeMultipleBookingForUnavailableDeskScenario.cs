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

[Collection("FuncScenarioCollection")]
public class EmployeeMakeMultipleBookingForUnavailableDeskScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _helper;

    public EmployeeMakeMultipleBookingForUnavailableDeskScenario(FunctionalTestFactory factory,
        ITestOutputHelper helper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _helper = helper;
    }

    [Fact]
    public async Task EmployeeMakeMultipleBookingWhereMultipleBookingForTheSameDeskExistsScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_helper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee makes multiple booking for period where multiple booking for the same desk exists scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
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

        await scenario.Step("Add multiple booking", async () =>
        {
            const int workplace = 11;
            var fromDate = DateTime.UtcNow.Date;
            var toDate = fromDate.AddDays(2);

            var beforeBookReservations = await scenario.Step("Get all reservations for this period", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
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
                return await httpClient.GetCollisions(userId: _factory.EmployeeUserId,
                    officeId: _factory.MinskOfficeId,
                    workplaceId: workplace,
                    dateFrom: fromDate,
                    dateTo: toDate);
            });

            await scenario.Step($"Check if workplace #{workplace} has no collisions for these days", () =>
            {
                collisions.Should().BeEmpty();
            });

            await httpClient.AuthAs(createdUser!.Email);

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                return await httpClient.CreateSelfBooking(booking);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
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

                return Task.CompletedTask;
            });
        });

        await scenario.Step("Add the second booking", async () =>
        {
            const int workplace = 11;
            var fromDate = DateTime.UtcNow.Date;
            var toDate = fromDate.AddDays(2);

            var beforeBookReservations = await scenario.Step("Get all reservations for this period", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step($"Check if workplace #{workplace} has reservations for these days", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplace))
                    .Should().BeTrue();
            });

            var afterSecondBookingResponse = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step("Verify if booking wasn't created successfully", () =>
            {
                afterSecondBookingResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

                return Task.CompletedTask;
            });
        });
    }
}
