using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills.Core;

public class HardSkillsExistBusinessRuleTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly IHardSkillsExistBusinessRule _hardSkillsExistBusinessRule;

    public HardSkillsExistBusinessRuleTests()
    {
        _hardSkillsExistBusinessRule = new HardSkillsExistBusinessRule(_teamBoardDbContextMock.Object);
    }

    [Theory]
    [InlineData("JuniorL1")]
    [InlineData("JuniorL2")]
    [InlineData("JuniorL3")]
    public async Task ValidateWhenTryingToCheckHardSkillsWithSignedSeniorityLevel_Exception(string hardSkillSeniorityLevelId)
    {
        var seniorityLevelsIds = new[] { "Junior", "JuniorL1", "JuniorL2", "JuniorL3" };

        _teamBoardDbContextMock
            .Setup(x => x.SeniorityLevels)
            .Returns(seniorityLevelsIds
                .Select(id => new SeniorityLevel
                {
                    Id = id,
                    Description = id,
                    ExternalName = id,
                    InternalName = id,
                    ShortName = id
                })
                .ToArray().BuildMockDbSet().Object
            );

        var action = () => _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(
            [new HardSkillSeniorityDto { Id = 117, SeniorityLevelId = hardSkillSeniorityLevelId }],
            It.IsAny<CancellationToken>());

        await action
            .Should()
            .ThrowExactlyAsync<InvalidDataAppException>("Hard skill seniority level with sign can't be used for hard skill");
    }

    [Theory]
    [InlineData("Junior")]
    [InlineData("Middle")]
    [InlineData("None")]
    public async Task ValidateWhenTryingToCheckSignedHardSkillsWithValidSeniorityLevel_Success(string hardSkillSeniorityLevelId)
    {
        var seniorityLevelsIds = new[] { "Junior", "JuniorL1", "JuniorL2", "JuniorL3", "Middle", "None" };

        _teamBoardDbContextMock
            .Setup(x => x.SeniorityLevels)
            .Returns(seniorityLevelsIds
                .Select(id => new SeniorityLevel
                {
                    Id = id,
                    Description = id,
                    ExternalName = id,
                    InternalName = id,
                    ShortName = id
                })
                .ToArray().BuildMockDbSet().Object
            );

        var action = () => _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(
            [new HardSkillSeniorityDto { Id = 117, SeniorityLevelId = hardSkillSeniorityLevelId }],
            It.IsAny<CancellationToken>());

        await action
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ValidateWhenTryingToCheckNotExistingHardSkills_Exception()
    {
        var seniorityLevelsIds = new[] { "Junior", "JuniorL1", "JuniorL2", "JuniorL3", "Middle", "None", "Senior" };

        _teamBoardDbContextMock
            .Setup(x => x.SeniorityLevels)
            .Returns(seniorityLevelsIds
                .Select(id => new SeniorityLevel
                {
                    Id = id,
                    Description = id,
                    ExternalName = id,
                    InternalName = id,
                    ShortName = id
                })
                .ToArray().BuildMockDbSet().Object
            );

        var action = () => _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(
            hardSkills: [
                new HardSkillSeniorityDto { SeniorityLevelId = "Middle" },
                new HardSkillSeniorityDto { SeniorityLevelId = "Tomato" },
                new HardSkillSeniorityDto { SeniorityLevelId = "SuperGoodGuy" },
                new HardSkillSeniorityDto { SeniorityLevelId = "NiceSkillsAwesomeBackground" },
            ],
            It.IsAny<CancellationToken>());

        await action
            .Should()
            .ThrowExactlyAsync<EntityNotFoundException>();
    }
}
