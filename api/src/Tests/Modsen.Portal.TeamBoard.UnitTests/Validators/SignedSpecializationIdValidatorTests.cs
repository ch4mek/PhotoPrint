using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.UnitTests.Validators;

public class SignedSpecializationIdValidatorTests
{
    [Theory]
    [InlineData("Trainee")]
    [InlineData("Middle")]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateWhenSeniorityLevelIdIsNotSigned_False(string? seniorityLevelId)
    {
        // Act
        var result = SeniorityLevel.IsSeniorityLevelSigned(seniorityLevelId, checkForNoneSeniorityLevel: true);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenNoneSeniorityLevelIdIsPassedButNotAllowed_False()
    {
        // Act
        var result = SeniorityLevel.IsSeniorityLevelSigned("None", checkForNoneSeniorityLevel: false);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateWhenNoneSeniorityLevelIdIsPassedAndAllowed_True()
    {
        // Act
        var result = SeniorityLevel.IsSeniorityLevelSigned("None", checkForNoneSeniorityLevel: true);

        // Assert
        result.Should().BeTrue();
    }
}
