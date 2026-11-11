using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;
using Modsen.Portal.IntegrationTests.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;

namespace Modsen.Portal.IntegrationTests.Unstable;

[Collection("UnstableCollection")]
public class UnstableIntegrationTests : InquiriesUsageTestsBase, IClassFixture<IntegrationTestFactory>
{
    public UnstableIntegrationTests(IntegrationTestFactory factory) : base(factory){ }

    [Fact]
    public async Task GetUserAgreementAssignsWhenUserHasBeenDeactivatedShouldNotHaveUserAssign()
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

        var createdUser = await client.CreateUser(userToCreate, _jsonOptions);

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = AccrualType.ProportianallyEachMonth,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Days,
            TransferType = TransferType.Transferred,
            Type = InquiryType.Vacation
        });

        var agreementTemplateId = await client.CreateAgreemenTemplateWithResultAsync(new CreateAgreementTemplateInput
        {
            InquiryTemplateId = template!.Id,
            IsDefault = false,
            Name = "agreement test",
            DaysPerContractYear = 1234
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [createdUser!.Id], UnassigneUserIds: [],
                template!.Id, agreementTemplateId, DateOnly.FromDateTime(DateTime.UtcNow)), _jsonOptions);

        var assignsBeforeDeactivation = await client.GetUserAgreementAssignsAsync(agreementTemplateId);

        // Act
        await client.SetUserInactive(createdUser!.Id);
        var assignsAfterDeactivation = await client.GetUserAgreementAssignsAsync(agreementTemplateId);

        // Assert
        assignsBeforeDeactivation!.Items.Should().OnlyContain(x => x.UserId == createdUser!.Id);
        assignsAfterDeactivation!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserInquiryTemplateAssignsWhenUserHasBeenDeactivatedShouldNotHaveUserAssign()
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

        var createdUser = await client.CreateUser(userToCreate, _jsonOptions);

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [createdUser!.Id], UnassigneUserIds: [],
                template!.Id, null, DateOnly.FromDateTime(DateTime.UtcNow)), _jsonOptions);

        var assignBeforeDeactivation = await client.GetUserInquiryTemplatesAssignsAsync(template!.Id);

        // Act
        await client.SetUserInactive(createdUser!.Id);
        await Task.Delay(500);
        var assignAfterDeactivation = await client.GetUserInquiryTemplatesAssignsAsync(template!.Id);

        // Assert
        assignBeforeDeactivation!.Items.Should().Contain(x => x.UserId == createdUser!.Id);
        assignAfterDeactivation!.Items.Should().NotContain(x => x.UserId == createdUser!.Id);
    }

    [Fact]
    public async Task UpdateUserContractWhenAuthAsCeoOrCtoAndDataIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var input = new CreateUserContractInput(
            userId,
            DateOnly.FromDateTime(DateTime.Now).AddYears(-6),
            DateOnly.FromDateTime(DateTime.Now).AddYears(-5),
            null, null, null, null);

        var response = await client.CreateContracts(input)
            .WithResponseType<UserContractOutput>();

        // Act
        var body = new UpdateUserContractInput(response.Id, response.ContractFrom, response.ContractTo!.Value,
            null, null, null, null);

        var putResponse = await client.UpdateContracts(body);

        // Assert
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTrackedTimePerUsersWhenYearAndMonthSpecifiedShouldReturnTrackedTimePerUserForTheSpecifiedPeriod()
    {
        // Arrange
        _factory.CleanTimesheets();

        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var jsUser = await client.CreateEmployee(departmentId: "Js");
        var javaUser = await client.CreateEmployee(departmentId: "Java");

        var clientId = _factory.ModsenClientId!.Value;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var firstProject = await client.CreateProject(clientId: clientId, isBillable: true);
        var secondProject = await client.CreateProject(clientId: clientId, isBillable: false);

        await client.CreateProjectAssignment(userId: javaUser!.Id, projectId: firstProject!.Id);
        await client.CreateProjectAssignment(userId: javaUser.Id, projectId: secondProject!.Id, isTimeBillable: false);
        await client.CreateProjectAssignment(userId: jsUser!.Id, projectId: secondProject.Id, isTimeBillable: false);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(userId: javaUser.Id, projectId: firstProject.Id, date: date, trackedInMinutes: 480),
            await client.TrackFactAsync(userId: javaUser.Id, projectId: secondProject.Id, date: date, trackedInMinutes: 180),
            await client.TrackFactAsync(userId: javaUser.Id, projectId: secondProject.Id, date: date, trackedInMinutes: 120)
        };

        await client.AuthAs(jsUser.Email);
        await client.TrackFactAsync(projectId: secondProject.Id, date: date, trackedInMinutes: 100);
        await client.TrackFactAsync(projectId: secondProject.Id, date: date, trackedInMinutes: 150);


        // Act
        await client.AuthAsCto();
        var result = await client.GetTrackedTimePerUsersReportAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        result.Should().NotBeNull();

        var employeeResult = result.Items.First(x => x.UserId == javaUser.Id);
        employeeResult.ProjectsCount.Should().Be(2);
        employeeResult.TotalTimeInMinutes.Should().Be(780);

        var headResult = result.Items.First(x => x.UserId == jsUser.Id);
        headResult.ProjectsCount.Should().Be(1);
        headResult.TotalTimeInMinutes.Should().Be(250);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }

    [Fact]
    public async Task GetInquiryByIdWhenInquiryOpenAndPositionsOfUsersChangedResponseWithUpdatedPositions()
    {
        // Arrange
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
            DepartmentId = "Ruby",
            ResourceGroupId = null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1
        }, _jsonOptions);

        var existingHod = (await client.GetUsersWithSearch(
                filter: "(isActive eq true) and (departmentId in ('Ruby')) and (position in ('HeadOfDepartment'))"))
            .Items.FirstOrDefault();

        var manager = existingHod ??
            await client.CreateUser(new CreateUserInput
            {
                Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                FirstName = Guid.CreateVersion7().ToAlphabetic(),
                LastName = Guid.CreateVersion7().ToAlphabetic(),
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Location = LocationType.Minsk,
                Position = PortalRoleNames.HeadOfDepartment,
                DepartmentId = "Ruby",
                ResourceGroupId = null,
                Color = "#123412",
                CountryId = 1,
                RegisteredLocationId = 1
            }, _jsonOptions);

        var template = await client.CreateTemplateAsync(new InquiryTemplateInput
        {
            AcceptPolicies = new List<UpdateInquiryTemplateAcceptPolicyDto>
            {
                new UpdateInquiryTemplateAcceptPolicyDto
                {
                    OrderWeight = 2,
                    Scope = InquiryAcceptPolicyScopes.HeadOfDepartment
                }
            },
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingPaid,
            Comment = null,
            IsIncludeHolidays = true,
            IsIncludeWeekends = true,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            TrackingUnitType = TrackingUnit.Hours,
            TransferType = null,
            Type = InquiryType.SickLeave
        });

        await client.CreateInquiryAssignAsync(
            new CreateInquiryAssignInput(AssigneUserIds: [user!.Id], UnassigneUserIds: [],
                template!.Id, null, today), _jsonOptions);

        var inquiry = await client.CreateInquiryAsync(user!.Id, new UserInquiryInput
        {
            AgreementTemplateId = null,
            DateFrom = today,
            DateTo = today,
            Files = [],
            Hours = 5,
            InquiryTemplateId = template!.Id,
            Notes = null,
        });

        await client.AuthAsCto();

        var userToUpdate = new UpdateUserInput
        {
            Id = user.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = user.Location,
            Position = PortalRoleNames.GroupManager,
            DepartmentId = user.DepartmentId,
            ResourceGroupId = user.ResourceGroupId,
            Color = user.Color,
            CountryId = user.CountryId,
            RegisteredLocationId = user.RegisteredLocationId
        };

        var managerToUpdate = new UpdateUserInput
        {
            Id = manager!.Id,
            Email = null,
            FirstName = null,
            LastName = null,
            Location = LocationType.Vitebsk,
            Position = PortalRoleNames.HeadOfDepartment,
            DepartmentId = manager.DepartmentId,
            ResourceGroupId = manager.ResourceGroupId,
            Color = manager.Color,
            CountryId = manager.CountryId,
            RegisteredLocationId = manager.RegisteredLocationId
        };

        await client.UpdateUserAsHttpMessageAsync(userToUpdate, _jsonOptions);
        await client.UpdateUserAsHttpMessageAsync(managerToUpdate, _jsonOptions);

        // Act
        var result = await client.GetInquiryByIdAsync(inquiry!.Id);

        // Assert
        result!.ManagerOfUser!.Id.Should().Be(managerToUpdate.Id);
        result.ManagerOfUser.Position.Should().Be(managerToUpdate.Position);
        result.CreatedForUser!.Position.Should().Be(userToUpdate.Position);
    }

    [Fact]
    public async Task GetCommentsWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var inquiry = await PrepareInquiries(client);

        var created = await client.CreateInquiryCommentAsync(inquiry.Id, "[{\"type\":\"p\",\"children\":[{\"text\":\"Comment\"}]}]");

        // Act
        var comments = await client.GetInquiryCommentsAsync(inquiry.Id);

        // Assert
        comments!.Count.Should().Be(1);
    }

    [Fact]
    public async Task GetUpcomingEventsWhenUserHasUpdatedUpcomingAbsenceShouldReturnUpdatedAbsence()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var day = DateOnly.FromDateTime(DateTime.UtcNow);

        //  create inquiry template
        var templateToCreate = new InquiryTemplateInput
        {
            Name = "Own expense third",
            AcceptPolicies = [
                new UpdateInquiryTemplateAcceptPolicyDto{ OrderWeight = 1, Scope = InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO },
                new UpdateInquiryTemplateAcceptPolicyDto{ OrderWeight = 2, Scope = InquiryAcceptPolicyScopes.HeadOfDepartment }
            ],
            AccrualType = null,
            ActivityType = ActivityType.NonWorkingUnpaid,
            Comment = "bar",
            IsIncludeHolidays = false,
            IsIncludeWeekends = false,
            IsProhibitOnProbationPeriod = false,
            IsRequireAttachments = false,
            IsSubstitutesToApproveAllowed = false,
            TrackingUnitType = TrackingUnit.Days,
            TransferType = null,
            Type = InquiryType.OwnExpense
        };

        var template = await (await client.CreateTemplateAsHttpResponseAsync(templateToCreate, _jsonOptions))
            .WithResponseType<InquiryTemplateOutput>();

        //  assign user to inquiry
        var assignToCreate = new CreateInquiryAssignInput(AssigneUserIds: [_factory.EmployeeUserId!.Value],
            UnassigneUserIds: [], template.Id, AgreementId: null, EffectiveFrom: day);

        await client.CreateInquiryAssignAsync(assignToCreate, _jsonOptions);

        // update user claims
        var claimsToUpdate = new UserClaimsInput
        {
            ClaimTypes = ["ViewDashboard"]
        };

        await client.PatchAsJsonAsync($"/api/users/{_factory.EmployeeUserId!.Value}/claims", claimsToUpdate);

        //  create request absence
        await client.AuthAsEmployee();

        var inquiryToCreate = new UserInquiryInput
        {
            AgreementTemplateId = null,
            InquiryTemplateId = template.Id,
            DateFrom = day,
            DateTo = day,
            Files = [],
            Hours = null
        };

        var inquiry = await client.CreateInquiryAsync(_factory.EmployeeUserId!.Value, inquiryToCreate);

        var inquiryToUpdate = new UpdateUserInquiryInput
        {
            DateFrom = day.AddDays(20),
            DateTo = day.AddDays(20),
            Notes = "Transfer"
        };

        await client.UpdateInquiryAsync(inquiry.Id, inquiryToUpdate);

        // Act
        var response = await client.GetUpcomingEvents(_factory.EmployeeUserId!.Value, 7);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var upcomingEvents = await response.Content.ReadFromJsonAsync<List<UpcomingEventOutput>>(
            _serializerOptions, _cancellationToken);

        var upcomingEvent = upcomingEvents.First(x => x.Status == UserInquiryStatus.Open.ToString() &&
            x.StartDate == day.AddDays(20));
        upcomingEvent.Status.Should().Be(UserInquiryStatus.Open.ToString());
        upcomingEvent.Name.Should().Be(template.Name);
        upcomingEvent.StartDate.Should().Be(inquiryToUpdate.DateFrom);
        upcomingEvent.EndDate.Should().Be(inquiryToUpdate.DateTo);
        upcomingEvent.Type.Should().Be(UpcomingEventType.Absence.GetDescription());
        upcomingEvent.PerformanceReviewDateType.Should().BeNull();
        upcomingEvent.InquiryType.Should().Be(template.Type);
    }

    [Fact]
    public async Task GetProjectPermissionAssignmentsWhenSearchingByProjectNameShouldResponseWithPermissionAssignmentsWithAccordingProjectName()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var clientId = _factory.ModsenClientId.GetValueOrDefault();
        const int pageSize = 10;

        var employee = await client.CreateEmployee();

        var searchProject = await client.CreateProject(clientId: clientId,
            isPublic: false,
            isBillable: true);

        await client.CreateProjectAssignment(
            userId: employee.Id,
            projectId: searchProject.Id);

        var publicProject = await client.CreateProject(clientId: clientId,
            isPublic: true,
            isBillable: false);

        var assignments = await client.UpdateUserPermissionAssignments(userId: employee.Id,
                resources: new Dictionary<string, string>
                {
                    { searchProject.Id.ToString(CultureInfo.InvariantCulture), searchProject.Name },
                    { publicProject.Id.ToString(CultureInfo.InvariantCulture), publicProject.Name }
                });

        var assignment = assignments!.First(assignment => assignment.ResourceId == searchProject.Id.ToString());

        var expectedAssignments = new List<ProjectPermissionAssignmentOutput>
        {
            new()
            {
                AssignmentId = assignment.AssignmentId,
                UserId = employee.Id,
                ProjectId = searchProject.Id.ToString(CultureInfo.InvariantCulture),
                UserFullName = $"{employee.LastName} {employee.FirstName}",
                ProjectName = assignment.ResourceName,
                UserColor = employee.Color
            }
        };

        // Act
        var actualAssignments = await client.GetPermissionAssignments(_jsonOptions, filter: $"contains(tolower(projectName), tolower('{searchProject.Name}'))",
                top: pageSize);

        // Assert
        actualAssignments!.Items.Should().ContainEquivalentOf(expectedAssignments[0]);
        actualAssignments.Total.Should().Be(1);
    }

    [Fact]
    public async Task GetProjectsByClientIdAsyncWhenAuthedAsHeadOfDepartmentShouldResponseWithProjectsWithUsersFromSameDepartment()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var createdPmoEmployee = await httpClient.CreateEmployee(departmentId: "PMO");
        var employeeId = createdPmoEmployee!.Id;

        const string filter = "((departmentId eq 'PMO') and (position eq 'HeadOfDepartment'))";
        var users = await httpClient.GetUsersWithSearch(filter: filter, top: 1);

        var client = await httpClient.CreateClientAsync();
        var clientId = client!.Id;

        var project = await httpClient.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await httpClient.CreateProjectAssignment(userId: employeeId, projectId: projectId);

        await httpClient.CreateProject(clientId: clientId, isPublic: false, isBillable: false);

        var expectedProjects = new List<ClientProjectOutput>
        {
            new()
            {
                Id = project.Id,
                Color = project.Color,
                IsActive = project.IsActive,
                Name = project.Name,
                FactTrackedTime = null,
                BillableTrackedTime = null
            }
        };

        await httpClient.AuthAs(users.Items.First().Email);

        // Act
        var result = await httpClient.GetProjectsByClientIdAsync(clientId);

        // Assert
        result.Should().BeEquivalentTo(expectedProjects);
    }

    public static IEnumerable<object[]> ActiveIntervals =>
       new List<object[]>
       {
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), DateOnly.FromDateTime(DateTime.UtcNow) },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)), null },
            new object []{ DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), null }
       };

    [Theory]
    [MemberData(nameof(ActiveIntervals))]
    public async Task GetOdpProjectUsersWhenAssignIntervalIncludesCurrentDateOrLaterShouldResponseWithActiveStatus(DateOnly dateFrom, DateOnly? dateTo)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            Color = "#ffffff",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = "Js",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate, _jsonOptions);

        var projectToCreate = new CreateProjectInput
        {
            ClientId = _factory.ModsenClientId!.Value,
            Color = "#ffffff",
            ManagerId = null,
            Name = Guid.CreateVersion7().ToAlphabetic(),
            IsBillable = false,
            IsPublic = false
        };

        var createdProjectResponse = await client.CreateProjectAsHttpResponse(projectToCreate);

        var createdProject = await createdProjectResponse.Content.ReadFromJsonAsync<ProjectOutput>(_cancellationToken);

        var projectUserToCreate = new CreateUserProjectAssignmentInput(dateFrom, dateTo, IsTimeOnProjectBillable: true,
            Fte: 1m, UserOnProjectSeniorityLevelId: "SeniorL2");

        var projectUserResponse = await client.CreateProjectAssignment(user!.Id, createdProject.Id,
            projectUserToCreate);

        projectUserResponse.IsSuccessStatusCode.Should().BeTrue();

        // Act
        var response = await client.GetUserProjects($"filter=UserId eq {user.Id} and ProjectId eq {createdProject.Id}&top=20&skip=0");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var projectUsers = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpProjectUser>>(_cancellationToken);

        projectUsers.Items.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetPieChartProjectsClientsAndUsersWhenAuthAsGroupManagerShouldReturnGroupManagerProjectsTimeForTheSpecifiedPeriod()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var createdUserFromGroup = await client.CreateEmployee(resourceGroupId: 1);
        var netUserId = createdUserFromGroup!.Id;
        var createdUserNotFromGroup = await client.CreateEmployee(departmentId: "Java", resourceGroupId: 2);
        var javaUserId = createdUserNotFromGroup!.Id;

        var projectClient = await client.CreateClientAsync();
        var clientId = projectClient!.Id;

        var project = await client.CreateProject(clientId: clientId);
        var projectId = project!.Id;

        await client.CreateProjectAssignment(userId: netUserId, projectId: projectId);
        await client.CreateProjectAssignment(userId: javaUserId, projectId: projectId);

        await client.TrackFactAsync(projectId: projectId, userId: javaUserId, trackedInMinutes: 160, date: date);

        var timesheets = new List<TimesheetCoreDto>
        {
            await client.TrackFactAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 150, date: date),
            await client.TrackFactAsync(projectId: projectId, userId: netUserId, trackedInMinutes: 100, date: date),

            await client.TrackBillableAsync(projectId: projectId, userId: javaUserId, trackedInMinutes: 120, date: date)
        };

        // Act
        await client.AuthAs(EmailConstants.GroupManagerEmail);

        var output = await client.GetPieChartReportsAsync(
            year: date.Year,
            month: date.Month,
            trackedTimeType: TrackedTimeType.Fact);

        // Assert
        output.Should().NotBeNull();
        output.PerProjects.Should().NotBeEmpty();
        output.PerClients.Should().NotBeEmpty();
        output.PerUsers.Should().NotBeEmpty();

        output.PerUsers.Where(user => user.IsActive).Should().HaveCount(1);
        output.PerUsers.First(user => user.IsActive).TotalTimeInMinutes.Should().Be(250);

        await client.DeleteTimesheetsUnderCeoRoleAsync(timesheets);
    }
}
