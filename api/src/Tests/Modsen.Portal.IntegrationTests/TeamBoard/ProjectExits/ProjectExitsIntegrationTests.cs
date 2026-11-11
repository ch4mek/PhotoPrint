using System.Net;
using System.Net.Http.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

namespace Modsen.Portal.IntegrationTests.TeamBoard.ProjectExits;

[Collection("TeamBoardCollection")]
public class ProjectExitsControllerIntegrationTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IntegrationTestFactory _factory;

    public ProjectExitsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("top=20&skip=0")]
    [InlineData("top=10&skip=50")]
    [InlineData("top=50&skip=50&orderby=Id")]
    public async Task GetPageableProjectExits_WhenRequestIsValid_ResponseOk(string query)
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
            Position = PortalRoleNames.GroupManager,
            DepartmentId = "SAP",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = [StaffingClaimTypeNames.ManageAllProjectExits.Name], };
        await client.PatchAsJsonAsync($"/api/users/{user!.Id}/claims", claimsToUpdate, _cancellationToken);

        await client.AuthAs(user!.Email);

        // Act
        var response = await client.GetPageableProjectExitsAsHttpResponseAsync(query, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task GetProjectExitsWithODataQuery_WhenQueryIsValid_ReturnsExpectedItems(string position)
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

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = [StaffingClaimTypeNames.ManageAllProjectExits.Name], };
        await client.PatchAsJsonAsync($"/api/users/{user!.Id}/claims", claimsToUpdate);

        await client.AuthAs(user!.Email);

        var query = "orderby=userId%20asc&$top=50&$skip=0";

        // Act
        var response = await client.GetPageableProjectExitsAsHttpResponseAsync(query, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetEmptyList_WhenRequestIsMadeByEmployee_ResponseOk()
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
            Position = PortalRoleNames.Employee,
            DepartmentId = "SAP",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var claimsToUpdate = new UserClaimsInput { ClaimTypes = [StaffingClaimTypeNames.ViewProjectExits.Name], };

        await client.PatchAsJsonAsync($"/api/users/{user!.Id}/claims", claimsToUpdate, _cancellationToken);

        await client.AuthAs(user!.Email);

        var query = "orderby=userId%20asc&$top=50&$skip=0";

        // Act
        var response = await client.GetPageableProjectExitsAsHttpResponseAsync(query, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
