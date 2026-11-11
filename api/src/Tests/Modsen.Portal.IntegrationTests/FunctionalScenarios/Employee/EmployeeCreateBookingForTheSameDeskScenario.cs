using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateBookingForTheSameDeskScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateBookingForTheSameDeskScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateBookingScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);
        var workplaceId = _testFactory.WorkplaceIdInMinskOffice + 3;
        var date = DateTime.UtcNow.Date;

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee creates single booking for the same desk scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Create booking", async () =>
        {
            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has reservations for the current day", () =>
            {
                reservations!.Should().NotBeNull();
                reservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceNumber == workplaceId))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceId}", async () =>
            {
                return await httpClient.GetCollisions(
                    userId: _testFactory.EmployeeUserId,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceId,
                    dateFrom: date,
                    dateTo: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplaceId, fromDate: date, toDate: date);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(
                    officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Verify if workplace #{workplaceId} was booked successfully", () =>
            {
                reservationsAfterBook!.Should().NotBeNull();
                reservationsAfterBook.Should().ContainSingle();

                var bookedWorkplace = reservationsAfterBook[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceId);

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceId);
                bookedWorkplace.FirstName.Should().Be("Test");
                bookedWorkplace.LastName.Should().Be("Employee");
                bookedWorkplace.IsBlocked.Should().Be(false);
            });
        });

        await scenario.Step("Create booking for the same desk", async () =>
        {
            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(
                    officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has reservations for the current day", () =>
            {
                reservations!.Should().NotBeNull();
                reservations.Any(x => x.UserWorkplaces.Any(f => f.WorkplaceId == workplaceId))
                    .Should().BeTrue();
            });
        });
    }
}
