// TODO: FIX TESTS in next story

using System.Net;
using System.Net.Http.Json;
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
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.Inquiries.Accepts;

[Collection("InquiriesCollection")]
public class InquiryAcceptsIntegrationTests : InquiriesUsageTestsBase
{
    public InquiryAcceptsIntegrationTests(IntegrationTestFactory factory)
        : base(factory)
    {
    }

    //    [Fact]
    //    public async Task GetAcceptsByInquiryId_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var accepts = await client.GetInquiryAcceptsAsync(inquiry.Id);

    //        // Assert
    //        accepts.Count.Should().Be(inquiry.UserInquiryAccepts.Count);
    //    }

    //    [Fact]
    //    public async Task GetAcceptsByInquiryId_WhenInquiryNotExists_ResponseWithNotFoundStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var response = await client.GetInquiryAcceptsAsHttpResponseAsync(inquiry.Id + 1);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    //    }

    //    [Fact]
    //    public async Task GetAcceptsByInquiryId_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        await client.AuthAsEmployee();

    //        var response = await client.GetInquiryAcceptsAsHttpResponseAsync(inquiry.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task AcceptInquiry_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var created = await client.AcceptInquiryAsync(inquiry.Id, accept.Id);

    //        using var scope = _factory.Services.CreateScope();

    //        await scope.PrepareAgreementsWithTransactions(client, inquiry.InquiryUserId);

    //        // Assert

    //        created.Status.Should().Be(UserInquiryAcceptStatus.Accepted);
    //    }

    //    [Fact]
    //    public async Task AcceptInquiry_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        await client.AuthAsCto();

    //        var response = await client.AcceptInquiryAsHttpResponseAsync(inquiry.Id, accept.Id);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task AcceptInquiry_ResponseWithNotFoundStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var response = await client.AcceptInquiryAsHttpResponseAsync(inquiry.Id, accept.Id + 1);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    //    }

    //    [Fact]
    //    public async Task RejectInquiry_ResponseWithOk()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var created = await client.RejectInquiryAsync(inquiry.Id, accept.Id);

    //        var rejectedInquiry = await client.GetInquiryAsync(inquiry.Id);

    //        // Assert
    //        created.Status.Should().Be(UserInquiryAcceptStatus.Rejected);

    //        rejectedInquiry.Status.Should().Be(UserInquiryAcceptStatus.Rejected);
    //    }

    //    [Fact]
    //    public async Task RejectInquiry_ResponseWithForbiddenStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        await client.AuthAsCto();

    //        var response = await client.RejectInquiryAsHttpResponseAsync(inquiry.Id, accept.Id);
    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    //    }

    //    [Fact]
    //    public async Task RejectInquiry_ResponseWithNotFoundStatus()
    //    {
    //        // Arrange
    //        using var client = _factory.CreateClient();

    //        var inquiry = await PrepareInquiries(client);

    //        // Act
    //        var accept = inquiry.UserInquiryAccepts.First();

    //        var response = await client.RejectInquiryAsHttpResponseAsync(inquiry.Id, accept.Id + 1);

    //        // Assert
    //        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    //    }

    [Fact]
    public async Task RejectAsCtoShouldBeRejected()
    {
        // Arrange
        var client = _factory.CreateClient();
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

        //await client.AuthAsCto();

        // Act
        var response = await client.RejectInquiryAsync(inquiryForUser!.Id);

        // Assert
        response.Should().HaveCount(1);
        var entry = response.First();
        entry!.Status.Should().Be(UserInquiryAcceptStatus.Rejected);
        entry.RejectedByUser!.Id.Should().Be(3);
    }

