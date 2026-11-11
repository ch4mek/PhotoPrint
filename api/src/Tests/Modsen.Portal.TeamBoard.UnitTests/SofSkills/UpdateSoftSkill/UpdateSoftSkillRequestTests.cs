using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.SofSkills.UpdateSoftSkill;

public class UpdateSoftSkillRequestTests
{
    private readonly Mock<IIsSoftSkillNameUniqueBusinessRule> _isSoftSkillNameUniqueBusinessRule = new();
    private readonly Mock<IGetSoftSkillDbQuery> _getSoftSkillDbQuery = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly UpdateSoftSkillRequest _request;

    public UpdateSoftSkillRequestTests()
    {
        _request = new UpdateSoftSkillRequest(
            _isSoftSkillNameUniqueBusinessRule.Object,
            _getSoftSkillDbQuery.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenSoftSkillDoesNotExistShouldThrowEntityNotFoundException()
    {
        //Arrange
        var softSkillId = 1;
        var newSoftSKillName = "newSkillName";

        var command = new UpdateSoftSkillRequest.Command(softSkillId, newSoftSKillName);

        _getSoftSkillDbQuery.Setup(x => x.ExecuteAsync(softSkillId, _cancellationToken))
            .ReturnsAsync((SoftSkill?)null);

        //Act
        var result = async () => await  _request.Handle(command, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenSoftSkillWithTheSameNameAlreadyExistsShouldThrowEntityNotFoundException()
    {
        //Arrange
        var softSkillId = 1;
        var newSoftSKillName = "newSkillName";

        var command = new UpdateSoftSkillRequest.Command(softSkillId, newSoftSKillName);

        _getSoftSkillDbQuery.Setup(x => x.ExecuteAsync(softSkillId, _cancellationToken))
            .ReturnsAsync(new SoftSkill
            {
                Id = softSkillId,
                Name = "name",
                IsActive = true
            });

        _isSoftSkillNameUniqueBusinessRule.Setup(
                x => x.IsCorrespondsToBusiness(softSkillId, newSoftSKillName, _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var result = async () => await  _request.Handle(command, _cancellationToken);

        //Assert
        await result.Should().ThrowAsync<EntityConflictException>(PortalMessages.SoftSkillNameAlreadyOccupied);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnResponse()
    {
        //Arrange
        var softSkillId = 1;
        var newSoftSKillName = "newSkillName";

        var command = new UpdateSoftSkillRequest.Command(softSkillId, newSoftSKillName);

        _getSoftSkillDbQuery.Setup(x => x.ExecuteAsync(softSkillId, _cancellationToken))
            .ReturnsAsync(new SoftSkill
            {
                Id = softSkillId,
                Name = "name",
                IsActive = true
            });

        _isSoftSkillNameUniqueBusinessRule.Setup(
                x => x.IsCorrespondsToBusiness(softSkillId, newSoftSKillName, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken));

        //Act
        var result = await _request.Handle(command, _cancellationToken);

        //Assert
        result.Id.Should().Be(softSkillId);
        result.Name.Should().BeEquivalentTo(newSoftSKillName);
    }
}
