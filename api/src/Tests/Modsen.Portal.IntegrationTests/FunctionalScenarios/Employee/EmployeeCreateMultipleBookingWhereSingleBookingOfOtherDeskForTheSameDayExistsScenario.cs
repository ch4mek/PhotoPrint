using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateMultipleBookingWhereSingleBookingOfOtherDeskForTheSameDayExistsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateMultipleBookingWhereSingleBookingOfOtherDeskForTheSameDayExistsScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task EmployeeCreateMultipleBookingWhereSingleBookingOfOtherDeskForTheSameDayAlreadyExistsScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee makes multiple booking for period where single booking for the same desk and the same day exists scenario");

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

        await scenario.Step("Add new booking", async () =>
        {
            var workplace = _testFactory.WorkplaceIdInMinskOffice + 15;
            var date = DateTime.UtcNow.Date.AddMonths(2).AddDays(10);

            await httpClient.AuthAs(createdUser!.Email);

            var beforeBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
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
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplace,
                    dateFrom: date,
                    dateTo: date);
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
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
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

        await scenario.Step("Add multiple booking", async () =>
        {
            await httpClient.AuthAs(createdUser!.Email);
            var workplace = _testFactory.WorkplaceIdInMinskOffice;
            var fromDate = DateTime.UtcNow.Date.AddMonths(2).AddDays(10);
            var toDate = DateTime.UtcNow.Date.AddDays(1);

            await httpClient.AuthAs(createdUser.Email);

            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(workplaceId: workplace,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });
    }
}
