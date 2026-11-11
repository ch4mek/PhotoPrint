using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using NScenario;
using Xunit.Abstractions;

namespace Modsen.Portal.IntegrationTests.FunctionalScenarios;

[Collection("FuncScenarioCollection")]
public class HodRepetitiveSelfBookingScenario
{
    private readonly FunctionalTestFactory _factory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HodRepetitiveSelfBookingScenario(
        FunctionalTestFactory testFactory,
        ITestOutputHelper outputHelper)
    {
        _factory = testFactory;
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task RepetitiveSelfBookingScenario()
    {
        int reservationsMonthCount;
        using (var scope = _factory.Services.CreateScope())
        {
            var portalSettingsAccessor = scope.ServiceProvider.GetRequiredService<IPortalSettingsAccessor>();
            var settings = await portalSettingsAccessor.GetByNames(
                [BookingSettingNames.ReservationsMonthCount], _cancellationToken);
            reservationsMonthCount = settings.FirstOrDefault().GetInt();
        }

        using var httpClient = _factory.CreateClient();
        await using var outputAdapter = new XUnitOutputAdapter(_outputHelper);

        var scenario = TestScenarioFactory.Default(
            outputAdapter,
            scenarioPrefix: "[Head of Department]",
            title: "Head of Department Create repetitive self booking scenario");

        const int weeksInInterval = 1;
        var bookingDate = DateTime.UtcNow.Date;
        var lastDateOfRepetitiveBooking = bookingDate.AddMonths(reservationsMonthCount);

        await scenario.Step("Login as CEO ", async () =>
        {
            await scenario.Step("Call auth endpoint", async () =>
            {
                await httpClient.AuthAsCeo();
            });
        });

        await scenario.Step("Delete user's reservations", async () =>
        {
            var userReservations = await scenario.Step("Get all reservations of current user", async () =>
            {
                return await httpClient.GetBookings(officeId: _factory.MinskOfficeId, includeEmployee: true);
            });

            if (userReservations.Bookings != null)
            {
                foreach (var booking in userReservations.Bookings)
                {
                    await httpClient.DeleteBookingAsHttpMessage(booking.BookingId);
                }
            }
        });

        var currentUser = await scenario.Step("Login", async () =>
        {
            await scenario.Step(
                "Call auth endpoint",
                () => httpClient.AuthAsHeadOfDepartment());

            var user = await scenario.Step("Call 'Me' endpoint for getting info about current user", async () =>
            {
                return await httpClient.GetCurrentUser();
            });

            await scenario.Step("Verify info about user", () =>
            {
                user.Should().NotBeNull();
                user.DepartmentId.Should().NotBeNull();
                user.Email.Should().NotBeNullOrEmpty();
                user.Position.Should().Be(PortalRoleNames.HeadOfDepartment);
            });

            return user;
        });

        var bookedWorkplaceId = await scenario.Step("Book free workplace", async () =>
        {
            var workplaceId = _factory.WorkplaceIdInWarsawOffice;
            var response = await scenario.Step($"[M-{workplaceId}] Book workplace", async () =>
            {
                return await httpClient.CreateSelfBookingAsHttpMessage(
                    workplaceId: workplaceId,
                    weeksInInterval: weeksInInterval,
                    fromDate: bookingDate,
                    toDate: bookingDate);
            });

            await scenario.Step(
                $"[M-{workplaceId}] Validate response status code",
                () => response.StatusCode.Should().Be(HttpStatusCode.OK));

            return workplaceId;
        });

        await scenario.Step("Check booked workplace by current reservations", async () =>
        {
            var reservations = await scenario.Step("Get all reservations", async () =>
            {
                return await httpClient.GetReservation(officeId: _factory.WarsawOfficeId,
                    fromDate: bookingDate,
                    toDate: lastDateOfRepetitiveBooking);
            });

            var expectedReservedDays = 0;
            for (var start = bookingDate; start <= lastDateOfRepetitiveBooking; start = start.AddDays(7))
            {
                expectedReservedDays++;
            }

            await scenario.Step("Validate response body", () =>
            {
                reservations.Should().NotBeNull();
                reservations.FindAll(x => x.UserWorkplaces.Any(workplace =>
                        workplace.UserId == currentUser.Id &&
                        workplace.WorkplaceId == bookedWorkplaceId &&
                        workplace.FirstName == currentUser.FirstName &&
                        workplace.LastName == currentUser.LastName))
                    .Should().HaveCount(expectedReservedDays);
            });
        });
    }
}
