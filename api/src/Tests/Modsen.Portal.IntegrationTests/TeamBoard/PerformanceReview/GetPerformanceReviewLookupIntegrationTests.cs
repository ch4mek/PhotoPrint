using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
namespace Modsen.Portal.IntegrationTests.TeamBoard.PerformanceReview;

[Collection("GeneralCollection")]
public class GetPerformanceReviewLookupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetPerformanceReviewLookupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task GetPerformanceReviewLookupWhenAuthedUserDoesNotHaveViewDashboardClaimShouldForbidden(
        string position)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = position,
            DepartmentId = "Recruitment",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        await client.AuthAs(user!.Email);

        // Act
        var response = await client.GetPerformanceReviewLookupAsHttpResponseMessageAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task GetPerformanceReviewLookupWhenAuthedUserHasViewDashboardClaimShouldOk(
        string position)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var user = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = position,
            DepartmentId = "SAP",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = ["ViewDashboard"], };
        await client.PatchAsJsonAsync($"/api/users/{user!.Id}/claims", claimsToUpdate);

        await client.AuthAs(user!.Email);

        // Act
        var response = await client.GetPerformanceReviewLookupAsHttpResponseMessageAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
