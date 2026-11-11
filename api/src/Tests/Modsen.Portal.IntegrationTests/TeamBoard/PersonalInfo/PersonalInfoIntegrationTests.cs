using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TeamBoard.PersonalInfo;

[Collection("GeneralCollection")]
public class PersonalInfoIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public PersonalInfoIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserDetailsWhenUserCreatedShouldReturnUserDetails()
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
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate);

        // Act
        var result = await client.GetUserDetailsAsync(user!.Id).WithResponseType<UserDetailsOutput>();

        // Assert
        result.Plan.Should().Be(null);
    }

    [Fact]
    public async Task UpdateUserPlanWhenDataCorrectShouldReturnUpdatedplan()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var plan = "plan";
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
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate);

        // Act
        var result = await client.UpdateUserPlanAsync(user!.Id, plan).WithResponseType<UserPlanOutput>();

        // Assert
        result.Plan.Should().Be(plan);
    }

    [Fact]
    public async Task UpdateUserIsNeedProjectWhenDataCorrectShouldReturnUpdatedIsNeedproject()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var isNeedProject = true;
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
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var user = await client.CreateUser(userToCreate);

        // Act
        var result = await client.UpdateUserIsNeedProjectAsync(user!.Id, isNeedProject).WithResponseType<bool>();

        // Assert
        result.Should().Be(isNeedProject);
    }

    [Fact]
    public async Task GetPersonalInfoWhenTeamBoardUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = 9999;

        // Act
        var response = await client.GetPersonalInfo(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsEmployeeResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.HeadOfDepartmentUserId;

        // Act
        var response = await client.GetPersonalInfo(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsEmployeeResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId;

        // Act
        var response = await client.GetPersonalInfo(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsHeadOfDepartmentAndUserIsFromTheSameDepartmentResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.GetPersonalInfo(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsGroupManagerOfDepartmentAndUserIsFromTheSameResourceGroupResponseWithOkResult()
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
            DepartmentId = "Net",
            ResourceGroupId = 1,
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };
        var user = await client.CreateUser(userToCreate);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetPersonalInfo(user.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsSysAdminResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = 10;

        // Act
        var response = await client.GetPersonalInfo(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPersonalInfoWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var activeUserId = _factory.EmployeeUserId;

        // Act
        var response = await client.GetPersonalInfo(activeUserId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync(_cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var personalInfo = JsonSerializer.Deserialize<PersonalInfoDto>(content, options);

        personalInfo.Should().NotBeNull();
        personalInfo.Email.Should().Be("modsen.portal.test.employee@modsen-software.com");
    }

    [Fact]
    public async Task UpdatePersonalDataWhenTeamBoardUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var corporateEmail = $"{Guid.CreateVersion7().ToAlphabetic()}@gmail.com";
        var personalEmail = $"{Guid.CreateVersion7().ToAlphabetic()}@gmail.com";
        var personalEmailSecond = $"{Guid.CreateVersion7().ToAlphabetic()}@gmail.com";
        const string telegram = "@testtg";
        var birthDay = new DateOnly(2002, 2, 2);

        var userId = 999999;
        var body = new UpdatePersonalDataRequestDto(
            UserId: userId,
            FirstName: "test",
            LastName: "test",
            FirstNameRu: "Тест",
            LastNameRu: "Тест",
            PatronymicRu: "Тест",
            GroupId: null,
            DepartmentId: "Net",
            CorporateEmail: corporateEmail,
            PersonalEmail: personalEmail,
            PersonalEmailSecond: null,
            PersonalPhone: null,
            Telegram: telegram,
            LinkedIn: null,
            EnglishLevel: "B2",
            DateOfBirth: birthDay,
            Position: "Employee",
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null);

        // Act
        var response = await client.UpdatePersonalInfo(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePersonalDataWhenAuthAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var emptyString = "";

        var userId = _factory.EmployeeUserId!.Value;
        var body = new UpdatePersonalDataRequestDto(
            UserId: userId,
            FirstName: null,
            LastName: emptyString,
            FirstNameRu: emptyString,
            LastNameRu: emptyString,
            PatronymicRu: emptyString,
            GroupId: null,
            DepartmentId: null,
            CorporateEmail: null,
            PersonalEmail: null,
            PersonalEmailSecond: null,
            PersonalPhone: null,
            Telegram: emptyString,
            LinkedIn: null,
            EnglishLevel: emptyString,
            DateOfBirth: DateOnly.MinValue,
            Position: emptyString,
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null);

        // Act
        var response = await client.UpdatePersonalInfo(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task UpdatePersonalDataWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstNameq",
            LastName = "TestUserLastNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"modsen.portal.test.corporate@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var userId = createdUser!.Id;
        var corporateEmail = new string("modsen.portal.test.corporate@modsen-software.com");
        var personalEmail = new string("modsen.portal.test.personal@modsen-software.com");
        var personalEmailSecond = new string("new@modsen-software.com");
        var personalPhone = new string("+375291111111");
        var englishLevelId = new string("C1");
        var telegram = "@testTg";
        var birthDate = new DateOnly(2002, 2, 2);
        var position = "Employee";

        var body = new UpdatePersonalDataRequestDto(
            UserId: userId,
            FirstName: "test",
            LastName: "test",
            FirstNameRu: "ТестИмя",
            LastNameRu: "ТестФамилия",
            PatronymicRu: "ТестОтчество",
            GroupId: null,
            DepartmentId: "Net",
            CorporateEmail: corporateEmail,
            PersonalEmail: personalEmail,
            PersonalEmailSecond: personalEmailSecond,
            PersonalPhone: personalPhone,
            Telegram: telegram,
            LinkedIn: null,
            EnglishLevel: englishLevelId,
            DateOfBirth: birthDate,
            Position: position,
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null);

        // Act
        var response = await client.UpdatePersonalInfo(body);

        var content = await response.Content.ReadAsStringAsync(_cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var result = JsonSerializer.Deserialize<PersonalDataOutput>(content, options);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.PersonalEmail.Should().Be(personalEmail);
        result.PersonalPhone.Should().Be(personalPhone);
        result.EnglishLevelId.Should().Be(englishLevelId);
        result.DateOfBirth.Should().Be(birthDate);
        result.Position.Should().Be(position);
    }

    [Theory]
    [InlineData(null, "Тест", "Тест")]
    [InlineData("Тест", null, "Тест")]
    [InlineData("Тест", "Тест", null)]
    public async Task UpdatePersonalDataWhenRuFieldIsNullResponseWithBadRequestResult(
        string? firstNameRu, string? lastNameRu, string? patronymicRu)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var body = new UpdatePersonalDataRequestDto(
            UserId: userId,
            FirstName: "test",
            LastName: "test",
            FirstNameRu: firstNameRu,
            LastNameRu: lastNameRu,
            PatronymicRu: patronymicRu,
            GroupId: null,
            DepartmentId: "Net",
            CorporateEmail: null,
            PersonalEmail: null,
            PersonalEmailSecond: null,
            PersonalPhone: null,
            Telegram: null,
            LinkedIn: null,
            EnglishLevel: "B2",
            DateOfBirth: new DateOnly(2002, 2, 2),
            Position: "Employee",
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null);

        // Act
        var response = await client.UpdatePersonalInfo(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePersonalDataWhenMilitaryServiceStatusIsIntegerResponseWithBadRequestResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;
        var json = $$"""
            {
                "userId": {{userId}},
                "firstName": "test",
                "lastName": "test",
                "firstNameRu": "Тест",
                "lastNameRu": "Тест",
                "patronymicRu": "Тест",
                "departmentId": "Net",
                "englishLevel": "B2",
                "dateOfBirth": "2002-02-02",
                "position": "Employee",
                "militaryServiceStatus": 1
            }
            """;
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await client.PatchAsync("api/personal-info", content, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateCommentAsyncWhenTeamBoardUserDoesNotExistResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = 9999;
        var comment = "comment";

        var body = new UpdateTeamBoardUserCommentInput(userId, comment);

        // Act
        var response = await client.UpdatePersonalInfoComment(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCommentAsyncWhenAuthAsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;
        var comment = "comment";

        var body = new UpdateTeamBoardUserCommentInput(userId, comment);

        // Act
        var response = await client.UpdatePersonalInfoComment(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateCommentAsyncWhenRequestIsValidResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.CtoUserId!.Value;
        var comment = "comment";

        var body = new UpdateTeamBoardUserCommentInput(userId, comment);

        // Act
        var response = await client.UpdatePersonalInfoComment(body);
        var result = await response.Content.ReadFromJsonAsync<TeamBoardUserCommentOutput>(_cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        result.Comment.Should().NotBeNull();
        result.Comment.Should().Be(comment);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsCeoTryToGetInactiveEmployeeResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userEmail = Guid.CreateVersion7().ToAlphabetic();
        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstNameq",
            LastName = "TestUserLastNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{userEmail}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var createEmployeeInactiveResponse = await client.SetUserInactiveAsHttpMessage(createdUser!.Id);

        // Act
        var response = await client.GetPersonalInfo(createdUser.Id);

        // Assert
        createEmployeeInactiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsCeoTryToGetYourselfResponseOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var ceoId = _factory.CeoUserId;

        // Act
        var ceoResponse = await client.GetPersonalInfo(ceoId);

        // Assert
        ceoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsHeadOfDepartmentTryToGetInactiveUserResponseWithNotFoundResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userEmail = Guid.CreateVersion7().ToAlphabetic();
        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstNameq",
            LastName = "TestUserLastNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{userEmail}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var createEmployeeInactiveResponse = await client.SetUserInactiveAsHttpMessage(createdUser!.Id);

        // Act
        var response = await client.GetPersonalInfo(createdUser.Id);

        // Assert
        createEmployeeInactiveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetPersonalInfoWhenAuthAsGroupManagerOfDepartmentAndUserIsNotFromTheSameResourceGroupResponseWithOkResult()
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
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };
        var user = await client.CreateUser(userToCreate);

        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetPersonalInfo(user!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdatePersonalDataWhenExistsUserWithSamePersonalEmailResponseWithConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userToCreate = new CreateUserInput
        {
            FirstName = "TestUserFirstNameq",
            LastName = "TestUserLastNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"modsen.portal.test.corporate.email@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var userWithSameEmailInDiffCase = new CreateUserInput
        {
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"modsen.portal.test.same.email@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            PersonalEmail = "modsen.personal.NEW@modsen-software.com"
        };
        await client.CreateUser(userWithSameEmailInDiffCase);

        var userId = createdUser!.Id;
        var corporateEmail = new string("modsen.portal.test.corporate@modsen-software.com");
        var personalEmail = new string("modsen.portal.test.personal@modsen-software.com");
        var personalEmailSecond = new string("modsen.personal.new@modsen-software.com");
        var personalPhone = new string("+375291111111");
        var englishLevelId = new string("C1");
        var telegram = "@testTg";
        var birthDate = new DateOnly(2002, 2, 2);
        var position = "Employee";

        var body = new UpdatePersonalDataRequestDto(
            UserId: userId,
            FirstName: "test",
            LastName: "test",
            FirstNameRu: "ТестИмя",
            LastNameRu: "ТестФамилия",
            PatronymicRu: "ТестОтчество",
            GroupId: null,
            DepartmentId: "Net",
            CorporateEmail: corporateEmail,
            PersonalEmail: personalEmail,
            PersonalEmailSecond: personalEmailSecond,
            PersonalPhone: personalPhone,
            Telegram: telegram,
            LinkedIn: null,
            EnglishLevel: englishLevelId,
            DateOfBirth: birthDate,
            Position: position,
            MilitaryServiceStatus: null,
            IsGraduatedJobPlacement: null);

        // Act
        var response = await client.UpdatePersonalInfo(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
