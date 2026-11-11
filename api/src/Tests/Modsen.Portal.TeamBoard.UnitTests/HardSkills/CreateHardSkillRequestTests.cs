using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills;

public class CreateHardSkillRequestTests
{
    private readonly Mock<IHardSkillUpdateValidator> _hardSkillUpdateValidatorMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly CreateHardSkillRequest _handler;

    public CreateHardSkillRequestTests()
    {
        _handler = new CreateHardSkillRequest
        (
            _authenticatedUserContext.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object,
            _hardSkillUpdateValidatorMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenHardSkillWithTheSameNameAlreadyExistsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new CreateHardSkillRequest.Command("xUnit", "TEST");

        _hardSkillUpdateValidatorMock
            .Setup(x => x.ValidateAsync(null, command.Name, command.HardSkillGroupId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataAppException("Hard skill with the same name already exists"));

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Hard skill with the same name already exists");
    }

    [Fact]
    public async Task HandleWhenHardSkillWithTheSameNameDoesNotExistShouldCreateNewHardSkill()
    {
        // Arrange
        var skillName = "Indexes";
        var groupId = "DB";
        var command = new CreateHardSkillRequest.Command(skillName, groupId);

        var createdSkill = new HardSkillDto { Name = skillName, HardSkillGroupId = groupId };

        _hardSkillUpdateValidatorMock
            .Setup(x => x.ValidateAsync(null, command.Name, command.HardSkillGroupId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _authenticatedUserContext.Setup(x => x.UserId)
            .Returns(1);

        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _dbContextMock.Setup(x => x.HardSkills)
            .Returns(new List<HardSkill>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert

        result.Should().BeEquivalentTo(createdSkill);
    }
}
