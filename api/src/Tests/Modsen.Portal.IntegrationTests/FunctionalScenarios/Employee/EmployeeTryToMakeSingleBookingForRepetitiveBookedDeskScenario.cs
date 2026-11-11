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
public class EmployeeTryToMakeSingleBookingForRepetitiveBookedDeskScenario
{
    private readonly FunctionalTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public EmployeeTryToMakeSingleBookingForRepetitiveBookedDeskScenario(FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task TryToMakeSingleBookingForRepetitiveBookedDeskScenario()
    {
        using var httpClient = _testFactory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee – Verification of unavailability of a single booking creating for period where repetitive booking for the same desk exists scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        const int workplace = 13;
        var date = DateTime.UtcNow.Date.AddDays(60);
        var formattedDate = date.ToString("yyyy'-'MM'-'dd'Z'");
        const int weeksInterval = 1;

        await scenario.Step($"Add new repetitive booking for workplace #{workplace} {formattedDate}, interval={weeksInterval} ", async () =>
        {
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
                return await httpClient.GetCollisions(userId: _testFactory.EmployeeUserId,
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
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = date,
                    ToDate = date,
                    WeeksInInterval = weeksInterval
                };

                return await httpClient.CreateSelfBooking(booking);
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

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDate}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = date,
                    ToDate = date
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDate} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        var dateSecondWeek = date.AddDays(7);
        var formattedDateSecondWeek = dateSecondWeek.ToString("yyyy'-'MM'-'dd'Z'");

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDateSecondWeek}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateSecondWeek,
                    ToDate = dateSecondWeek
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDateSecondWeek} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });

        var dateThirdWeek = date.AddDays(7);
        var formattedDateThirdWeek = dateThirdWeek.ToString("yyyy'-'MM'-'dd'Z'");

        await scenario.Step($"Add new single booking for workplace #{workplace} {formattedDateThirdWeek}", async () =>
        {
            var response = await scenario.Step("Call booking POST endpoint", async () =>
            {
                var booking = new CreateSelfBookingTestInput
                {
                    RequestType = CreateBookingBaseInput.CreateSelf,
                    WorkplaceId = workplace,
                    FromDate = dateThirdWeek,
                    ToDate = dateThirdWeek
                };

                return await httpClient.CreateSelfBookingAsHttpMessage(booking);
            });

            await scenario.Step($"Verify if booking for workplace #{workplace} {formattedDateThirdWeek} was failed", () =>
            {
                response.IsSuccessStatusCode.Should().BeFalse();
            });
        });
    }
}
