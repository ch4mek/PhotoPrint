using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Booking.Migrator;

[Collection("BookingCollection")]
public class PublishExistingUsersToBookingIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public PublishExistingUsersToBookingIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task MigrateUserToBookingSchemaAfterCreateUserMigratedUser()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ОбновленноеИмя",
            LastNameRu = "ОбновленнаяФамилия",
            PatronymicRu = "ОбновленноеОтчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.Today).AddMonths(-6),
            ContractDateTo = DateOnly.FromDateTime(DateTime.Today).AddMonths(6),
            DateOfBirth = new DateOnly(2000, 2, 2)
        };

        var createdUser = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users", userToCreate, _cancellationToken);

        using var scope = _factory.Services.CreateScope();

        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();

        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();
        var result = dbContext.BookingUsers.FirstOrDefault(user => user.UserId == createdUser!.Id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(createdUser!.Id);
        result.FirstName.Should().Be(createdUser.FirstName);
        result.LastName.Should().Be(createdUser.LastName);
        result.FirstNameRu.Should().Be(createdUser.FirstNameRu);
        result.LastNameRu.Should().Be(createdUser.LastNameRu);
        result.Email.Should().Be(createdUser.Email);
        result.DepartmentId.Should().Be(createdUser.DepartmentId);
    }

    [Fact]
    public async Task MigrateUserToBookingSchemaAfterUpdateUserMigratedUser()
    {
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var userToUpdate = new UpdateUserInput
        {
            Id = _factory.EmployeeUserId!.Value,
            FirstName = null,
            LastName = null,
            FirstNameRu = null,
            LastNameRu = null,
            PatronymicRu = null,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.SystemAdministrator,
            Location = LocationType.Vitebsk,
            DepartmentId = "Administration",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var updatedUser = await client.PutAsJsonAsync<UpdateUserInput, UserOutput>("api/Users", userToUpdate, _cancellationToken);

        using var scope = _factory.Services.CreateScope();

        var migrationTasks = scope.ServiceProvider.GetServices<IMigrationTask>();

        foreach (var migrationTask in migrationTasks)
        {
            migrationTask.Execute();
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();
        var result = dbContext.BookingUsers.FirstOrDefault(user => user.UserId == updatedUser!.Id);

        result.Should().NotBeNull();
        result.UserId.Should().Be(updatedUser!.Id);
        result.FirstName.Should().Be(updatedUser.FirstName);
        result.LastName.Should().Be(updatedUser.LastName);
        result.FirstNameRu.Should().Be(updatedUser.FirstNameRu);
        result.LastNameRu.Should().Be(updatedUser.LastNameRu);
        result.Email.Should().Be(updatedUser.Email);
        result.DepartmentId.Should().Be(updatedUser.DepartmentId);
    }
}
