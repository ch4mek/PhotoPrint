using System.Net;
using System.Text.Json;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Cv;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Cv.Cvs;

[Collection("CvCollection")]
public class CvsControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CvsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetPageableCvsWhenUserHasManageCvsClaimResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetPageableCvsAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPageableCvsWhenUserHasNoManageCvsClaimResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetPageableCvsAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPageableCvsWhenUserNotAuthenticatedResponseWithUnauthorizedResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetPageableCvsAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPageableCvsWhenCvsExistResponseWithPagedCvs()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var createCvInput = CreateValidCvInput(_factory.CtoUserId!.Value);
        var createdCv = await client.CreateCv(createCvInput, _jsonOptions);

        // Act
        var result = await client.GetPageableCvs(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0",
            ["$filter"] = $"Id eq {createdCv.Id}"
        },
            _jsonOptions);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().ContainSingle(x => x.Id == createdCv.Id && x.CvName == createdCv.CvName);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task GetCvsLookupWhenUserHasManageCvsClaimResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetCvsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCvsLookupWhenUserHasNoManageCvsClaimResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetCvsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCvsLookupWhenUserNotAuthenticatedResponseWithUnauthorizedResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetCvsLookupAsHttpMessage();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCvsLookupWhenCvsExistResponseWithCvsLookup()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var createCvInput = CreateValidCvInput(_factory.CtoUserId!.Value);
        var createdCv = await client.CreateCv(createCvInput, _jsonOptions);

        // Act
        var result = await client.GetCvsLookup(_jsonOptions);

        // Assert
        result.Should().ContainSingle(x => x.Id == createdCv.Id && x.CvName == createdCv.CvName);
    }

    [Fact]
    public async Task CreateCvWhenRequestValidResponseWithCreatedCv()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var input = CreateValidCvInput(_factory.CtoUserId!.Value);

        // Act
        var cvDto = await client.CreateCv(input, _jsonOptions);

        // Assert
        cvDto.CvName.Should().Be(input.CvName);
        cvDto.UserId.Should().Be(input.UserId);
        cvDto.WorkExperiences.Should().HaveCount(1);
        cvDto.WorkExperiences.Should().ContainSingle(x => x.Specialization == input.WorkExperiences[0].Specialization);
        cvDto.Languages.Should().HaveCount(1);
        cvDto.Languages.Should().ContainSingle(x => x.Name == input.Languages[0].Name);
        cvDto.ProjectExperiences.Should().HaveCount(1);
        cvDto.ProjectExperiences.Should().ContainSingle(x => x.Position == input.ProjectExperiences[0].Position);
        cvDto.SkillGroups.Should().HaveCount(1);
        cvDto.SkillGroups.Should().ContainSingle(x => x.Name == input.SkillGroups[0].Name);
        cvDto.SkillGroups[0].Skills.Should().HaveCount(1);
        cvDto.SkillGroups[0].Skills.Should().ContainSingle(x => x.Name == input.SkillGroups[0].Skills[0].Name);
    }

    [Fact]
    public async Task CreateCvWhenCvNameExistsResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var input = CreateValidCvInput(_factory.CtoUserId!.Value);
        await client.CreateCv(input, _jsonOptions);

        // Act
        var response = await client.CreateCvAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCvWhenUserHasNoManageCvsClaimResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var input = CreateValidCvInput(_factory.EmployeeUserId!.Value);

        // Act
        var response = await client.CreateCvAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateCvWhenUserNotAuthenticatedResponseWithUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var input = CreateValidCvInput(_factory.CtoUserId!.Value);

        // Act
        var response = await client.CreateCvAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteCvWhenCvsExistResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();
        var createCvInput = CreateValidCvInput(_factory.CtoUserId!.Value);
        var createdCv = await client.CreateCv(createCvInput, _jsonOptions);

        // Act
        var result = await client.DeleteCv(createdCv.Id);

        // Assert
        var getCvResult = await client.GetCv(createdCv.Id);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        getCvResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private CreateCvDto CreateValidCvInput(int userId)
    {
        return new CreateCvDto
        {
            CvName = "NewCV_" + Guid.CreateVersion7(),
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            UserProjectPosition = "Developer",
            UserLocation = "Minsk",
            UserEducation = "BSU",
            UserSkills = "C#, .NET",
            UserAbout = "Experienced developer",
            Website = "https://modsen-software.com",
            IsCorporateDesign = true,
            WorkExperiences =
            [
                new() { Specialization = "Software Engineer", ExperienceInYears = 5 }
            ],
            Languages =
            [
                new() { Name = "English", Level = Core.Cv.CvLanguageLevel.C2 }
            ],
            ProjectExperiences =
            [
                new()
                {
                    Position = "Lead Developer",
                    TeamSize = "12",
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    IsCurrent = true
                }
            ],
            SkillGroups =
            [
                new()
                {
                    Name = "Programming",
                    NameRu = "Программирование",
                    IsCustom = false,
                    Skills =
                    [
                        new() { Name = "Java", Level = Core.Cv.CvSkillLevel.Professional, ExperienceInYears = 5 }
                    ]
                }
            ]
        };
    }
}
