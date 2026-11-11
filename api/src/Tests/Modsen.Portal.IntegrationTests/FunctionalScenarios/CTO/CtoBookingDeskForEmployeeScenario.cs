using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;
namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class CtoBookingDeskForEmployeeScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ITestOutputHelper _outputHelper;

    public CtoBookingDeskForEmployeeScenario(
        FunctionalTestFactory factory,
        ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task BookingDeskForEmployeeScenario()
    {
        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[CTO]",
            title: "Booking desk for employee scenario");

        const int reservationDays = 8;
        var fromDate = DateTime.UtcNow.Date;
        var toDate = fromDate.AddDays(reservationDays - 1);

        await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsCto());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().BeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.Cto);
            });
        });

        var createdUser = await scenario.Step("Create new employee", async () =>
        {
            await httpClient.AuthAsCto();

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

        var bookedWorkplaceId = await scenario.Step("Book free workplace", async () =>
        {
            var response = await scenario.Step($"[M-{_factory.WorkplaceIdInMinskOffice}] Book workplace", async () =>
            {
                return await httpClient.CreateUserBookingAsHttpMessage(
                    workplaceId: _factory.WorkplaceIdInMinskOffice,
                    fromDate: fromDate,
                    toDate: toDate,
                    userId: createdUser!.Id);
            });

            await scenario.Step(
                $"[M-{_factory.WorkplaceIdInMinskOffice}] Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return _factory.WorkplaceIdInMinskOffice;
        });

        await scenario.Step("Check booked workplace by current reservations", async () =>
        {
            var reservations = await scenario.Step("Get all reservations", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.MinskOfficeId,
                    fromDate: fromDate,
                    toDate: toDate);
            });

            await scenario.Step("Validate response body", () =>
            {
                reservations.Should().NotBeNull();
                reservations.FindAll(x => x.UserWorkplaces.Any(workplace =>
                        workplace.UserId == createdUser!.Id &&
                        workplace.WorkplaceId == bookedWorkplaceId &&
                        workplace.FirstName == createdUser.FirstName &&
                        workplace.LastName == createdUser.LastName))
                    .Should().HaveCount(reservationDays);
            });
        });
    }
}
