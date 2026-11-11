using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills;

public class UpdateHardSkillRequestTests
{
    private readonly Mock<IHardSkillUpdateValidator> _hardSkillUpdateValidatorMock = new();
    private readonly Mock<IGetHardSkillDbQuery> _getHardSkillDbQuery = new();
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContext = new();

    private readonly CancellationToken _token;
    private readonly UpdateHardSkillRequest _handler;

    public UpdateHardSkillRequestTests()
    {
        _token = CancellationToken.None;

        _handler = new UpdateHardSkillRequest(
            _getHardSkillDbQuery.Object,
            _teamBoardDbContext.Object,
            _hardSkillUpdateValidatorMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenHardSkillDoesNotExistsShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateHardSkillRequest.Command(1, "SkillName", "GroupID");

        _getHardSkillDbQuery.Setup(x =>
                x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HardSkill?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, _token));
    }

    [Fact]
    public async Task HandleWhenHardSkillWithTheSameNameAlreadyExistsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateHardSkillRequest.Command(1, "SkillName", "GroupID");

        _getHardSkillDbQuery.Setup(x =>
                x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HardSkill
            {
                Id = command.Id,
                Name = command.Name,
                HardSkillGroupId = command.HardSkillGroupId
            });

        _hardSkillUpdateValidatorMock
            .Setup(x => x.ValidateAsync(command.Id, command.Name, command.HardSkillGroupId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataAppException("Hard skill with the same name already exists"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataAppException>(() => _handler.Handle(command, _token));
    }

    [Fact]
    public async Task HandleWhenHardSkillGroupDoesNotExistsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var command = new UpdateHardSkillRequest.Command(1, "SkillName", "GroupID");

        _getHardSkillDbQuery.Setup(x =>
                x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HardSkill
            {
                Id = command.Id,
                Name = command.Name,
                HardSkillGroupId = command.HardSkillGroupId
            });

        _hardSkillUpdateValidatorMock
            .Setup(x => x.ValidateAsync(command.Id, command.Name, command.HardSkillGroupId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataAppException("There is no hard skill group with such id"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataAppException>(() => _handler.Handle(command, _token));
    }

    [Fact]
    public async Task HandleWhenHardSkillWithTheSameNameDoesNotExistShouldUpdateExistingHardSkill()
    {
        // Arrange
        var id = 1;
        var skillName = "Indexes";
        var groupId = "GroupID";
        var command = new UpdateHardSkillRequest.Command(id, skillName, groupId);

        _getHardSkillDbQuery.Setup(x =>
                x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HardSkill
            {
                Id = 1,
                Name = "PreviousName",
                HardSkillGroupId = command.HardSkillGroupId
            });

        _hardSkillUpdateValidatorMock
            .Setup(x => x.ValidateAsync(null, command.Name, command.HardSkillGroupId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _teamBoardDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        var expectedResult = new HardSkillDto
        {
            Id = id,
            Name = skillName,
            HardSkillGroupId = groupId
        };

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
