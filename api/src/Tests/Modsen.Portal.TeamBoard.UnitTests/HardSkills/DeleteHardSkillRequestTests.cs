using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills;

public class DeleteHardSkillRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly DeleteHardSkillRequest _handler;

    public DeleteHardSkillRequestTests()
    {
        _handler = new DeleteHardSkillRequest(_dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenHardSkillCanNotBeDeletedShouldThrowException()
    {
        // Arrange
        var hardSkillId = 1;
        var hardSkillName = "Windows";
        var hardSkillGroupId = "OS";

        var command = new DeleteHardSkillRequest.Command(hardSkillId);

        _dbContextMock.Setup(x => x.UserSpecializations)
            .Returns(new List<DataAccess.Npgsql.DatabaseEntities.UserSpecialization>()
            {
                new()
                {
                    SpecializationTemplate = new SpecializationTemplate()
                    {
                        Id = 1,
                        Name = "test",
                        SpecializationTemplateHardSkills =
                        [
                            new()
                            {
                                HardSkill =
                                    new HardSkill { IsActive = true, Name = "test", HardSkillGroupId = "1" },
                                HardSkillId = hardSkillId
                            }
                        ]
                    }
                }
            }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.HardSkills.FindAsync(
                It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new HardSkill
                {
                    Id = hardSkillId,
                    Name = hardSkillName,
                    HardSkillGroupId = hardSkillGroupId,
                    IsActive = true
                });


        // Act
        var result = async () => await _handler.Handle(command, _token);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>(
            "You cannot leave a specialization that is already assigned to users without hard skills");
    }
}
