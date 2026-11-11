using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.Home;

[Collection("GeneralCollection")]
public class HomeIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public HomeIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUpcomingEventsWhenUserHasNoUpcomingAbsenceShouldReturnEmpty()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetUpcomingEvents(userId, 7);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var events = await response.Content.ReadFromJsonAsync<List<UpcomingEventOutput>>(_cancellationToken);
        events.Where(x => x.Type == UpcomingEventType.Absence.ToString()).Should().BeEmpty();
    }

    [Fact]
    public async Task GetUpcomingEventsWhenUserHasUpcomingAbsenceShouldReturnAbsence()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //  create inquiry template
        var templateToCreate = new InquiryTemplateInput
        {
            Name = "Own expense second",
            AcceptPolicies = [
                new UpdateInquiryTemplateAcceptPolicyDto{ OrderWeight = 1, Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO },
                new UpdateInquiryTemplateAcceptPolicyDto{ OrderWeight = 2, Scope = InquiryAcceptPolicyScopes.HeadOfDepartment }
            ],
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = "bar",
            IsIncludeHolidays = false,
            IsIncludeWeekends = false,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        };

        var template = await (await client.CreateTemplateAsHttpResponseAsync(templateToCreate, _jsonOptions))
            .WithResponseType<InquiryTemplateOutput>();

        //  assign user to inquiry
        var assignToCreate = new CreateInquiryAssignInput(AssigneUserIds: [_factory.EmployeeUserId!.Value],
            UnassigneUserIds: [], template.Id, AgreementId: null, EffectiveFrom: today);

        await client.CreateInquiryAssignAsync(assignToCreate, _jsonOptions);

        // update user claims
        var claimsToUpdate = new UserClaimsInput
        {
            ClaimTypes = ["ViewDashboard"]
        };

        await client.PatchAsJsonAsync($"/api/users/{_factory.EmployeeUserId!.Value}/claims", claimsToUpdate);

        //  create request absence
        await client.AuthAsEmployee();

        var inquiryToCreate = new UserInquiryInput
        {
            AgreementTemplateId = null,
            InquiryTemplateId = template.Id,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null
        };

        var inquiry = await client.CreateInquiryAsync(_factory.EmployeeUserId!.Value, inquiryToCreate);

        // Act
        var response = await client.GetUpcomingEvents(_factory.EmployeeUserId!.Value, 7);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var upcomingEvents = await response.Content.ReadFromJsonAsync<List<UpcomingEventOutput>>(
            _jsonOptions, _cancellationToken);

        var upcomingEvent = upcomingEvents.First(x => x.Status == UserInquiryStatus.Open.ToString()
            && x.Name == template.Name
            && x.StartDate == today);
        upcomingEvent.Status.Should().Be(UserInquiryStatus.Open.ToString());
        upcomingEvent.Name.Should().Be(template.Name);
        upcomingEvent.StartDate.Should().Be(today);
        upcomingEvent.EndDate.Should().Be(today);
        upcomingEvent.Type.Should().Be(UpcomingEventType.Absence.GetDescription());
        upcomingEvent.PerformanceReviewDateType.Should().BeNull();
        upcomingEvent.InquiryType.Should().Be(template.Type);
    }
}
