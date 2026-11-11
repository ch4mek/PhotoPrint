using System.Text.Json;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class EmployeeCreateMultipleBookingForSingleBookedDeskByOtherUserScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeCreateMultipleBookingForSingleBookedDeskByOtherUserScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task CreateMultipleBookingForSingleBookedDeskByOtherUserScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee cannot create multiple booking for the same desk and period if another single booking already exists for other user scenario");

        await scenario.Step("Login as CEO", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
            });
        });

        var firstCreatedUser = await CreateUserAsync(scenario, httpClient);
        var secondCreatedUser = await CreateUserAsync(scenario, httpClient);

        await scenario.Step("Login as first user", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(firstCreatedUser.Email);
            });
        });

        var date = DateTime.UtcNow.Date.AddDays(1);
        const int workplaceNumber = 14;

        await scenario.Step("Create a single booking for first user", async () =>
        {
            var beforeBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no reservations for the current day", () =>
            {
                beforeBookReservations!.Should().NotBeNull();
                beforeBookReservations.Any(x => x.UserWorkplaces.Any(uw => uw.WorkplaceId == workplaceNumber))
                    .Should().BeFalse();
            });

            var collisions = await scenario.Step($"Get all collisions for the workplaceNumber #{workplaceNumber}", async () =>
            {
                return await httpClient.GetCollisions(userId: firstCreatedUser.Id,
                    officeId: _testFactory.MinskOfficeId,
                    workplaceId: workplaceNumber,
                    dateTo: date,
                    dateFrom: date);
            });

            await scenario.Step($"Check if workplaceNumber #{workplaceNumber} has no collisions for the current day", () =>
            {
                collisions.Should().BeEmpty();
            });

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBooking(workplaceId: workplaceNumber, toDate: date, fromDate: date);
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

        await scenario.Step("Login as second user", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAs(secondCreatedUser.Email);
            });
        });

        var fromDate = DateTime.UtcNow.Date;
        var toDate = DateTime.UtcNow.AddDays(2).Date;

        await scenario.Step("Attempt to create multiple booking for second user where single booking exists", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(workplaceId: workplaceNumber, toDate: toDate,
                    fromDate: fromDate);
            });

            await scenario.Step($"Verify if booking for workplace #{workplaceNumber} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        await scenario.Step("Verify that the first user's booking still exists", async () =>
        {
            var reservations = await httpClient.GetReservation(
                officeId: _testFactory.MinskOfficeId,
                fromDate: date,
                toDate: date);

            reservations.Should().NotBeNull();
            reservations.Should().ContainSingle(res => res.UserWorkplaces.Any(x => x.WorkplaceId == workplaceNumber));
            reservations.Should().ContainSingle(res => res.UserWorkplaces.Any(x => x.UserId == firstCreatedUser.Id));
        });
    }

    private Task<UserOutput> CreateUserAsync(ITestScenario scenario, HttpClient httpClient)
    {
        return scenario.Step("Create employee", async () =>
        {

            var createdUser = await scenario.Step("Call 'Users' endpoint to create new user", async () =>
            {
                return await httpClient.CreateEmployee();
            });

            await scenario.Step("Verify employee creation", () =>
            {
                createdUser.Should().NotBeNull();
            });

            return createdUser!;
        });
    }
}
