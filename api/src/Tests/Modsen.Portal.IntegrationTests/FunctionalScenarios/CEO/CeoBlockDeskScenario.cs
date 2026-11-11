using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CeoBlockDeskScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CeoBlockDeskScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task BlockDeskScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CEO]",
            title: "CEO block desk scenario");

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () => await httpClient.AuthAsCeo());

            return await scenario.Step(
                "Call 'Me' endpoint for getting info about current user",
                async () =>
                {
                    return await httpClient.GetCurrentUser();
                });
        });

        await scenario.Step("Block desk", async () =>
        {
            var workplaceId = _testFactory.WorkplaceIdInMinskOffice;
            var date = DateTime.UtcNow.Date.AddMonths(1).AddDays(1);

            var reservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step($"Check if workplace #{workplaceId} has no reservations for the current day",
                () =>
                {
                    reservations!.Should().NotBeNull();
                    reservations.Any(x => x.UserWorkplaces.Any(workplace => workplace.WorkplaceNumber == workplaceId))
                        .Should().BeFalse();
                });

            var collisions = await scenario.Step($"Get all collisions for the workplace #{workplaceId}",
                async () =>
                {
                    return await httpClient.GetCollisions(officeId: _testFactory.MinskOfficeId,
                        workplaceId: workplaceId,
                        dateFrom: date,
                        dateTo: date);
                });

            await scenario.Step($"Check if workplace #{workplaceId} has no collisions for the current day",
                () => collisions.Should().BeEmpty());

            await scenario.Step("Call booking POST endpoint", async () =>
            {
                await httpClient.CreateBlockBooking(workplaceId: workplaceId, fromDate: date, toDate: date);
            });

            var userBookings = await scenario.Step("Get all reservations of current user", async () =>
            {
                return await httpClient.GetBookings(userId: currentUser.Id,
                    includeEmployee: true,
                    officeId: _testFactory.MinskOfficeId);
            });

            var reservationsAfterBook = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _testFactory.MinskOfficeId,
                    fromDate: date,
                    toDate: date);
            });

            await scenario.Step("Clear the block if it has been created", async () =>
            {
                await httpClient.DeleteBookingAsHttpMessage(userBookings!.Bookings![0].BookingId);
            });

            await scenario.Step($"Verify if workplace #{workplaceId} was blocked successfully", () =>
            {
                reservationsAfterBook!.Should().NotBeNull();
                reservationsAfterBook.Should().ContainSingle();

                var bookedWorkplace = reservationsAfterBook[0].UserWorkplaces.FirstOrDefault(x => x.WorkplaceId == workplaceId);

                bookedWorkplace.Should().NotBeNull();
                bookedWorkplace.WorkplaceId.Should().Be(workplaceId);
                bookedWorkplace.FirstName.Should().Be("Test");
                bookedWorkplace.LastName.Should().Be("CEO");
                bookedWorkplace.IsBlocked.Should().Be(true);
            });
        });
    }

}
