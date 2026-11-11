using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.LabBoard.Requests;

public class GetInternshipSpecializationsWithHardSkillsRequestTests
{
    private readonly GetInternshipSpecializationsWithHardSkillsRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetInternshipSpecializationsWithHardSkillsRequestTests()
    {
        _handler = new GetInternshipSpecializationsWithHardSkillsRequest(_dbContextMock.Object);

        SetupDbContext();
    }

    [Fact]
    public async Task HandleWhenSpecializationTemplatesAreActiveShouldReturn()
    {
        // Arrange
        var request = new GetInternshipSpecializationsWithHardSkillsRequest.Query();

        _dbContextMock.SetupGet(x => x.InternshipSpecializations)
            .Returns(new List<InternshipSpecialization>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.SpecializationTemplateId == 1);
        result.Should().Contain(x => x.SpecializationTemplateName == "Name");
    }

    private void SetupDbContext()
    {
        var templates = new List<SpecializationTemplate>()
        {
            new()
            {
                Id = 1,
                Name = "Name",
                SpecializationTemplateHardSkills =
                [
                    new SpecializationTemplateHardSkill
                    {
                        HardSkillId = 1,
                        HardSkill = new HardSkill
                        {
                            Name = "Hard Skill",
                            HardSkillGroupId = "1",
                        }
                    }
                ],
                IsActive = true,
            },
            new()
            {
                Id = 2,
                Name = "Name2",
                SpecializationTemplateHardSkills =
                [
                    new SpecializationTemplateHardSkill
                    {
                        HardSkillId = 1,
                        HardSkill = new HardSkill
                        {
                            Name = "Hard Skill",
                            HardSkillGroupId = "1",
                        }
                    }
                ],
                IsActive = false,
            }
        };

        _dbContextMock.SetupGet(x => x.SpecializationTemplates)
            .Returns(templates.BuildMockDbSet().Object);
    }
}
