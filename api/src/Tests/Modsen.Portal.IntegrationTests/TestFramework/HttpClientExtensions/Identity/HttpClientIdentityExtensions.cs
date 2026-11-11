using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Storage.Controllers.Output;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;

internal static class HttpClientIdentityExtensions
{
    private static readonly Faker _faker = new();
    //private static readonly JsonSerializerOptions _jsonSerializerOptions = IntegrationTestsConstants.DefaultSerializerOptions;
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static async Task<UserOutput?> CreateEmployee(this HttpClient client,
        int? resourceGroupId = null,
        string departmentId = "Net",
        string? email = null,
        string? firstName = null,
        string? firstNameRu = null,
        string? lastNameRu = null,
        string? patronymicRu = null,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.Employee,
            departmentId: departmentId,
            resourceGroupId: resourceGroupId,
            location: location);

        employeeToCreate.Email = email ?? employeeToCreate.Email;

        employeeToCreate.FirstName = firstName.IsNullOrEmpty() ? employeeToCreate.FirstName : firstName!;

        employeeToCreate.FirstNameRu = firstNameRu.IsNullOrEmpty() ? employeeToCreate.FirstNameRu : firstNameRu!;
        employeeToCreate.LastNameRu = lastNameRu.IsNullOrEmpty() ? employeeToCreate.LastNameRu : lastNameRu!;

