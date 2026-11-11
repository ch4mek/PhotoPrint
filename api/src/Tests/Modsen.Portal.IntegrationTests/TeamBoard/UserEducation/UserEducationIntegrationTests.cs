using System.Text.Json;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserEducation;

[Collection("GeneralCollection")]
public class UserEducationIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserEducationIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [MemberData(nameof(GetValidUserEducationParams))]
    public async Task CreateUserEducationWhenRequestValidReturnSuccessStatusCode(CreateUserEducationDto input)
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        //Act
        var response = await client.CreateUserEducationAsHttpMessageAsync(input);

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidUserEducationParams))]
    public async Task CreateUserEducationWhenRequestInvalidNotReturnSuccessStatusCode(CreateUserEducationDto input)
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        //Act
        var response = await client.CreateUserEducationAsHttpMessageAsync(input);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUserEducationWhenRequestValidReturnCreatedEducationDto()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var input = new CreateUserEducationDto
        {
            UserId = userId,
            Comment = "Valid Comment",
            EducationalInstitution = "Bsuir",
            EducationLevel = EducationLevel.Higher,
            Faculty = "Some faculty",
            GraduationYear = DateTime.Today.Year,
            Specialization = "Valid Specialization",
            StudyForm = StudyForm.FullTime
        };

        //Act
        var result = await client.CreateUserEducationAsync(input);

        //Assert
        result.Comment.Should().Be(input.Comment);
        result.EducationalInstitution.Should().Be(input.EducationalInstitution);
        result.EducationLevel.Should().Be(input.EducationLevel);
        result.Faculty.Should().Be(input.Faculty);
        result.GraduationYear.Should().Be(input.GraduationYear);
        result.Specialization.Should().Be(input.Specialization);
        result.StudyForm.Should().Be(input.StudyForm);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateUserEducationWhenRequestValidReturnUpdatedEducationDto()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var targetUserId = 10;
        var createInput = new CreateUserEducationDto
        {
            UserId = targetUserId,
            Comment = "Valid Comment",
            EducationalInstitution = "Bsuir",
            EducationLevel = EducationLevel.Higher,
            Faculty = "Some faculty",
            GraduationYear = DateTime.Today.Year,
            Specialization = "Valid Specialization",
            StudyForm = StudyForm.FullTime
        };

        var educationId = (await client.CreateUserEducationAsync(createInput)).Id;

        var input = new UpdateUserEducationDto
        {
            Comment = Guid.CreateVersion7().ToAlphabetic(),
            EducationalInstitution = Guid.CreateVersion7().ToAlphabetic(),
            EducationLevel = EducationLevel.Postgraduate,
            Faculty = Guid.CreateVersion7().ToAlphabetic(),
            GraduationYear = 2012,
            Specialization = Guid.CreateVersion7().ToAlphabetic(),
            StudyForm = StudyForm.Correspodence
        };

        //Act
        var result = await client.UpdateUserEducationAsync(id: educationId, input: input);

        //Assert
        result.Comment.Should().Be(input.Comment);
        result.EducationalInstitution.Should().Be(input.EducationalInstitution);
        result.EducationLevel.Should().Be(input.EducationLevel);
        result.Faculty.Should().Be(input.Faculty);
        result.GraduationYear.Should().Be(input.GraduationYear);
        result.Specialization.Should().Be(input.Specialization);
        result.StudyForm.Should().Be(input.StudyForm);
        result.UserId.Should().Be(targetUserId);
    }

    [Fact]
    public async Task GetPageableUserEducationsAsyncWhenRequestFromEmployeeReturnWithoutComment()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var employee = await client.CreateEmployee();
        var employeeId = employee!.Id;
        var employeeEmail = employee.Email;

        await client.CreateUserEducationAsync(userId: employeeId, comment: "Some valid Comment");

        await client.AuthAs(employeeEmail);

        //Act
        var result = await client.GetUserEducationsWithSearchAsync(userId: employeeId);

        //Assert
        result.Items.First().Comment.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetPageableUserEducationsAsyncWhenRequestFromHeadReturnWithComment()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var headId = _factory.HeadOfDepartmentUserId!.Value;

        const string comment = "Valid comment";

        await client.CreateUserEducationAsync(userId: headId, comment: comment);

        await client.AuthAsHeadOfDepartment();

        //Act
        var result = await client.GetUserEducationsWithSearchAsync(userId: headId);

        //Assert
        result.Items.First().Comment.Should().Be(comment);
    }

    public static IEnumerable<object[]> GetValidUserEducationParams()
    {
        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = "Valid Comment",
                EducationalInstitution = "Bsuir",
                EducationLevel = EducationLevel.Higher,
                Faculty = "Some faculty",
                GraduationYear = DateTime.Today.Year,
                Specialization = "Valid Specialization",
                StudyForm = StudyForm.FullTime
            }
        ];

        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = new string('a', 1000),
                EducationalInstitution = new string('a', 250),
                EducationLevel = EducationLevel.Higher,
                Faculty = new string('a', 250),
                GraduationYear = DateTime.Today.Year,
                Specialization = new string('a', 250),
                StudyForm = StudyForm.FullTime
            }
        ];

        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = new string('a', 2),
                EducationalInstitution = new string('a', 2),
                EducationLevel = EducationLevel.Higher,
                Faculty = new string('a', 2),
                GraduationYear = DateTime.Today.Year,
                Specialization = new string('a', 2),
                StudyForm = StudyForm.FullTime
            }
        ];
    }

    public static IEnumerable<object[]> GetInvalidUserEducationParams()
    {
        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 99999,
                Comment = "Valid Comment",
                EducationalInstitution = "Bsuir",
                EducationLevel = EducationLevel.Higher,
                Faculty = "Some faculty",
                GraduationYear = DateTime.Today.Year,
                Specialization = "Valid Specialization",
                StudyForm = StudyForm.FullTime
            }
        ];

        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = new string('a', 1001),
                EducationalInstitution = new string('a', 251),
                EducationLevel = EducationLevel.Higher,
                Faculty = new string('a', 251),
                GraduationYear = DateTime.Today.Year,
                Specialization = new string('a', 251),
                StudyForm = StudyForm.FullTime
            }
        ];

        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = new string('a', 2),
                EducationalInstitution = new string('a', 2),
                EducationLevel = EducationLevel.Higher,
                Faculty = new string('a', 2),
                GraduationYear = 10000,
                Specialization = new string('a', 2),
                StudyForm = StudyForm.FullTime
            }
        ];

        yield return
        [
            new CreateUserEducationDto
            {
                UserId = 1,
                Comment = new string('a', 1),
                EducationalInstitution = new string('a', 1),
                EducationLevel = EducationLevel.Higher,
                Faculty = new string('a', 1),
                GraduationYear = 2025,
                Specialization = new string('a', 1),
                StudyForm = StudyForm.FullTime
            }
        ];
    }
}
