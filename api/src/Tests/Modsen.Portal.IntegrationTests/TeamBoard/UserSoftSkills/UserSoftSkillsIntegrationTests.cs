using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSoftSkills;

[Collection("GeneralCollection")]
public class UserSoftSkillsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserSoftSkillsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData("?top=10&skip=0&orderby=id")]
    [InlineData("?top=15&skip=0&filter=softSkillName eq 'Mentoring'")]
    public async Task GetUserSoftSkillsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetUsersSoftSkillsAsHttpMessage(_factory.CeoUserId, query);
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserSoftSkillsWhenRequestIsValidResponseWithSoftSkills()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var softSkillId = 2;
        var seniorityLevelId = "Senior";

        var updateUserSoftSkillInput = new CreateUserSoftSkillsSnapshotInput
        {
            SnapshotDate = DateOnly.FromDateTime(DateTime.UtcNow),
            UserSoftSkills = new List<UpdateUserSoftSkillInput>
            {
                new()
                {
                    SoftSkillId = softSkillId,
                    SeniorityLevelId = seniorityLevelId
                },
            }
        };

        await client.CreateUsersSoftSkills(_factory.EmployeeUserId, updateUserSoftSkillInput)
            .WithResponseType<int>();

        // Act
        var result = await client.GetUsersSoftSkills(_factory.EmployeeUserId,
            $"?top=15&skip=0&filter=id eq {softSkillId}");

        // Assert
        result.Items.Any(item => item.Id == softSkillId).Should().BeTrue();
    }

    [Fact]
    public async Task GetUserSoftSkillsWhenAuthAsEmployeeResponseWithForbidden()
    {
        // Arrange
        var userId = 4;

        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var response = await client.GetUsersSoftSkillsAsHttpMessage(userId, "?top=20&$skip=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateUserSoftSkillsWhenRequestIsValidReturnOkResult()
    {
        // Arrange

        var softSkillId = 2;
        var seniorityLevelId = "Senior";

        var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var updateUserSoftSkillInput = new CreateUserSoftSkillsSnapshotInput
        {
            SnapshotDate = DateOnly.FromDateTime(DateTime.UtcNow),
            UserSoftSkills = new List<UpdateUserSoftSkillInput>
            {
                new()
                {
                    SoftSkillId = softSkillId,
                    SeniorityLevelId = seniorityLevelId
                },
            }
        };

        //Act
        var response = await client.CreateUsersSoftSkills(_factory.HeadOfDepartmentUserId, updateUserSoftSkillInput)
            .WithResponseType<int>();

        var updateResponse = await client.UpdateUsersSoftSkills(response,
            new UpdateUserSoftSkillsSnapshotInput
            {
                UserSoftSkills = new List<UpdateUserSoftSkillInput>
                {
                    new()
                    {
                        SoftSkillId = softSkillId,
                        SeniorityLevelId = "Lead"
                    }
                }
            });

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