        var createdEmployee = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            employeeToCreate, _cancellationToken);

        return createdEmployee;
    }

    public static async Task<UserOutput?> CreateIntern(
        this HttpClient client,
        int? resourceGroupId = null,
        string? personalEmail = null,
        string? firstName = null,
        string? lastName = null,
        string? firstNameRu = null,
        string? lastNameRu = null,
        string? patronymicRu = null,
        LocationType location = LocationType.Minsk)
    {
        var internToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.Intern,
            departmentId: "LD",
            resourceGroupId: resourceGroupId,
            location: location);

        internToCreate.Email = null;

        internToCreate.PersonalEmail = personalEmail ??
            _faker.Internet.Email(provider: "gmail.com", uniqueSuffix: Guid.CreateVersion7().ToAlphabetic());

        internToCreate.FirstName = firstName.IsNullOrEmpty() ? internToCreate.FirstName : firstName;
        internToCreate.LastName = lastName.IsNullOrEmpty() ? internToCreate.LastName : lastName;

        internToCreate.FirstNameRu = firstNameRu.IsNullOrEmpty() ? internToCreate.FirstNameRu : firstNameRu;
        internToCreate.LastNameRu = lastNameRu.IsNullOrEmpty() ? internToCreate.LastNameRu : lastNameRu;

        internToCreate.PatronymicRu = patronymicRu.IsNullOrEmpty() ? internToCreate.PatronymicRu : patronymicRu;

        var createdIntern = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            internToCreate, _cancellationToken);

        return createdIntern;
    }

    public static async Task<HttpResponseMessage> CreateEmployeeAsHttpMessage(this HttpClient client,
        int? resourceGroupId = null,
        string departmentId = "Net",
        string? email = null,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.Employee,
            departmentId: departmentId,
            resourceGroupId: resourceGroupId,
            location: location);

        employeeToCreate.Email = email ?? employeeToCreate.Email;

        return await client.PostAsJsonAsync("api/Users",
            employeeToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<UserOutput?> CreateGroupManager(this HttpClient client,
        int? resourceGroupId = null,
        string departmentId = "Net",
        string? email = null,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.GroupManager,
            departmentId: departmentId,
            resourceGroupId: resourceGroupId,
            location: location);

        employeeToCreate.Email = email ?? employeeToCreate.Email;

        return await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            employeeToCreate, _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateGroupManagerAsHttpMessage(this HttpClient client,
        int? resourceGroupId = null,
        string departmentId = "Net",
        string? email = null,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.GroupManager,
            departmentId: departmentId,
            resourceGroupId: resourceGroupId,
            location: location);

        employeeToCreate.Email = email ?? employeeToCreate.Email;

        return await client.PostAsJsonAsync("api/Users",
            employeeToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateCeoAsHttpMessage(this HttpClient client,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.Ceo,
            departmentId: null,
            resourceGroupId: null,
            location: location);

        return await client.PostAsJsonAsync("api/Users",
            employeeToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateCtoAsHttpMessage(this HttpClient client,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.Cto,
            departmentId: null,
            resourceGroupId: null,
            location: location);

        return await client.PostAsJsonAsync("api/Users",
            employeeToCreate, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<UserOutput?> CreateSysAdmin(this HttpClient client,
        string departmentId,
        LocationType location = LocationType.Minsk)
    {
        var sysAdminToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.SystemAdministrator,
            departmentId: departmentId,
            resourceGroupId: null,
            location: location);

        sysAdminToCreate.Email = $"{Guid.CreateVersion7().ToAlphabetic()}{EmailConstants.AdminEmail}";

        var createdSysAdmin = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            sysAdminToCreate, _cancellationToken);

        return createdSysAdmin;
    }

    private static CreateUserInput FillEmptyFieldsCreateUserInput(CreateUserInput input)
    {
        input.LastNameRu = string.IsNullOrWhiteSpace(input.LastNameRu) ? RandomStringGenerator.ToRussian() : input.LastNameRu;
        input.FirstNameRu = string.IsNullOrWhiteSpace(input.FirstNameRu) ? RandomStringGenerator.ToRussian() : input.FirstNameRu;
        input.PatronymicRu = string.IsNullOrWhiteSpace(input.PatronymicRu) ? RandomStringGenerator.ToRussian() : input.PatronymicRu;
        input.DateOfBirth = input.DateOfBirth == default ? new DateOnly(2000, 2, 2) : input.DateOfBirth;
        input.ContractDateFrom = input.ContractDateFrom == default ? DateOnly.FromDateTime(DateTime.Today).AddMonths(-6) : input.ContractDateFrom;
        input.ContractDateTo = input.ContractDateTo == default ? DateOnly.FromDateTime(DateTime.Today).AddMonths(6) : input.ContractDateTo;

        return input;
    }

    public static Task<UserOutput?> CreateUser(this HttpClient client, CreateUserInput input)
    {
        return client.CreateUser(input, IntegrationTestsConstants.DefaultSerializerOptions);
    }

    public static async Task<UserOutput?> CreateUser(this HttpClient client,
        CreateUserInput input, JsonSerializerOptions options)
    {
        input = FillEmptyFieldsCreateUserInput(input);
        var createdEmployee = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            input, options, _cancellationToken);

        return createdEmployee;
    }

    public static async Task<HttpResponseMessage> CreateUserAsHttpMessage(this HttpClient client,
        CreateUserInput input)
    {
        input = FillEmptyFieldsCreateUserInput(input);
        return await client.PostAsJsonAsync("api/Users",
            input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);

    }

    private static CreateUserInput GenerateCreateUserInput(string position,
        string? departmentId,
        int? resourceGroupId,
        LocationType location)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var createInput = new CreateUserInput
        {
            Position = position,
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = RandomStringGenerator.ToRussian(),
            LastNameRu = RandomStringGenerator.ToRussian(),
            PatronymicRu = RandomStringGenerator.ToRussian(),
            Color = _faker.Internet.Color(),
            Email = _faker.Internet.Email(provider: "modsen-software.com", uniqueSuffix: Guid.CreateVersion7().ToAlphabetic()),
            CountryId = 1,
            Location = location,
            DepartmentId = departmentId,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
            ResourceGroupId = resourceGroupId,
            ContractDateFrom = today.AddMonths(-6),
            ContractDateTo = today.AddMonths(6),
            CurrentLocation = null,
            DateOfBirth = today.AddYears(-20),
            HireDate = today.AddMonths(-6),
            MilitaryServiceStatus = MilitaryServiceStatus.Unfit,
            IsGraduateJobPlacement = null,
            PersonalEmail = null,
            PersonalEmailSecond = null,
            PersonalPhone = null,
            Telegram = null,
        };

        return createInput;
    }

    public static class RandomStringGenerator
    {
        private static readonly Random _random = new();

        public static string ToRussian()
        {
            const string russianAlphabet = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
            var length = _random.Next(2, 20);
            var result = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                result.Append(russianAlphabet[_random.Next(russianAlphabet.Length)]);
            }

            return result.ToString();
        }
    }

    public static async Task<UserOutput?> CreateHeadOfDepartment(this HttpClient client,
        string departmentId,
        LocationType location = LocationType.Minsk)
    {
        var employeeToCreate = GenerateCreateUserInput(
            position: PortalRoleNames.HeadOfDepartment,
            departmentId: departmentId,
            resourceGroupId: null,
            location: location);

        var createdEmployee = await client.PostAsJsonAsync<CreateUserInput, UserOutput>("api/Users",
            employeeToCreate, _cancellationToken);

        return createdEmployee;
    }

    public static Task<UserOutput> UpdateUserAsync(this HttpClient client,
        UpdateUserInput input)
    {
        return client.PutAsJsonAsync<UpdateUserInput, UserOutput>(
            "api/Users",
            input,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateUserAsHttpMessageAsync(this HttpClient client,
        UpdateUserInput input, JsonSerializerOptions jsonOptions)
    {
        return client.PutAsJsonAsync(
            "api/Users",
            input,
            jsonOptions,
            _cancellationToken);
    }

    public static Task<UserSignatureOutput> GetUserSignatureAsync(this HttpClient client)
    {
        return client.GetFromJsonAsync<UserSignatureOutput>(
            "api/Users/current/signature",
            _cancellationToken);
    }

    public static Task<UserSignatureOutput> UpdateUserSignatureAsync(this HttpClient client,
        string fullName,
        string position,
        string? phone)
    {
        var updateSignatureRequest = new UpdateUserSignatureInput
        {
            FullName = fullName,
            Phone = phone,
            Position = position
        };

        return client.PutAsJsonAsync<UpdateUserSignatureInput, UserSignatureOutput>(
            "api/Users/current/signature",
            updateSignatureRequest,
            _cancellationToken);
    }

    public static Task<UserSignatureOutput> UpdateUserSignatureAsync(this HttpClient client,
        UpdateUserSignatureInput updateSignatureRequest)
    {

        return client.PutAsJsonAsync<UpdateUserSignatureInput, UserSignatureOutput>(
            "api/Users/current/signature",
            updateSignatureRequest,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateUserSignatureAsHttpMessageAsync(this HttpClient client,
        string fullName,
        string position,
        string? phone)
    {
        var updateSignatureRequest = new UpdateUserSignatureInput
        {
            FullName = fullName,
            Phone = phone,
            Position = position
        };

        return client.PutAsJsonAsync(
            new Uri("api/Users/current/signature", UriKind.Relative),
            updateSignatureRequest,
            _cancellationToken);
    }

    public static Task<CurrentUserOutput> UpdateCurrentUserAsync(this HttpClient client,
        string? departmentId = "Net",
        string timeZone = "BY",
        LocationType location = LocationType.Minsk,
        string? avatarColor = null,
        string? avatarFileId = null)
    {
        var updateCurrentUserInput = new UpdateCurrentUserInput
        {
            AvatarColor = avatarColor,
            AvatarFileId = avatarFileId,
            Location = location,
            TimeZone = timeZone,
            DepartmentId = departmentId
        };

        return client.PutAsJsonAsync<UpdateCurrentUserInput, CurrentUserOutput>(
            "/api/Users/current",
            updateCurrentUserInput,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateCurrentUserAsHttpMessageAsync(this HttpClient client,
        string? departmentId = "Net",
        string timeZone = "BY",
        LocationType location = LocationType.Minsk,
        string? avatarColor = null,
        string? avatarFileId = null)
    {
        var updateCurrentUserInput = new UpdateCurrentUserInput
        {
            AvatarFileId = avatarFileId,
            AvatarColor = avatarColor,
            Location = location,
            TimeZone = timeZone,
            DepartmentId = departmentId
        };

        return client.PutAsJsonAsync(
            "/api/Users/current",
            updateCurrentUserInput,
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateCurrentUserAsHttpMessageAsync(this HttpClient client,
        UpdateCurrentUserInput input)
    {
        return client.PutAsJsonAsync("/api/Users/current", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static async Task<UserOutput> SetUserActive(this HttpClient client,
        int userId)
    {
        var userStatus = new UserStatusInput { IsActive = true };

        return await client.PutAsJsonAsync<UserStatusInput, UserOutput>(
            $"api/Users/{userId}/IsActive",
            userStatus,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> SetUserActiveAsHttpMessage(this HttpClient client,
        int userId)
    {
        var userStatus = new UserStatusInput { IsActive = true };

        return await client.PutAsJsonAsync(
            $"api/Users/{userId}/IsActive",
            userStatus,
            _cancellationToken);
    }

    public static async Task<UserOutput> SetUserInactive(this HttpClient client,
        int userId)
    {
        var userStatus = new UserStatusInput { IsActive = false };

        return await client.PutAsJsonAsync<UserStatusInput, UserOutput>(
            $"api/Users/{userId}/IsActive",
            userStatus,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> SetUserInactiveAsHttpMessage(this HttpClient client,
        int userId)
    {
        var userStatus = new UserStatusInput { IsActive = false };

        return await client.PutAsJsonAsync(
            $"api/Users/{userId}/IsActive",
            userStatus,
            _cancellationToken);
    }

    public static async Task<List<UserLookupOutput>> GetUsersLookup(this HttpClient client,
        string? keyWord = null,
        string[]? roles = null,
        bool? isActive = null)
    {
        var rolesSection = "";
        if (roles is not null)
        {
            var builder = new StringBuilder();
            foreach (var roleName in roles)
            {
                builder.Append(CultureInfo.InvariantCulture, $"roles={roleName}&");
            }

            rolesSection = builder.ToString();
        }

        var isActiveSection = isActive switch
        {
            null => "",
            _ => $"isActive={isActive.ToString()}&"
        };

        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}";

        return await  client.GetFromJsonAsync<List<UserLookupOutput>>(
            $"api/Users/lookup?{rolesSection}{isActiveSection}{keyWordSection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetUsersLookupAsHttpMessage(this HttpClient client,
        string[]? roles = null,
        bool? isActive = null)
    {
        var rolesSection = "";
        if (roles is not null)
        {
            var builder = new StringBuilder();
            foreach (var roleName in roles)
            {
                builder.Append(CultureInfo.InvariantCulture, $"roles={roleName}&");
            }

            rolesSection = builder.ToString();
        }

        var isActiveSection = isActive switch
        {
            null => "",
            _ => isActive.ToString()
        };

        return await client.GetAsync(
            $"api/Users/lookup?{rolesSection}{isActiveSection}",
            _cancellationToken);
    }

    public static async Task<UserOutput> GetCurrentUser(this HttpClient client)
    {
        return await client.GetFromJsonAsync<UserOutput>(
            "/api/Users/current",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<List<UserLookupWithRussianNamesShortOutput>> GetUserTabLookup(this HttpClient client,
    string? keyWord = null,
    string[]? roles = null,
    bool? isActive = null)
    {
        var rolesSection = "";
        if (roles is not null)
        {
            var builder = new StringBuilder();
            foreach (var roleName in roles)
            {
                builder.Append(CultureInfo.InvariantCulture, $"roles={roleName}&");
            }

            rolesSection = builder.ToString();
        }

        var isActiveSection = isActive switch
        {
            null => "",
            _ => $"isActive={isActive.ToString()}&"
        };

        var keyWordSection = keyWord.IsNullOrEmpty() ? "" : $"keyWord={keyWord}";

        return await client.GetFromJsonAsync<List<UserLookupWithRussianNamesShortOutput>>(
            $"api/Users/tab/lookup?{rolesSection}{isActiveSection}{keyWordSection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetUserTabLookupAsHttpMessage(this HttpClient client,
        string[]? roles = null,
        bool? isActive = null)
    {
        var rolesSection = "";
        if (roles is not null)
        {
            var builder = new StringBuilder();
            foreach (var roleName in roles)
            {
                builder.Append(CultureInfo.InvariantCulture, $"roles={roleName}&");
            }

            rolesSection = builder.ToString();
        }

        var isActiveSection = isActive switch
        {
            null => "",
            _ => isActive.ToString()
        };

        return await client.GetAsync(
            $"api/Users/tab/lookup?{rolesSection}{isActiveSection}",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetCurrentUserAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync("api/Users/current", _cancellationToken);
    }

    public static Task<PageableListOfItems<UserOutput>> GetUsersWithSearch(this HttpClient client,
        string filter = "",
        string orderBy = "",
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"&filter={filter}";
        var orderbySection = orderBy.IsNullOrEmpty() ? "" : $"&orderby={orderBy}";

        return client.GetFromJsonAsync<PageableListOfItems<UserOutput>>(
            $"api/Users/search?top={top}&skip={skip}{filterSection}{orderbySection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetUsersWithSearchAsHttpMessage(this HttpClient client,
        string filter = "",
        string orderBy = "",
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"&filter={filter}";
        var orderbySection = orderBy.IsNullOrEmpty() ? "" : $"&orderby={orderBy}";

        return client.GetAsync(
                $"api/Users/search?top={top}&skip={skip}{filterSection}{orderbySection}",
                _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetUsersWithRawSearchAsHttpMessage(this HttpClient client,
        string query)
    {
        return client.GetAsync(
            $"api/Users/search{query}",
            _cancellationToken);
    }

    public static async Task<PageableListOfItems<UserOutput>> GetUsersWithSearchAsPostMethod(this HttpClient client,
        string query,
        int top = 25,
        int skip = 0)
    {
        using var content = new StringContent(query, Encoding.UTF8, "text/plain");
        var response = await client.PostAsync(
            $"api/Users/search/$query?top={top}&skip={skip}",
            content,
            _cancellationToken);

        return await response.Content.ReadFromJsonAsync<PageableListOfItems<UserOutput>>(
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<UserOutput?> GetUserAsync(
        this HttpClient httpClient,
        int id)
    {
        return httpClient.GetFromJsonAsync<UserOutput>(
            $"api/Users/{id}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> GetUserAsHttpMessage(this HttpClient httpClient,
        int id)
    {
        return httpClient.GetAsync(
            $"api/Users/{id}",
            _cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteUserAsync(this HttpClient httpClient,
        int id)
    {
        return httpClient.DeleteAsync(new Uri($"api/Users/{id}", UriKind.Relative), _cancellationToken);
    }

    public static async Task<HttpResponseMessage> LogoutAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync("api/account/logout", _cancellationToken);
    }

    public static async Task<ApplicationFileOutput?> CreateApplicationFile(this HttpClient client,
        MultipartFormDataContent form, ApplicationFilePurpose? purpose = null)
    {
        var url = "/api/ApplicationFile";

        if (purpose is not null)
        {
            url += $"?purpose={purpose}";
        }

        var postResponse = await client.PostAsync(url, form, _cancellationToken);

        return await postResponse.Content.ReadFromJsonAsync<ApplicationFileOutput>(_cancellationToken);
    }

    public static async Task<HttpResponseMessage> CreateApplicationFileAsHttpMessage(this HttpClient client,
        MultipartFormDataContent form, ApplicationFilePurpose? purpose = null)
    {
        var url = "/api/ApplicationFile";

        if (purpose is not null)
        {
            url += $"?purpose={purpose}";
        }

        return await client.PostAsync(
            url, form, _cancellationToken);
    }

    public static async Task<byte[]> GetApplicationFile(this HttpClient client,
        string id, ApplicationFileSize? size = null)
    {
        var url = $"/api/ApplicationFile/{id}";

        if (size is not null)
        {
            url += $"?size={size}";
        }

        var response = await client.GetAsync(url, _cancellationToken);

        return await response.Content.ReadAsByteArrayAsync(_cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetApplicationFileAsHttpMessage(this HttpClient client,
        string id, ApplicationFileSize? size = null)
    {
        var url = $"/api/ApplicationFile/{id}";

        if (size is not null)
        {
            url += $"?size={size}";
        }

        return await client.GetAsync(url, _cancellationToken);
    }

    public static async Task<UserSignatureOutput> GetCurrentUserSignature(this HttpClient client)
    {
        return await client.GetFromJsonAsync<UserSignatureOutput>(
            "api/Users/current/signature",
            _cancellationToken);
    }

    public static async Task<HttpResponseMessage> GetCurrentUserSignatureAsHttpMessage(this HttpClient client)
    {
        return await client.GetAsync(
            "api/Users/current/signature",
            _cancellationToken);
    }

    public static async Task<RedirectRuleDto?> CreateRedirectRule(this HttpClient client,
        string redirectTo,
        string? redirectKey = null)
    {
        var request = new CreateRedirectRuleInput
        {
            RedirectTo = redirectTo,
            RedirectKey = redirectKey
        };

        return await client.PostAsJsonAsync<CreateRedirectRuleInput, RedirectRuleDto>(
            "api/redirect-rules", request, _cancellationToken);
    }

    public static async Task<RedirectRuleDto?> GetRedirectRule(this HttpClient client,
        string redirectKey)
    {
        return await client.GetFromJsonAsync<RedirectRuleDto>(
            $"api/redirect-rules/{redirectKey}", _cancellationToken);
    }
}
