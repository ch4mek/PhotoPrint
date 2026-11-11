using System.Net;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.Inquiries.Agreements;

[Collection("InquiriesCollection")]
public class InquiryAgreementsIntegrationTests : InquiriesUsageTestsBase
{
    public InquiryAgreementsIntegrationTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task DeleteAgreementAsyncWhenAgreementDoesNotExistReturnsNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        //Act
        var response = await client.DeleteAgreementAsync(1000);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAgreementAsyncWhenRequestValidShouldReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var agreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        //Act
        var result = await client.CreateAgreemenTemplateAsync(agreementInput);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateAgreementAsyncWhenRequestValidUpdatesAgreement()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createAgreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        var createdAgreementId = await client.CreateAgreemenTemplateWithResultAsync(createAgreementInput);
        
        var agreementInput = new AgreementTemplateInput
        {
            Name = "Updated Agreement",
            DaysPerContractYear = 30,
            IsDefault = true,
        };

        //Act
        var updatedAgreement = await client.UpdateAgreemenTemplateAsync(createdAgreementId, agreementInput);

        //Assert
        updatedAgreement.DaysPerContractYear.Should().Be(agreementInput.DaysPerContractYear);
        updatedAgreement.IsDefault.Should().Be(agreementInput.IsDefault);
    }

    [Fact]
    public async Task UpdateAgreementAsyncWhenAgreementDoesNotExistsReturnNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var command = new UserAgreementUpdateInput
        {
            DateTo = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            DaysPerContractYear = 10,
            FreezeUntilDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(2))
        };

        //Act
        var updatedAgreementResponse = await client.UpdateAgreementAsHttpResponseAsync(-1, command);

        //Assert
        updatedAgreementResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAgreementAsyncWhenAgreementContainsDuplicatedNameShouldReturnConflict()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createAgreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        await client.CreateAgreemenTemplateAsync(createAgreementInput);

        var agreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 11,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        //Act
        var result = await client.CreateAgreemenTemplateAsync(agreementInput);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateAgreementAsyncWhenAgreementContainsUnavailableInquiryTypeShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var agreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 11,
            IsDefault = true,
            InquiryTemplateId = 1, //Social vacation
        };

        //Act
        var result = await client.CreateAgreemenTemplateAsync(agreementInput);

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAgreementAsyncWhenAgreementDoesNotExistReturnsNotFound()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var agreementInput = new AgreementTemplateInput
        {
            Name = "Updated Agreement",
            DaysPerContractYear = 30,
            IsDefault = true,
        };

        //Act
        var response = await client.UpdateAgreemenTemplateWithResponceAsync(1000, agreementInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAgreementAsyncWhenAgreementContainsDuplicatedNameShouldReturnConflict()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var createFirstAgreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        await client.CreateAgreemenTemplateAsync(createFirstAgreementInput);

        var createSecondAgreementInput = new CreateAgreementTemplateInput
        {
            Name = "Test Agreement 2",
            DaysPerContractYear = 11,
            IsDefault = true,
            InquiryTemplateId = 8, // Vacation
        };

        var createdAgreementId = await client.CreateAgreemenTemplateWithResultAsync(createSecondAgreementInput);

        var agreementInput = new AgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 11,
            IsDefault = true,
        };

        //Act
        var response = await client.UpdateAgreemenTemplateWithResponceAsync(createdAgreementId, agreementInput);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // TODO - fix tests in transaction story

    //[Fact]
    //public async Task GetAgreementHistoryAsync_WhenRequestValid_ReturnsOutput()
    //{
    //    //Arrange
    //    using var client = _factory.CreateClient();
    //    await client.AuthAsCeo();

    //    await PrepareInquiries(client);

    //    //Act
    //    var response = await client.GetAgreementHistoryAsResponseMessage(
    //        _factory.CeoUserId.GetValueOrDefault(),
    //        AgreementType.Unknown,
    //        DateOnly.FromDateTime(DateTime.UtcNow).AddYears(3),
    //        DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-9));

    //    //Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //}

    //[Fact]
    //public async Task GetAgreementBalanceAsync_WhenRequestValid_ReturnsDecimal()
    //{
    //    //Arrange
    //    using var client = _factory.CreateClient();
    //    await client.AuthAsCeo();

    //    await PrepareInquiries(client);

    //    var userAgreements = await client.GetAgreementsAsync((int)_factory.CeoUserId!);

    //    var agreementId = userAgreements.FirstOrDefault().Id;

    //    //Act
    //    var response = await client.GetAgreementBalanceAsHttpResponseAsync(agreementId);

    //    //Assert
    //    response.StatusCode.Should().Be(HttpStatusCode.OK);
    //}

    [Fact]
    public async Task CreateAgreementTransactionsWhenRequestIsValidReturnsOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

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
            DepartmentId = "Net",
            ResourceGroupId = _factory.ResourceGroupId,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        var createUserContractInput = new CreateUserContractInput(
            user!.Id,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
            null, null, null, null);

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies =
            [
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            ],
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            Type = InquiryType.Vacation,
            AccrualType = AccrualType.ProportianallyEachMonth,
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
        });

        var agreementId = await client.CreateAgreemenTemplateWithResultAsync(
            new CreateAgreementTemplateInput
        {
            Name = "Test Agreement",
            DaysPerContractYear = 25,
            IsDefault = true,
            InquiryTemplateId = template.Id, // Vacation
        });

        var inquiryAssignToCreate = new CreateInquiryAssignInput(AssigneUserIds: [user!.Id],
            UnassigneUserIds: [], template!.Id, AgreementId: agreementId, EffectiveFrom: createUserContractInput.ContractFrom);
        var inquiryAssign = await client.CreateInquiryAssignAsync(inquiryAssignToCreate, _jsonOptions);

        //Act
        var response = await client
            .CreateAgreementTransactionsAsync(user.Id, agreementId, new CorrectionAgreementTransactionInput
            {
                DaysCount = 01.23m,
                Description = "test"
            });

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var history = await client.GetUserAgreementHistoryAsync(user.Id);
        history.Should().NotBeNull();
        history.Items.Count.Should().Be(1);
        history.Items.First().Accrued.Should().Be(01.23m);
        history.Items.First().Description.Should().Be("test");
        history.Items.First().IsCreatedManually.Should().BeTrue();
        history.Items.First().UserAgreementId.Should().Be(null);
        history.Items.First().TransactionType.Should().Be(UserAgreementTransactionType.Accrual);
    }
}
