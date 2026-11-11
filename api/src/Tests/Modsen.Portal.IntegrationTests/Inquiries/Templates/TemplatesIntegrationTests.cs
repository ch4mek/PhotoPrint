using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Inquiries.Templates;

[Collection("InquiriesCollection")]
public class TemplatesIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private const InquiryAcceptPolicyScopes CeoCtoScope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO;
    public TemplatesIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetAllTemplatesResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        // Act
        var response = await client.GetTemplatesAsHttpResponseAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTemplateByIdResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.SocialVacation,
            IsSubstitutesToApproveAllowed = false,
            AcceptPolicies = [ new() { OrderWeight = 1, Scope = CeoCtoScope } ]
        };

        var template = await client.CreateTemplateAsync(createInput);
        // Act

        var response = await client.GetTemplateByIdAsHttpResponseAsync(template.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTemplateByIdResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.HomeOffice,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        // Act
        var response = await client.GetTemplateByIdAsHttpResponseAsync(template.Id + 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTemplateResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.HomeOffice,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        // Act
        var template = await client.CreateTemplateAsync(createInput);

        // Assert
        template!.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTemplateResponseWithBadRequestStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = null,
        };

        // Act
        var response = await client.CreateTemplateAsHttpResponseAsync(createInput, _jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTemplateResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.MilitaryEnlistmentOffice,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var updateTemplateInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type =  InquiryType.HomeOffice,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        // Act
        var updatedTemplate = await client.UpdateTemplateAsync(template!.Id, updateTemplateInput);

        // Assert
        updatedTemplate.Name.Should().Be(updatedTemplate.Name);
    }

    [Fact]
    public async Task UpdateTemplateResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.SocialVacation,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var updateTemplateInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.WeekendTransfer,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        // Act
        var response = await client.UpdateTemplateAsHttpResponseAsync(template!.Id + 1, updateTemplateInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTemplateResponseWithBadRequestStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.SocialVacation,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        var updateTemplateInput = new InquiryTemplateInput
        {
            Name = "Sick leave",
            Type = Core.Inquiries.InquiryType.WeekendTransfer,
        };

        // Act
        var response = await client.UpdateTemplateAsHttpResponseAsync(template!.Id, updateTemplateInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteTemplateResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            AccrualType = AccrualType.OnceInYear,
            TransferType = TransferType.Reset,
            Type = InquiryType.SickDay,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        // Act
        var response = await client.DeleteTemplateAsHttpResponseAsync(template!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTemplateResponseWithNotFoundStatus()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var createInput = new InquiryTemplateInput
        {
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.OwnExpense,
            AcceptPolicies = [new() { OrderWeight = 1, Scope = CeoCtoScope }]
        };

        var template = await client.CreateTemplateAsync(createInput);

        // Act
        var response = await client.DeleteTemplateAsHttpResponseAsync(template!.Id + 1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTemplatesWhenAssignedUserIsInactiveShouldNotCountInactiveUser()
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

        var createdUser = await client.CreateUser(userToCreate, _jsonOptions);

        var inquiryAssignToCreate = new CreateInquiryAssignInput([createdUser!.Id], [], template!.Id, null, null);
        var inquiryAssign = await client.CreateInquiryAssignAsync(inquiryAssignToCreate, _jsonOptions);

        await client.SetUserInactive(createdUser!.Id);

        // Act
        var response = await client.GetTemplatesAsHttpResponseAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var templates = await response.Content.ReadFromJsonAsync<List<InquiryTemplateOutput>>(
            _jsonOptions);
        templates.Should().Contain(x => x.Id == template.Id);
        templates.First(x => x.Id == template.Id).AssignedUsersCount.Should().Be(0);
    }
}
