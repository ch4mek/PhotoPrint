using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.HardSkills;

public class GetHardSkillsLookupRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetHardSkillsLookupRequest _handler;

    public GetHardSkillsLookupRequestTests()
    {
        _handler = new GetHardSkillsLookupRequest(_teamBoardDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenHardSkillsDoesNotExistShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetHardSkillsLookupRequest.Query();

        _teamBoardDbContextMock.Setup(x => x.HardSkills)
            .Returns(new List<HardSkill>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, cancellationToken: _token);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenTwoHardSkillsExistShouldReturnListOfTwoSkills()
    {
        // Arrange
        var query = new GetHardSkillsLookupRequest.Query();

        var items = new List<HardSkillLookupDto>
        {
            new (Id: 1, Name: "Skill1"),
            new (Id: 2, Name: "Skill2")
        };

        _teamBoardDbContextMock.Setup(x => x.HardSkills)
            .Returns(new List<HardSkill>
            {
                new()
                {
                    Id = 1,
                    Name = "Skill1", HardSkillGroupId = "1",
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    Name = "Skill2",HardSkillGroupId = "1",
                    IsActive = true
                },
                new()
                {
                    Id = 3,
                    Name = "Skill3",HardSkillGroupId = "1",
                    IsActive = false
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(items);
    }
}
