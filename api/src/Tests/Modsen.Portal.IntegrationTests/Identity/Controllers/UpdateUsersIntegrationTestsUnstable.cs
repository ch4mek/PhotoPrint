using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Identity.Controllers;

[Collection("UnstableIdentityCollection")]
public class UpdateUsersUnstableIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateUsersUnstableIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserAsAdministratorWhenRequestIsValidReturnUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(firstNameRu: "ОбновленноеИмя",
            lastNameRu: "ОбновленнаяФамилия");

        var userToUpdate = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = "ОбновленноеИмя",
            LastNameRu = "ОбновленнаяФамилия",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#fdfdfd",
            Position = employee.Position,
            Location = employee.Location,
            DepartmentId = employee.DepartmentId,
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        var expected = new UpdateUserInput
        {
            Id = employee.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = userToUpdate.FirstNameRu,
            LastNameRu = userToUpdate.LastNameRu,
            Email = userToUpdate.Email,
            Color = "#fdfdfd",
            Position = employee.Position,
            Location = employee.Location,
            DepartmentId = employee.DepartmentId,
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        await client.AuthAsAdmin();

        // Act
        var updatedUser = await client.UpdateUserAsync(userToUpdate);

        // Assert
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.HireDate)
                .Excluding(x => x.PatronymicRu));
    }

    [Fact]
    public async Task UpdateUserWhenAuthedAsAdminAndDontChangeEmailShouldReturnUpdatedUserWithSameEmail()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUser = await client.CreateEmployee(firstNameRu: "ОбновленноеИмя",
            lastNameRu: "ОбновленнаяФамилия");

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = "ОбновленноеИмя",
            LastNameRu = "ОбновленнаяФамилия",
            Email = createdUser.Email,
            Color = "#fdfdfd",
            Position = createdUser.Position!,
            Location = createdUser.Location,
            DepartmentId = createdUser.DepartmentId,
            CountryId = 1,
            RegisteredLocationId = createdUser.RegisteredLocationId
        };

        var expected = new UpdateUserInput
        {
            Id = createdUser.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = userToUpdate.FirstNameRu,
            LastNameRu = userToUpdate.LastNameRu,
            Email = createdUser.Email,
            Color = "#fdfdfd",
            Position = createdUser.Position!,
            Location = createdUser.Location,
            DepartmentId = createdUser.DepartmentId,
            CountryId = 1,
            RegisteredLocationId = createdUser.RegisteredLocationId
        };

        await client.AuthAsAdmin();

        // Act
        var updatedUser = await client.UpdateUserAsync(userToUpdate);

        // Assert
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.HireDate)
                .Excluding(x => x.PatronymicRu));
    }

    [Fact]
    public async Task UpdateUserAsAdministratorWhenUserIsNotEmployeeReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createdUser = await client.CreateEmployee(firstNameRu: "ОбновленноеИмяТестОдин",
            lastNameRu: "ОбновленнаяФамилия",
            patronymicRu: "ОбновленноеОтчество");

        var userToUpdate = new UpdateUserInput
        {
            Id = createdUser!.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = "ОбновленноеИмяТестОдин",
            LastNameRu = "ОбновленнаяФамилия",
            PatronymicRu = "ОбновленноеОтчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#fdfdfd",
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Js",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        await client.AuthAsAdmin();

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserWhenAuthedAsSystemAdministratorAndTryToUpdateUserWithExistingEmailShouldReturnConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com";
        await client.CreateEmployee(email: email);

        var createdUserToBeUpdated = await client.CreateEmployee();

        var updateUserInput = new UpdateUserInput
        {
            Id = createdUserToBeUpdated!.Id,
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            FirstNameRu = "ОбновленноеИмя",
            LastNameRu = "ОбновленнаяФамилия",
            PatronymicRu = "ОбновленноеОтчество",
            Email = email,
            Color = "#fdfdfd",
            Position = createdUserToBeUpdated.Position!,
            Location = createdUserToBeUpdated.Location!,
            DepartmentId = createdUserToBeUpdated.DepartmentId!,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        await client.AuthAsAdmin();

        // Act
        var updateResponse = await client.UpdateUserAsHttpMessageAsync(updateUserInput, _jsonOptions);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

}
