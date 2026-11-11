using MockQueryable.Moq;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.DbQueries;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Core;

public class GetCvByIdWithIncludesDbQueryTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetCvByIdWithIncludesDbQuery _query;
    private readonly CancellationToken _cancellationToken;

    public GetCvByIdWithIncludesDbQueryTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _query = new GetCvByIdWithIncludesDbQuery(_cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ExecuteAsyncWhenCvExistsAndIsActiveReturnsCvWithIncludes()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "MyCV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = false,
            WorkExperiences = new List<WorkExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Specialization = "Developer", ExperienceInYears = 5 },
                new() { Id = 2, CvId = cvId, Specialization = "Analyst", ExperienceInYears = 3 }
            },
            Languages = new List<LanguageEntity>
            {
                new() { Id = 1, CvId = cvId, Name = "English", Level = CvLanguageLevel.B2 },
                new() { Id = 2, CvId = cvId, Name = "Spanish", Level = CvLanguageLevel.A1 }
            },
            ProjectExperiences = new List<ProjectExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Position = "Lead Developer", TeamSize = "5", StartDate = DateOnly.FromDateTime(DateTime.UtcNow) }
            },
            SkillGroups = new List<SkillGroupEntity>
            {
                new()
                {
                    Id = 1,
                    CvId = cvId,
                    Name = "Tech",
                    NameRu = "Технологии",
                    Skills = new List<SkillEntity>
                    {
                        new() { Id = 1, SkillGroupId = 1, Name = "C#", Level = CvSkillLevel.Professional },
                        new() { Id = 2, SkillGroupId = 1, Name = "Java", Level = CvSkillLevel.Medium }
                    }
                },
                new()
                {
                    Id = 2,
                    CvId = cvId,
                    Name = "SoftSkills",
                    NameRu = "Софт скиллы",
                    Skills = new List<SkillEntity>
                    {
                        new() { Id = 3, SkillGroupId = 2, Name = "Communication", Level = CvSkillLevel.Professional }
                    }
                }
            }
        };
        var cvs = new List<CvEntity> { cv };
        var mockDbSet = cvs.BuildMockDbSet();
        _cvDbContextMock.Setup(x => x.Cvs).Returns(mockDbSet.Object);

        // Act
        var result = await _query.ExecuteAsync(cvId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.IsDeleted.Should().BeFalse();
        result.WorkExperiences.Should().HaveCount(2);
        result.WorkExperiences.Should().Contain(x => x.Specialization == "Developer" && x.ExperienceInYears == 5);
        result.WorkExperiences.Should().Contain(x => x.Specialization == "Analyst" && x.ExperienceInYears == 3);
        result.Languages.Should().HaveCount(2);
        result.Languages.Should().Contain(x => x.Name == "English" && x.Level == CvLanguageLevel.B2);
        result.Languages.Should().Contain(x => x.Name == "Spanish" && x.Level == CvLanguageLevel.A1);
        result.ProjectExperiences.Should().HaveCount(1);
        result.ProjectExperiences.Should().Contain(x => x.Position == "Lead Developer" && x.TeamSize == "5");
        result.SkillGroups.Should().HaveCount(2);
        var techGroup = result.SkillGroups.First(x => x.Name == "Tech");
        techGroup.NameRu.Should().Be("Технологии");
        techGroup.Skills.Should().HaveCount(2);
        techGroup.Skills.Should().Contain(x => x.Name == "C#" && x.Level == CvSkillLevel.Professional);
        techGroup.Skills.Should().Contain(x => x.Name == "Java" && x.Level == CvSkillLevel.Medium);
        var softSkillsGroup = result.SkillGroups.First(x => x.Name == "SoftSkills");
        softSkillsGroup.NameRu.Should().Be("Софт скиллы");
        softSkillsGroup.Skills.Should().HaveCount(1);
        softSkillsGroup.Skills.Should().Contain(x => x.Name == "Communication" && x.Level == CvSkillLevel.Professional);
    }

    [Fact]
    public async Task ExecuteAsyncWhenCvDoesNotExistReturnsNull()
    {
        // Arrange
        const int cvId = 1;
        var cvs = new List<CvEntity>();
        var mockDbSet = cvs.BuildMockDbSet();
        _cvDbContextMock.Setup(x => x.Cvs).Returns(mockDbSet.Object);

        // Act
        var result = await _query.ExecuteAsync(cvId, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsyncWhenCvIsDeletedReturnsNull()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "test cv",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = true,
            WorkExperiences = new List<WorkExperienceEntity>(),
            Languages = new List<LanguageEntity>(),
            ProjectExperiences = new List<ProjectExperienceEntity>(),
            SkillGroups = new List<SkillGroupEntity>()
        };
        var cvs = new List<CvEntity> { cv };
        var mockDbSet = cvs.BuildMockDbSet();
        _cvDbContextMock.Setup(x => x.Cvs).Returns(mockDbSet.Object);

        // Act
        var result = await _query.ExecuteAsync(cvId, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsyncWhenCvExistsWithEmptyCollectionsReturnsCvWithEmptyCollections()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "MyCV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = false,
            WorkExperiences = new List<WorkExperienceEntity>(),
            Languages = new List<LanguageEntity>(),
            ProjectExperiences = new List<ProjectExperienceEntity>(),
            SkillGroups = new List<SkillGroupEntity>()
        };
        var cvs = new List<CvEntity> { cv };
        var mockDbSet = cvs.BuildMockDbSet();
        _cvDbContextMock.Setup(x => x.Cvs).Returns(mockDbSet.Object);

        // Act
        var result = await _query.ExecuteAsync(cvId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.IsDeleted.Should().BeFalse();
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
    }
}
