using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Utilities;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.Utilities.Controllers.Agent;

namespace Modsen.Portal.IntegrationTests.Utilities;

[Collection("GeneralCollection")]
public class AgentControllerTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public AgentControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetPermanentUserInfo()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестИмя",
            LastNameRu = "ТестФамилия",
            PatronymicRu = "ТестОтчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            IsGraduateJobPlacement = true,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.Today).AddDays(-9),
            ContractDateTo = DateOnly.FromDateTime(DateTime.Today).AddDays(3),
            CurrentLocation = "Ghomel",
            DateOfBirth = new DateOnly(1999, 9, 9),
            HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-9),
            MilitaryServiceStatus = MilitaryServiceStatus.Fit,
            PersonalEmail = null,
            PersonalEmailSecond = null,
            PersonalPhone = null,
            ResourceGroupId = null,
            Telegram = "@tester",
        };

        var expectedUserInfo = new PermanentUserInfoOutput
        {
            FirstName = userToCreate.FirstName,
            LastName = userToCreate.LastName,
            FirstNameRu = userToCreate.FirstNameRu,
            LastNameRu = userToCreate.LastNameRu,
            PatronymicRu = userToCreate.PatronymicRu,
            DateOfBirth = userToCreate.DateOfBirth,
            Email = userToCreate.Email,
            Country = "Belarus",
            Position = userToCreate.Position,
            Location = userToCreate.Location.GetDescription(),
            Departament = userToCreate.DepartmentId,
            IsGraduateJobPlacement = userToCreate.IsGraduateJobPlacement,
            Contracts =
            [
                new()
                {
                    DateFrom = userToCreate.ContractDateFrom.Value,
                    DateTo = userToCreate.ContractDateTo,
                }
            ],
            Managers =
            [
                new()
                {
                    FirstName = "Test",
                    LastName = "HeadOfDepartment",
                    Position = "HeadOfDepartment"
                }
            ],
            CurrentLocation = userToCreate.CurrentLocation,
            HireDate = userToCreate.HireDate,
            MilitaryServiceStatus = userToCreate.MilitaryServiceStatus,
            PersonalEmail = userToCreate.PersonalEmail,
            PersonalEmailSecond = userToCreate.PersonalEmailSecond,
            PersonalPhone = userToCreate.PersonalPhone,
            Telegram = userToCreate.Telegram,
        };

        var createdUser = await client.CreateUser(userToCreate);
        client.Logout();
        client.DefaultRequestHeaders.Add("X-Agent-Api-Key", "tElmRlb7ldaw4d235BzIHR1p4c2oPsm6KL3XFUlI");

        // Act
        var userInfo = await client.GetUserPermanentInfo(new UserInfoInput(createdUser!.Email), _jsonOptions);

        // Assert
        userInfo.Should().NotBeNull();
        userInfo.Should().BeEquivalentTo(expectedUserInfo);
    }

    [Fact]
    public async Task GetImpermanentUserInfo()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "ТестИмя",
            LastNameRu = "ТестФамилия",
            PatronymicRu = "ТестОтчество",
            Color = "#ffffff",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            CountryId = 1,
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            IsGraduateJobPlacement = true,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.Today).AddMonths(-3),
            ContractDateTo = DateOnly.FromDateTime(DateTime.Today).AddMonths(3),
            CurrentLocation = "Ghomel",
            DateOfBirth = new DateOnly(1999, 9, 9),
            HireDate = DateOnly.FromDateTime(DateTime.Today).AddDays(-9),
            MilitaryServiceStatus = MilitaryServiceStatus.Fit,
            PersonalEmail = null,
            PersonalEmailSecond = null,
            PersonalPhone = null,
            ResourceGroupId = null,
            Telegram = "@tester"
        };

        var createdUser = await client.CreateUser(userToCreate);

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
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [createdUser!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        await client.CreateInquiryAsync(createdUser!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 7,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });


        client.Logout();
        client.DefaultRequestHeaders.Add("X-Agent-Api-Key", "tElmRlb7ldaw4d235BzIHR1p4c2oPsm6KL3XFUlI");

        // Act
        var userInfo = await client.GetUserImpermanentInfo(new UserInfoInput(createdUser!.Email), _jsonOptions);

        // Assert
        userInfo.Should().NotBeNull();
        userInfo.BalanceDays.Should().NotBeNull();
        userInfo.BalanceDays.Count.Should().BeGreaterThan(0);
        userInfo.OpenRequests.Should().NotBeNull();
        userInfo.OpenRequests.Count.Should().Be(1);
    }
}
