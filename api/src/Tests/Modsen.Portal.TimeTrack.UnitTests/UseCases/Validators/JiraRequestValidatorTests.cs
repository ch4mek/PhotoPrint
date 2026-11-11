using Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.UseCases.Validators;

public sealed class JiraRequestValidatorTests
{
    [Theory]
    [MemberData(nameof(GetInvalidCreateCommands))]
    public void CreateJiraSettingValidatorWhenCommandIsInvalidShouldReturnInvalidResult(
        string url,
        string pat,
        string jiraProjectTag,
        string? email,
        int projectId,
        int userId)
    {
        // Arrange
        var validator = new CreateJiraSettingRequest.Validator();
        var command = CreateValidCreateCommand(
            url,
            pat,
            jiraProjectTag,
            email,
            projectId,
            userId);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateJiraSettingValidatorWhenCommandIsValidShouldReturnValidResult()
    {
        // Arrange
        var validator = new CreateJiraSettingRequest.Validator();
        var command = CreateValidCreateCommand();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateJiraSettingValidatorWhenUrlDoesNotContainSchemeShouldReturnValidResult()
    {
        // Arrange
        var validator = new CreateJiraSettingRequest.Validator();
        var command = CreateValidCreateCommand(url: "jira.example.com:443");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetInvalidUpdateCommands))]
    public void UpdateJiraSettingValidatorWhenCommandIsInvalidShouldReturnInvalidResult(
        int id,
        string url,
        string pat,
        string jiraProjectTag,
        string? email,
        int projectId,
        int userId)
    {
        // Arrange
        var validator = new UpdateJiraSettingRequest.Validator();
        var command = CreateValidUpdateCommand(
            id,
            url,
            pat,
            jiraProjectTag,
            email,
            projectId,
            userId);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void UpdateJiraSettingValidatorWhenCommandIsValidShouldReturnValidResult()
    {
        // Arrange
        var validator = new UpdateJiraSettingRequest.Validator();
        var command = new UpdateJiraSettingRequest.Command(
            1,
            "https://jira.example.com",
            "pat",
            "MMS",
            "employee@company.com",
            1,
            1);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateJiraSettingValidatorWhenUrlDoesNotContainSchemeShouldReturnValidResult()
    {
        // Arrange
        var validator = new UpdateJiraSettingRequest.Validator();
        var command = CreateValidUpdateCommand(url: "jira.example.com:443");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GetJiraSettingByIdValidatorWhenIdIsInvalidShouldReturnInvalidResult(int id)
    {
        // Arrange
        var validator = new GetJiraSettingByIdRequest.Validator();

        // Act
        var result = validator.Validate(new GetJiraSettingByIdRequest.Query(id));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GetJiraSettingByIdValidatorWhenIdIsValidShouldReturnValidResult()
    {
        // Arrange
        var validator = new GetJiraSettingByIdRequest.Validator();

        // Act
        var result = validator.Validate(new GetJiraSettingByIdRequest.Query(1));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DeleteJiraSettingValidatorWhenIdIsInvalidShouldReturnInvalidResult(int id)
    {
        // Arrange
        var validator = new DeleteJiraSettingRequest.Validator();

        // Act
        var result = validator.Validate(new DeleteJiraSettingRequest.Command(id));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DeleteJiraSettingValidatorWhenIdIsValidShouldReturnValidResult()
    {
        // Arrange
        var validator = new DeleteJiraSettingRequest.Validator();

        // Act
        var result = validator.Validate(new DeleteJiraSettingRequest.Command(1));

        // Assert
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object?[]> GetInvalidCreateCommands()
    {
        yield return ["", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return ["ftp://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return ["https://jira.example.com", "", "MMS", "employee@gmail.com", 1, 1];
        yield return ["https://jira.example.com", "pat", "", "employee@gmail.com", 1, 1];
        yield return ["https://jira.example.com", "pat", "MMS", "employee@gmail.com", 0, 1];
        yield return ["https://jira.example.com", "pat", "MMS", "employee@gmail.com", -1, 1];
        yield return ["https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 0];
        yield return ["https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, -1];
    }

    public static IEnumerable<object?[]> GetInvalidUpdateCommands()
    {
        yield return [0, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return [-1, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return [1, "", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return [1, "ftp://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 1];
        yield return [1, "https://jira.example.com", "pat", "", "employee@gmail.com", 1, 1];
        yield return [1, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", 0, 1];
        yield return [1, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", -1, 1];
        yield return [1, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, 0];
        yield return [1, "https://jira.example.com", "pat", "MMS", "employee@gmail.com", 1, -1];
    }

    private static CreateJiraSettingRequest.Command CreateValidCreateCommand(
        string url = "https://jira.example.com",
        string pat = "pat",
        string jiraProjectTag = "MMS",
        string? email = "employee@company.com",
        int projectId = 1,
        int userId = 1)
    {
        return new CreateJiraSettingRequest.Command(
            url,
            pat,
            jiraProjectTag,
            email,
            projectId,
            userId);
    }

    private static UpdateJiraSettingRequest.Command CreateValidUpdateCommand(
        int id = 1,
        string url = "https://jira.example.com",
        string pat = "pat",
        string jiraProjectTag = "MMS",
        string? email = "employee@company.com",
        int projectId = 1,
        int userId = 1)
    {
        return new UpdateJiraSettingRequest.Command(
            id,
            url,
            pat,
            jiraProjectTag,
            email,
            projectId,
            userId);
    }
}
