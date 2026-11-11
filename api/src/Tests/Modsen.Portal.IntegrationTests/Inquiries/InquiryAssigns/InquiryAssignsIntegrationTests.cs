using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Inquiries.InquiryAssigns;

[Collection("InquiriesCollection")]
public class InquiryAssignsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private const InquiryAcceptPolicyScopes CeoCtoScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO;

    public InquiryAssignsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetInquiryTemplatesWhenAssignedInactiveUserShouldReturnWithoutInactiveUser()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCto();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.OwnExpense,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var inquiryAssignToCreate = new CreateInquiryAssignInput(AssigneUserIds: [createdUser!.Id],
            UnassigneUserIds: [], template!.Id, AgreementId: null, EffectiveFrom: null);
        var inquiryAssign = await client.CreateInquiryAssignAsync(inquiryAssignToCreate, _jsonOptions);

        await client.SetUserInactive(createdUser!.Id);

        // Act
        var response = await client.GetAssignsByTemplateIdAsHttpResponseAsync(template.Id, top: 10, skip: 0);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var userList = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserAssignDto>>(_cancellationToken);
        userList!.Items.Should().NotContain(x => x.UserId == createdUser.Id);
    }

    [Fact]
    public async Task GetAgreementTemplatesWhenAssignedInactiveUserShouldReturnWithoutInactiveUser()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.Vacation,
            AccrualType = AccrualType.ProportianallyEachMonth,
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var agreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = template.Id, // Vacation
        };

        var agreementId = await client.CreateAgreemenTemplateWithResultAsync(agreementInput);

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var inquiryAssignToCreate = new CreateInquiryAssignInput(
            AssigneUserIds: [createdUser!.Id],
            UnassigneUserIds: [],
            template!.Id,
            AgreementId: agreementId,
            EffectiveFrom: DateOnly.FromDateTime(DateTime.UtcNow));
        var inquiryAssign = await client.CreateInquiryAssignAsync(inquiryAssignToCreate, _jsonOptions);

        await client.SetUserInactive(createdUser!.Id);

        // Act
        var response = await client.GetAssignsByTemplateIdAsHttpResponseAsync(template.Id, top: 10, skip: 0);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var userList = await response.Content.ReadFromJsonAsync<PageableListOfItems<UserAssignDto>>(_cancellationToken);
        userList!.Items.Should().NotContain(x => x.UserId == createdUser.Id);
    }

    [Fact]
    public async Task GetUserInquiryTemplatesWhenUserHasBeenCreatedShouldHaveDefault()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        // Act
        var createdUser = await client.CreateUser(userToCreate);
        var response = await client.GetAssignsByUserIdAsync(createdUser!.Id);

        // Assert
        response.Count.Should().BeGreaterThanOrEqualTo(8);
    }

    [Fact]
    public async Task GetUserInquiryTemplatesWhenUserHasBeenDeactivatedAndActivatedShouldHaveDefault()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестЮзерИмя",
            LastNameRu = "ТестЮзерФамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        // Act
        await client.SetUserInactive(createdUser!.Id);
        await client.SetUserActive(createdUser!.Id);
        await Task.Delay(500);
        var assignsAfterActivation = await client.GetAssignsByUserIdAsync(createdUser!.Id);

        // Assert
        assignsAfterActivation.Count.Should().BeGreaterThanOrEqualTo(8);
    }
}
