using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills.Requests;

public class GetSpecializationsWithHardSkillsRequestTests
{
    private readonly GetSpecializationsWithHardSkillsRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _dbContext = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetSpecializationsWithHardSkillsRequestTests()
    {
        _handler = new GetSpecializationsWithHardSkillsRequest(_dbContext.Object);
    }

    [Fact]
    public async Task HandleWhenUserSpecializationHasNoHardSkillsShouldReturnUserSpecializtion()
    {
        // Arrange
        var specializationTemplates = new List<SpecializationTemplate>
        {
            new SpecializationTemplate()
            {
                Id = 1,
                Name = "spec temp without hardskills",
                IsActive = true,
                UserSpecializations =
                [
                    new DataAccess.Npgsql.DatabaseEntities.UserSpecialization()
                    {
                        Id = 1,
                        UserId = 1,
                        IsPrimary = true,
                        IsVisibleSeniorityLevel = true,
                        IsVisibleSpecialization = true,
                        SpecializationHardSkills = [],
                        SeniorityLevelId = "TraineeL2",
                        CreatedAtUtc = DateTime.UtcNow,
                        SpecializationTemplateId = 1,
                        CreatedById = 1,
                    }
                ],
                SpecializationTemplateHardSkills = [],
                SpecializationTemplateDepartments = [],
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = 1,
                
            }
        };

        var users = new List<TeamBoardUser>
        {
            new TeamBoardUser()
            {
                UserId = 1,
                FirstName = "firstName",
                LastName = "Lastname",
                IsActive = true,
                Email = "email",
            }
        };

        _dbContext.Setup(x => x.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        _dbContext.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        var expected = new List<SpecializationTemplateWithHardSkillsDto>
        {
            new()
            {
                SpecializationTemplateId = specializationTemplates.First().Id,
                SpecializationTemplateName = specializationTemplates.First().Name,
                HardSkills = []
            }
        };

        var request = new GetSpecializationsWithHardSkillsRequest.Query(1, true);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task HandleWhenUserSpecializationHasHardSkillsShouldReturnUserSpecialization()
    {
        // Arrange
        var specializationTemplates = new List<SpecializationTemplate>
        {
            new SpecializationTemplate()
            {
                Id = 1,
                Name = "spec temp with hardskills",
                IsActive = true,
                UserSpecializations =
                [
                    new DataAccess.Npgsql.DatabaseEntities.UserSpecialization()
                    {
                        Id = 1,
                        UserId = 1,
                        IsPrimary = true,
                        IsVisibleSeniorityLevel = true,
                        IsVisibleSpecialization = true,
                        SpecializationHardSkills =
                        [
                            new UserSpecializationHardSkill()
                            {
                                HardSkillId = 1,
                                HardSkill = new HardSkill() { Id = 1, HardSkillGroupId = "1", Name = "hs" },
                                UserSpecializationId = 1,
                                SeniorityLevelId = "Trainee"
                            }
                        ],
                        SeniorityLevelId = "TraineeL2",
                        CreatedAtUtc = DateTime.UtcNow,
                        SpecializationTemplateId = 1,
                        CreatedById = 1,
                    }
                ],
                SpecializationTemplateHardSkills =
                [
                    new SpecializationTemplateHardSkill()
                    {
                        HardSkillId = 1,
                        HardSkill = new HardSkill() { Id = 1, HardSkillGroupId = "1", Name = "hs" },
                        SpecializationTemplateId = 1
                    }
                ],
                SpecializationTemplateDepartments = [],
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = 1,

            }
        };

        var users = new List<TeamBoardUser>
        {
            new TeamBoardUser()
            {
                UserId = 1,
                FirstName = "firstName",
                LastName = "Lastname",
                IsActive = true,
                Email = "email",
            }
        };

        _dbContext.Setup(x => x.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        _dbContext.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        var expected = new List<SpecializationTemplateWithHardSkillsDto>
        {
            new()
            {
                SpecializationTemplateId = specializationTemplates.First().Id,
                SpecializationTemplateName = specializationTemplates.First().Name,
                HardSkills = new List<ShortHardSkillDto>()
                {
                    new()
                    {
                        Id = 1,
                        Name = "hs",
                    }
                }
            }
        };

        var request = new GetSpecializationsWithHardSkillsRequest.Query(1, true);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task HandleWhenUserSpecializationHasHardSkillsWithoutSeniorityShouldReturnUserSpecialization()
    {
        // Arrange
        var specializationTemplates = new List<SpecializationTemplate>
        {
            new SpecializationTemplate()
            {
                Id = 1,
                Name = "spec temp with hardskills no seniority",
                IsActive = true,
                UserSpecializations =
                [
                    new DataAccess.Npgsql.DatabaseEntities.UserSpecialization()
                    {
                        Id = 1,
                        UserId = 1,
                        IsPrimary = true,
                        IsVisibleSeniorityLevel = true,
                        IsVisibleSpecialization = true,
                        SpecializationHardSkills =
                        [
                            new UserSpecializationHardSkill()
                            {
                                HardSkillId = 1,
                                HardSkill = new HardSkill() { Id = 1, HardSkillGroupId = "1", Name = "hs" },
                                UserSpecializationId = 1,
                                SeniorityLevelId = null
                            }
                        ],
                        SeniorityLevelId = "TraineeL2",
                        CreatedAtUtc = DateTime.UtcNow,
                        SpecializationTemplateId = 1,
                        CreatedById = 1,
                    }
                ],
                SpecializationTemplateHardSkills =
                [
                    new SpecializationTemplateHardSkill()
                    {
                        HardSkillId = 1,
                        HardSkill = new HardSkill() { Id = 1, HardSkillGroupId = "1", Name = "hs" },
                        SpecializationTemplateId = 1
                    }
                ],
                SpecializationTemplateDepartments = [],
                CreatedAtUtc = DateTime.UtcNow,
                CreatedById = 1,

            }
        };

        var users = new List<TeamBoardUser>
        {
            new TeamBoardUser()
            {
                UserId = 1,
                FirstName = "firstName",
                LastName = "Lastname",
                IsActive = true,
                Email = "email",
            }
        };

        _dbContext.Setup(x => x.SpecializationTemplates)
            .Returns(specializationTemplates.BuildMockDbSet().Object);

        _dbContext.Setup(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        var expected = new List<SpecializationTemplateWithHardSkillsDto>
        {
            new()
            {
                SpecializationTemplateId = specializationTemplates.First().Id,
                SpecializationTemplateName = specializationTemplates.First().Name,
                HardSkills = new List<ShortHardSkillDto>()
                {
                    new()
                    {
                        Id = 1,
                        Name = "hs",
                    }
                }
            }
        };

        var request = new GetSpecializationsWithHardSkillsRequest.Query(1, true);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
