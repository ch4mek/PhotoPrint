using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills;

public class AreAllHardSkillsListedBusinessRuleTests
{
    private readonly Mock<ITeamBoardDbContext> _mockDbContext;
    private readonly AreAllHardSkillsListedBusinessRule _businessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public AreAllHardSkillsListedBusinessRuleTests()
    {
        _mockDbContext = new Mock<ITeamBoardDbContext>();
        _businessRule = new AreAllHardSkillsListedBusinessRule(_mockDbContext.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllHardSkillsAreListedShouldReturnTrue()
    {
        // Arrange
        var specializationTemplateId = 1;
        var hardSkills = new[]
        {
            new HardSkillSeniorityDto { Id = 1001, SeniorityLevelId = "F1" },
            new HardSkillSeniorityDto { Id = 1002, SeniorityLevelId = "F1" }
        };

        var specializationTemplate = new SpecializationTemplate
        {
            Id = specializationTemplateId,
            Name = "test",
            SpecializationTemplateHardSkills = new[]
            {
                new SpecializationTemplateHardSkill { HardSkillId = 1001 },
                new SpecializationTemplateHardSkill { HardSkillId = 1002 }
            }.ToList()
        };

        var specializationTemplates = new[] { specializationTemplate };
        _mockDbContext.Setup(db => db.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(specializationTemplateId, hardSkills, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSpecializationTemplateDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var specializationTemplateId = 999;
        var hardSkills = Array.Empty<HardSkillSeniorityDto>();

        var specializationTemplates = Array.Empty<SpecializationTemplate>();
        _mockDbContext.Setup(db => db.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        // Act
        var act = async () => await _businessRule.IsCorrespondsToBusiness(specializationTemplateId, hardSkills, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHardSkillsAreMissingShouldReturnFalse()
    {
        // Arrange
        var specializationTemplateId = 1;
        var hardSkills = new[]
        {
            new HardSkillSeniorityDto { Id = 1001, SeniorityLevelId = "F1" }
        };

        var specializationTemplate = new SpecializationTemplate
        {
            Id = specializationTemplateId,
            Name = "test",
            SpecializationTemplateHardSkills = new[]
            {
                new SpecializationTemplateHardSkill { HardSkillId = 1001 },
                new SpecializationTemplateHardSkill { HardSkillId = 1002 }
            }.ToList()
        };

        var specializationTemplates = new[] { specializationTemplate };
        _mockDbContext.Setup(db => db.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(specializationTemplateId, hardSkills, _cancellationToken);

        // Assert
        result.Should().BeFalse();
    }
}
