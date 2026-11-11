using System.Text.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.Booking.Contracts;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

public class EmployeeMakeSingleBookingAvailableDeskScenario : IClassFixture<FunctionalTestFactory>
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _helper;

    public EmployeeMakeSingleBookingAvailableDeskScenario(FunctionalTestFactory factory,
        ITestOutputHelper helper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _helper = helper;
    }

    [Fact]
    public async Task EmployeeMakeSingleBookingScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_helper);

        var scenario = TestScenarioFactory.Default(outputAdapter,
            scenarioPrefix: "[Employee]",
            title: "Employee makes single booking for an available desk scenario");

        await scenario.Step("Login", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsEmployee();
            });
        });

        await scenario.Step("Add new booking", async () =>
        {
            const int workplace = 13;
            var date = DateTime.UtcNow.Date;

            var beforeBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
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
                return await httpClient.GetCollisions(userId: _factory.EmployeeUserId,
                    officeId: _factory.MinskOfficeId,
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
                    ToDate = date
                };

                return await httpClient.CreateSelfBooking(booking);
            });

            var afterBookReservations = await scenario.Step("Get all reservations for the current day", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
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
    }
}
