using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.SofSkills.CreateSoftSkill;

public class CreateSoftSkillCommandHandlerTests
{
    private readonly Mock<IIsSoftSkillNameUniqueBusinessRule> _isSoftSkillNameUniqueBusinessRule = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly CreateSoftSkillRequest _handler;

    public CreateSoftSkillCommandHandlerTests()
    {
        _handler = new CreateSoftSkillRequest
        (
            _authenticatedUserContext.Object,
            _isSoftSkillNameUniqueBusinessRule.Object,
            _dbContextMock.Object,
            _clockServiceMock.Object
        );
    }

    [Fact]
    public async Task HandleWhenSoftSkillWithTheSameNameAlreadyExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var command = new CreateSoftSkillRequest.Command("Responsibility");

        _isSoftSkillNameUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(null, command.Name , _token))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(command, _token);

        // Assert
        await action.Should()
            .ThrowAsync<EntityConflictException>(PortalMessages.SoftSkillNameAlreadyOccupied);
    }

    [Fact]
    public async Task HandleWhenSoftSkillWithTheSameNameDoesNotExistShouldCreateNewSoftSkill()
    {
        // Arrange
        var skillName = "Responsibility";
        var command = new CreateSoftSkillRequest.Command(skillName);

        _isSoftSkillNameUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(null, command.Name , _token))
            .ReturnsAsync(true);

        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        _dbContextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _dbContextMock.Setup(x => x.SoftSkills.Add(It.IsAny<SoftSkill>()));

        _authenticatedUserContext.Setup(x => x.UserId)
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, _token);

        // Assert
        _dbContextMock
            .Verify(x => x.SoftSkills.Add(It.IsAny<SoftSkill>()), Times.Once());
        _dbContextMock
            .Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Name.Should().BeEquivalentTo(skillName);
    }
}
