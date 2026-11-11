using System.Net.Http.Json;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;

internal static class HttpClientUserEducationExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<UserEducationDto> CreateUserEducationAsync(this HttpClient client,
        CreateUserEducationDto input)
    {
        return client.PostAsJsonAsync<CreateUserEducationDto, UserEducationDto>(
            "api/teamboard-users/educations", input, _cancellationToken);
    }

    public static Task<UserEducationDto> CreateUserEducationAsync(this HttpClient client,
        int userId,
        string? comment = null,
        string? educationalInstitution = null,
        string? faculty = null,
        int graduationYear = 2025,
        string? specialization = null,
        EducationLevel educationLevel = EducationLevel.Higher,
        StudyForm studyForm = StudyForm.FullTime)
    {
        var input = new CreateUserEducationDto
        {
            UserId = userId,
            Comment = comment.IsNullOrEmpty() ? Guid.CreateVersion7().ToString() : comment,
            EducationalInstitution = educationalInstitution.IsNullOrEmpty() ? Guid.CreateVersion7().ToString() : educationalInstitution!,
            EducationLevel = educationLevel,
            Faculty = faculty.IsNullOrEmpty() ? Guid.CreateVersion7().ToString() : faculty!,
            GraduationYear = graduationYear,
            Specialization = specialization.IsNullOrEmpty() ? Guid.CreateVersion7().ToString() : specialization!,
            StudyForm = studyForm
        };

        return client.PostAsJsonAsync<CreateUserEducationDto, UserEducationDto>(
            "api/teamboard-users/educations", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> CreateUserEducationAsHttpMessageAsync(this HttpClient client,
        CreateUserEducationDto input)
    {
        return client.PostAsJsonAsync("api/teamboard-users/educations", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<UserEducationDto> UpdateUserEducationAsync(this HttpClient client, int id,
        UpdateUserEducationDto input)
    {
        return client.PutAsJsonAsync<UpdateUserEducationDto, UserEducationDto>(
            $"api/teamboard-users/educations/{id}", input, _cancellationToken);
    }

    public static Task<HttpResponseMessage> UpdateUserEducationAsHttpMessageAsync(this HttpClient client, int id,
        UpdateUserEducationDto input)
    {
        return client.PutAsJsonAsync($"api/teamboard-users/educations/{id}", input, IntegrationTestsConstants.DefaultSerializerOptions, _cancellationToken);
    }

    public static Task<PageableListOfItems<OdpUserEducation>> GetUserEducationsWithSearchAsync(this HttpClient client,
        int userId,
        string filter = "",
        string orderBy = "",
        int top = 25,
        int skip = 0)
    {
        var filterSection = filter.IsNullOrEmpty() ? "" : $"&filter={filter}";
        var orderbySection = orderBy.IsNullOrEmpty() ? "" : $"&orderby={orderBy}";

        return client.GetFromJsonAsync<PageableListOfItems<OdpUserEducation>>(
            $"api/teamboard-users/{userId}/educations?top={top}&skip={skip}{filterSection}{orderbySection}",
            IntegrationTestsConstants.DefaultSerializerOptions,
            _cancellationToken);
    }
}
