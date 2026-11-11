using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.SofSkills.DeleteSoftSkill;

public class DeleteSoftSkillCommandHandlerTests
{
    private readonly Mock<IGetSoftSkillDbQuery> _getSoftSkillDbQueryMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly DeleteSoftSkillRequest _handler;

    public DeleteSoftSkillCommandHandlerTests()
    {
        _handler = new DeleteSoftSkillRequest(_getSoftSkillDbQueryMock.Object,
            _teamboardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenSoftSkillDoesNotExistShouldThrowNotFoundException()
    {
        // Arrange
        var skillId = 1;
        var command = new DeleteSoftSkillRequest.Command(skillId);

        _getSoftSkillDbQueryMock.Setup(x => x.ExecuteAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SoftSkill?)null);

        // Act
        var result = async () => await _handler.Handle(command, _token);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenSoftSkillExistShouldDeleteSoftSkill()
    {
        // Arrange
        var skillId = 1;
        var skillName = "Responsibility";
        var command = new DeleteSoftSkillRequest.Command(skillId);
        var softSkill = new SoftSkill { IsActive = true, Name = skillName, Id = skillId };

        _getSoftSkillDbQueryMock.Setup(x => x.ExecuteAsync(skillId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(softSkill);

        _teamboardDbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _teamboardDbContextMock
            .Verify(x => x.SaveChangesAsync(_token), Times.Once());

        softSkill.IsActive.Should().BeFalse();

        result.Name.Should().BeEquivalentTo(softSkill.Name);
        result.Id.Should().Be(softSkill.Id);
    }
}
