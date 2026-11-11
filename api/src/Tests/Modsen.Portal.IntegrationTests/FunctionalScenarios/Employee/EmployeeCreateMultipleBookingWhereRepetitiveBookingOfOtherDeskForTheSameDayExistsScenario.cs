using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("UnstableFuncScenarioCollection")]
public class EmployeeCreateMultipleBookingWhereRepetitiveBookingOfOtherDeskForTheSameDayExistsScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateMultipleBookingWhereRepetitiveBookingOfOtherDeskForTheSameDayExistsScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact(Skip = "skip")]
    public async Task EmployeeCreateMultipleBookingWhereRepetitiveBookingOfOtherDeskForTheSameDayAlreadyExistsScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee Verification of unavailability of the multiple booking creating for the same day where repetitive booking of other desk for the same User exists scenario");

        await scenario.Step("Login", () =>
        {
            return Task.FromResult(scenario.Step("Call auth endpoint", () => httpClient.AuthAsEmployee()));
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

        await scenario.Step("Add new repetitive booking", async () =>
        {
            var workplace = _testFactory.WorkplaceIdInMinskOffice + 11;
            var date = DateTime.UtcNow.Date.AddMonths(1).AddDays(12);
            const int weeksInInterval = 1;

            await httpClient.AuthAs(createdUser!.Email);

            var beforeBookReservations = await scenario.Step("Get all reservations for this period", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory. MinskOfficeId,
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
                    officeId: _testFactory. MinskOfficeId,
                    workplaceId: workplace,
                    dateFrom: date,
                    dateTo: date);
            });

            await scenario.Step($"Check if workplace #{workplace} has no collisions for these days", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplace,
                    fromDate: date,
                    toDate: date,
                    weeksInInterval: weeksInInterval);
            });

            var weeksFromNowDate = date.AddDays(weeksInInterval * 7).Date;

            var afterBookReservations = await scenario.Step($"Get all reservations a {weeksInInterval} weeks from now", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
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
                bookedWorkplace!.UserId.Should().Be(currentUser.Id);
                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplace);
                bookedWorkplace.IsBlocked.Should().Be(false);

                return Task.CompletedTask;
            });
        });

        await scenario.Step("Add multiple booking", async () =>
        {
            var workplace = _testFactory.WorkplaceIdInMinskOffice + 12;
            var fromDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(12);
            var toDate = DateTime.UtcNow.Date.AddMonths(1).AddDays(14);

            await httpClient.AuthAs(createdUser!.Email);

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