    [Fact]
    public async Task RejectAsRequesterShouldBeCancelled()
    {
        // Arrange
        var client = _factory.CreateClient();
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

        await client.AuthAs(user.Email);

        // Act
        var response = await client.RejectInquiryAsync(inquiryForUser!.Id);

        // Assert
        response.Should().HaveCount(1);
        var entry = response.First();
        entry!.Status.Should().Be(UserInquiryAcceptStatus.Cancelled);
        entry.RejectedByUser!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task ApproveAsRequesterShouldBeForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
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

        await client.AuthAs(user.Email);

        // Act
        var httpResponse = await client.AcceptInquiryAsHttpResponseAsync(inquiryForUser!.Id);

        // Assert
        httpResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AcceptAsSubstituteWhenMainApproverIsAbsentShouldBeAccepted()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var gmId = _factory.GroupManagerId!.Value;

        //  create template
        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
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
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

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

        //  create assign approver
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [gmId], UnassigneUserIds: [], template!.Id, null, today), _jsonOptions);

        //  create inquiry for approver (1)
        var inquiryForApprover = await client.CreateInquiryAsync(gmId, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(-2),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.AcceptInquiryAsync(inquiryForUser!.Id);

        // Assert
        response.Should().HaveCount(1);
        var entry = response.First();
        entry!.Status.Should().Be(UserInquiryAcceptStatus.Accepted);
        entry.AcceptedByUser!.Id.Should().Be(_factory.HeadOfDepartmentUserId!.Value);
    }

    [Fact]
    public async Task AcceptAsSubstituteWhenMultiplePMsAreAbsentShouldBeAllAbsentAccepted()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var gmId = _factory.GroupManagerId!.Value;

        //  create template
        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
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

        //  create pms
        var pm2 = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "PMO",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });
        var pm3 = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "PMO",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        //  create projects
        var project1 = await client.CreateProject(new CreateProjectInput
        {
            IsBillable = true,
            IsPublic = false,
            ClientId = _factory.ModsenClientId!.Value,
            Name = "Project 1",
            ManagerId = _factory.PmoHeadOfDepartmentUserId!.Value,
        });

        var project2 = await client.CreateProject(new CreateProjectInput
        {
            IsBillable = true,
            IsPublic = false,
            ClientId = _factory.ModsenClientId!.Value,
            Name = "Project 2",
            ManagerId = pm2!.Id,
        });

        var project3 = await client.CreateProject(new CreateProjectInput
        {
            IsBillable = true,
            IsPublic = false,
            ClientId = _factory.ModsenClientId!.Value,
            Name = "Project 3",
            ManagerId = pm3!.Id
        });

        var dateFrom = today.AddDays(-2);
        var dateTo = today.AddDays(4);

        //  create project-user assigns
        await client.CreateProjectAssignmentAsHttpMessageAsync(user!.Id, project1!.Id, dateFrom, dateTo);
        await client.CreateProjectAssignmentAsHttpMessageAsync(user!.Id, project2!.Id, dateFrom, dateTo);
        await client.CreateProjectAssignmentAsHttpMessageAsync(user!.Id, project3!.Id, dateFrom, dateTo);

        //  create inquiry template for pms
        var templateForPm = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    UserId = _factory.CtoUserId,
                    Scope = InquiryAcceptPolicyScopes.User,
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        });

        //  create assign user
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        //  create assign template to pms
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [project2.ManagerId!.Value, project3.ManagerId!.Value],
                UnassigneUserIds: [], templateForPm!.Id, null, today), _jsonOptions);

        //  create inquiry for pms
        var pmInquiry1 = await client.CreateInquiryAsync(project2.ManagerId!.Value, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Files = [],
            Hours = null,
            InquiryTemplateId = templateForPm!.Id,
            Notes = null,
        });

        var pmInquiry2 = await client.CreateInquiryAsync(project3.ManagerId!.Value, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Files = [],
            Hours = null,
            InquiryTemplateId = templateForPm!.Id,
            Notes = null,
        });

        //  approve inquiry for pms
        await client.AcceptInquiryAsync(pmInquiry1!.Id);
        await client.AcceptInquiryAsync(pmInquiry2!.Id);

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

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.AcceptInquiryAsync(inquiryForUser!.Id);

        // Assert
        response.Should().HaveCount(2);
        response!.Should().OnlyContain(x => x.Status == UserInquiryAcceptStatus.Accepted
            && x.AcceptedByUser!.Id == _factory.HeadOfDepartmentUserId!.Value
            && (x.AcceptUser!.Id == project2.ManagerId.Value || x.AcceptUser.Id == project3.ManagerId.Value));
    }

    [Fact]
    public async Task RejectAsSubstituteWhenMainApproverIsAbsentShouldBeRejected()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var gmId = _factory.GroupManagerId!.Value;

        //  create template
        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.GroupManager,
                    SubstituteScope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        });
        var templateForApprover = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 1,
                    Scope = InquiryAcceptPolicyScopes.User,
                    UserId = _factory.CtoUserId,
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
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
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

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

        //  create assign approver
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [gmId], UnassigneUserIds: [], templateForApprover!.Id, null, today), _jsonOptions);

        //  create inquiry for approver (1)
        var inquiryForApprover = await client.CreateInquiryAsync(gmId, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today.AddDays(-2),
            DateTo = today.AddDays(2),
            Files = [],
            Hours = null,
            InquiryTemplateId = templateForApprover!.Id,
            Notes = null,
        });
        await client.AcceptInquiryAsync(inquiryForApprover!.Id);

        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.RejectInquiryAsync(inquiryForUser!.Id);

        // Assert
        response.Should().HaveCount(1);
        var entry = response.First();
        entry!.Status.Should().Be(UserInquiryAcceptStatus.Rejected);
        entry.RejectedByUser!.Id.Should().Be(_factory.HeadOfDepartmentUserId!.Value);
    }

    [Fact]
    public async Task AcceptAsSubstituteWhenMainApproverIsNotAbsentResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
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
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = _factory.PmoHeadOfDepartmentUserId!.Value
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
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
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

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


        await client.AuthAs(_factory.PmoHeadOfDepartmentEmail!);

        // Act
        var response = await client.AcceptInquiryAsHttpResponseAsync(inquiryForUser!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectAsSubstituteWhenMainApproverIsNotAbsentResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
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
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment,
                    SubstituteScope = InquiryAcceptPolicyScopes.User,
                    SubstituteUserId = _factory.PmoHeadOfDepartmentUserId!.Value
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = true,
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
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

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


        await client.AuthAs(_factory.PmoHeadOfDepartmentEmail!);

        // Act
        var response = await client.RejectInquiryAsHttpResponseAsync(inquiryForUser!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RejectAsChiefWhenInquiryIsApprovedAndApproversListIsEmptyShouldBeRejected()
    {
        // Arrange
        var client = _factory.CreateClient();
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
                    Scope = InquiryAcceptPolicyScopes.ProjectManager,
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

        //  create pm
        var pm = await client.CreateUser(new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "PMO",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        });

        //  create project with pm as manager and assign user to it
        var project = await client.CreateProject(new CreateProjectInput
        {
            IsBillable = true,
            IsPublic = false,
            ClientId = _factory.ModsenClientId!.Value,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            ManagerId = pm!.Id,
        });
        await client.CreateProjectAssignmentAsHttpMessageAsync(user!.Id, project!.Id, today, today);

        //  create assign user
        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        //  create inquiry for user — pm gets WaitingForApprove accept
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

        //  CTO accepts as chief → pm accept becomes Accepted, inquiry becomes Approved, WaitingForApprove list is empty
        await client.AcceptInquiryAsync(inquiryForUser!.Id);

        // Act
        var response = await client.RejectInquiryAsHttpResponseAsync(inquiryForUser!.Id);

        // Assert
        inquiryForUser.UserInquiryAccepts.Should().HaveCount(1);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accepts = await response.Content.ReadFromJsonAsync<List<UserInquiryAcceptOutput>>(_serializerOptions, _cancellationToken);
        accepts.Should().HaveCount(1);
        accepts![0].Status.Should().Be(UserInquiryAcceptStatus.Rejected);
    }
}
