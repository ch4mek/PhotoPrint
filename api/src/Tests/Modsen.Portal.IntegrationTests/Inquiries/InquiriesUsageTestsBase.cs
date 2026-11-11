using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Inquiries;

public class InquiriesUsageTestsBase
{
    protected readonly CancellationToken _cancellationToken = CancellationToken.None;
    protected readonly JsonSerializerOptions _serializerOptions;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected readonly IntegrationTestFactory _factory;

    public InquiriesUsageTestsBase(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _jsonOptions;
    }

    internal async Task<UserInquiryAgreementOutput> PrepareInquiries(HttpClient client)
    {
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        //  create template
        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.GroupManager
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        });

        //  create user inquiry for
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

        //  create assign user
        await client.CreateInquiryAssignAsync(new CreateInquiryAssignInput(
            AssigneUserIds: [user!.Id],
            UnassigneUserIds: [],
            template!.Id,
            null,
            today
        ), _jsonOptions);

        //  create inquiry for user
        var inquiryForUser = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        return inquiryForUser;
    }
}
