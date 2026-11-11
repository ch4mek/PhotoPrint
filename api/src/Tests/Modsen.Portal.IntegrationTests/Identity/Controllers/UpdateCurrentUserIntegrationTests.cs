using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("GeneralCollection")]
public class UpdateCurrentUserIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public UpdateCurrentUserIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenHeadTryToChangeDepartmentShouldReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.UpdateCurrentUserAsHttpMessageAsync(departmentId: "Java");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenAuthAsEmployeeShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var updateCurrentUserInput = new UpdateCurrentUserInput
        {
            Location = LocationType.Minsk,
            TimeZone = "BY",
            DepartmentId = "Net",
        };

        var expectedModel = new CurrentUserOutput
        {
            FirstName = "Test",
            LastName = "Employee",
            Location = updateCurrentUserInput.Location,
            TimeZone = updateCurrentUserInput.TimeZone,
            DepartmentId = updateCurrentUserInput.DepartmentId,
            AvatarColor = updateCurrentUserInput.AvatarColor,
            Email = EmailConstants.EmployeeEmail,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var updatedUser = await client.UpdateCurrentUserAsync(
            departmentId: "Net",
            location: LocationType.Minsk);

        // Assert
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(expectedModel, config =>
        {
            // Exclude random values
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Email);
            config.Excluding(x => x.Position);

            return config;
        });
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenEmployeeTryToUpdateHisDepartmentShouldReturnForbiddenResponse()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateCurrentUserAsHttpMessageAsync(departmentId: "Java");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenAuthedAsCTOAndUpdateDepartmentToNullShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var expectedModel = new CurrentUserOutput
        {
            FirstName = "Test",
            LastName = "CTO",
            Location = LocationType.Minsk,
            TimeZone = "BY",
            DepartmentId = null,
            AvatarColor = null,
            Email = EmailConstants.CtoEmail,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var userOutput = await client.UpdateCurrentUserAsync(departmentId: null);

        // Assert
        userOutput.Should().NotBeNull();
        userOutput.Should().BeEquivalentTo(expectedModel, config =>
        {
            // Exclude random values
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Email);
            config.Excluding(x => x.Position);

            return config;
        });
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenAuthedAsCEOAndUpdateDepartmentToNullShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var expectedModel = new CurrentUserOutput
        {
            FirstName = "Test",
            LastName = "CTO",
            Location = LocationType.Minsk,
            TimeZone = "BY",
            DepartmentId = null,
            AvatarColor = null,
            Email = EmailConstants.CtoEmail,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var userOutput = await client.UpdateCurrentUserAsync(departmentId: null);

        // Assert
        userOutput.Should().NotBeNull();
        userOutput.Should().BeEquivalentTo(expectedModel, config =>
        {
            // Exclude random values
            config.Excluding(x => x.Color);
            config.Excluding(x => x.Id);
            config.Excluding(x => x.AvatarFileId);
            config.Excluding(x => x.Email);
            config.Excluding(x => x.Position);

            return config;
        });
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task UpdateCurrentUserAsyncWhenAuthedAsRoleWithoutDepartmentAndTryToSetDepartmentShouldReturnBadRequest(string roleType)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsRole(roleType);

        // Act
        var response = await client.UpdateCurrentUserAsHttpMessageAsync(departmentId: "Net");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.GroupManager)]
    public async Task UpdateCurrentUserAsyncWhenAuthedAsRoleWithDepartmentAndTryToSetDepartmentToNullShouldReturnBadRequest(
        string roleType)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsRole(roleType);

        // Act
        var response = await client.UpdateCurrentUserAsHttpMessageAsync(departmentId: null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCurrentUserAsyncWhenSpecifiedAvatarFileDoesNotExistShouldReturnNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.UpdateCurrentUserAsHttpMessageAsync(
            departmentId: null,
            avatarFileId: Guid.CreateVersion7().ToAlphabetic());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}
