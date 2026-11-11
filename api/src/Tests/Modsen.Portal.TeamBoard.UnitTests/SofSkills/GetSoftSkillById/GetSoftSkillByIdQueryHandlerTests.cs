using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.SofSkills.GetSoftSkillById;

public class GetSoftSkillByIdQueryHandlerTests
{
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContext = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetSoftSkillByIdRequest _handler;

    public GetSoftSkillByIdQueryHandlerTests()
    {
        _handler = new GetSoftSkillByIdRequest(_teamboardDbContext.Object);
    }

    [Fact]
    public async Task HandleWhenSoftSkillDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var id = 2;
        var query = new GetSoftSkillByIdRequest.Query(id);

        _teamboardDbContext.Setup(x => x.SoftSkills)
            .Returns(new List<SoftSkill>().BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"Soft skill by id {id} is not found");
    }

    [Fact]
    public async Task HandleWhenSoftSkillExistsButIsNotactiveShouldThrowEntityNotFoundException()
    {
        // Arrange
        var id = 2;

        var query = new GetSoftSkillByIdRequest.Query(id);

        _teamboardDbContext.Setup(x => x.SoftSkills)
            .Returns(new List<SoftSkill>()
            {
                new()
                {
                    Id = 2,
                    IsActive = false,
                    Name = "Soft skill",
                }
            }.BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"Soft skill by id {id} is not found");
    }

    [Fact]
    public async Task HandleWhenSoftSkillExistsShouldReturnSoftSkill()
    {
        // Arrange
        var id = 2;

        var query = new GetSoftSkillByIdRequest.Query(id);

        _teamboardDbContext.Setup(x => x.SoftSkills)
            .Returns(new List<SoftSkill>()
            {
                new()
                {
                   Id = 2,
                   IsActive = true,
                   Name = "Soft skill",
                }
            }.BuildMockDbSet().Object);

        var expectedResult = new SoftSkillDto { Id = query.Id, Name = "Soft skill" };

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
