using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.Identity.Controllers.UserController;

[Collection("IdentityCollection")]
public class UpdateUserStatusIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private int CeoId => _factory.CeoUserId!.Value;
    private int CtoId => _factory.CtoUserId!.Value;
    private int EmployeeId => _factory.EmployeeUserId!.Value;
    private int AdminId => _factory.AdminUserId!.Value;

    public UpdateUserStatusIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateUserStatusWhenUserCanBeDeactivatedShouldResponseWithUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(AdminId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserStatusWhenUserCanBeActivatedShouldResponseWithUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.SetUserActiveAsHttpMessage(EmployeeId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUserStatusWhenUserCannotBeActivatedShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.SetUserActiveAsHttpMessage(CtoId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenAuthAsEmployeeShouldResponseWithForbiddenResult()
    {
        // Arrange
        const int userId = 1;
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(userId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateEmployeeStatusWhenAuthAsSystemAdministratorShouldResponseUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        await client.AuthAsAdmin();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(employee.Id);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "todo: fix")]
    public async Task UpdateEmployeeStatusToActiveWhenAuthAsSystemAdministratorShouldResponseUpdatedUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee(departmentId: "Java");
        var employeeId = employee!.Id;

        await client.SetUserInactive(employeeId);
        await client.AuthAsAdmin();

        // Act
        var response = await client.SetUserActiveAsHttpMessage(employeeId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCEOStatusWhenAuthAsSystemAdministratorShouldResponseForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(CeoId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenCeoTriesDeactivateHimselfShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(CeoId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenUserDoesNotExistShouldResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int userId = int.MaxValue;

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(userId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUserStatusWhenDeactivatingUserBeingAssignedToProjectShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();
        var employeeId = employee!.Id;

        await client.CreateProjectAssignment(userId: employeeId, projectId: _factory.IdleProjectId!.Value);

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(employeeId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenDeactivatingUserWithAssignedEquipmentShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var equipmentToCreate = new CreateEquipmentRequestDto
        {
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            SerialNumber = Guid.CreateVersion7().ToString(),
            BrokenStatus = EquipmentBrokenStatus.None,
            ApproverId = CeoId,
            InvoiceNumber = "DSAMVAS123ASKD123",
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            Comment = "=== TEST DATA ===",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePrice = 134,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        var equipment = await client.CreateEquipmentAsync(equipmentToCreate);

        var assignEquipmentInput = new AssignEquipmentInput
        {
            UserId = EmployeeId,
            IssueDate = DateTime.UtcNow.Date,
            ApproverId = CeoId
        };

        await client.CreateEquipmentHistoryAsync(equipment!.Id, assignEquipmentInput);

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(EmployeeId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenDeactivatingUserBeingEquipmentApproverShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var equipmentToCreate = new CreateEquipmentRequestDto
        {
            Name = "Hard Drive WD Caviar Blue 1TB (WD10EZEX)",
            BrokenStatus = EquipmentBrokenStatus.None,
            SerialNumber = Guid.CreateVersion7().ToString(),
            ApproverId = CtoId,
            Characteristics = "3.5\", SATA 3.0 (6Gbps), 7200 об/мин, буфер 64 МБ, линейная скорость 150/150 МБ/с",
            InvoiceNumber = "DSAMVAS123ASKD123",
            Comment = "=== TEST DATA ===",
            EquipmentTypeId = "PcAccessoriesHdd",
            GuaranteeDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Location = EquipmentLocationType.Belarus,
            PurchaseDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePlace = "Minsk",
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePrice = 134,
            PurchasePriceUsd = 44.5m,
            Storage = LocationType.Vitebsk,
            JiraTask = "qwe"
        };

        await client.CreateEquipmentAsync(equipmentToCreate);

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(CtoId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenDeactivatingUserWithScheduledBookingsShouldResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        const int workplaceId = 2;
        var createDate = DateTime.UtcNow.AddDays(5).Date;

        await client.CreateSelfBooking(workplaceId: workplaceId,
            createDate, createDate);

        await client.AuthAsCeo();

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(EmployeeId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserStatusWhenUserHasPermissionAssignmentShouldDeletePermissionAssignmentsFromDb()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var modsenClientId = _factory.ModsenClientId!.Value;

        var employee = await client.CreateEmployee();
        var employeeId = employee!.Id;

        var project = await client.CreateProject(
                clientId: modsenClientId,
                isBillable: false,
                isPublic: true);

        await client.UpdateUserPermissionAssignmentsAsHttpMessage(
            userId: employeeId,
            resources: new Dictionary<string, string>
            {
                { project!.Id.ToString(CultureInfo.InvariantCulture), project.Name }
            }, _jsonOptions);

        // Act
        var assignmentsBeforeDeleting = await client.GetPermissionAssignments(_jsonOptions, filter: $"ProjectId eq '{project.Id}'");

        await client.SetUserInactiveAsHttpMessage(employeeId);

        var assignmentsAfterDeleting = await client.GetPermissionAssignments(_jsonOptions, filter: $"ProjectId eq '{project.Id}'");

        await client.SetUserActiveAsHttpMessage(employeeId);

        // Assert
        var countOfAssignments = assignmentsBeforeDeleting!.Items.Count;

        assignmentsBeforeDeleting.Items.Should().HaveCount(countOfAssignments);
        assignmentsAfterDeleting!.Items.Should().HaveCount(countOfAssignments - 1);
    }

    [Fact]
    public async Task ActivateUserWhenAuthedAsHeadOfSysAdminDepartmentAndUserToUpdateIsSysAdminShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        using var scope = _factory.Services.CreateScope();

        var portalSettingsAccessor = scope.ServiceProvider.GetRequiredService<IPortalSettingsAccessor>();

        var systemUserId = await portalSettingsAccessor.GetStringValueByName(
            DepartmentSettingNames.SystemAdministrationDepartmentId, _cancellationToken);

        var createdAdmin = await client.CreateSysAdmin(departmentId : systemUserId);

        await client.AuthAsSysAdminHead();

        //Act
        var updatedAdmin = await client.SetUserActive(createdAdmin!.Id);

        //Assert
        updatedAdmin.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateEmployeeStatusWhenAuthAsSGmActivatingUserFromOtherGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const string departmentId = "Java";
        const int resourceGroupId = 2;

        var user = await client.CreateEmployee(resourceGroupId: resourceGroupId, departmentId: departmentId);
        var userId = user!.Id;

        await client.SetUserInactiveAsHttpMessage(userId);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.SetUserActiveAsHttpMessage(userId);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateEmployeeStatusWhenDeactivatingUserInGroupReturnForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        const int resourceGroupId = 1;

        var user = await client.CreateEmployee(resourceGroupId: resourceGroupId);

        // Act
        var response = await client.SetUserInactiveAsHttpMessage(user!.Id);

        // Arrange
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
