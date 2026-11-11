using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Core.DbQueries;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;
using Modsen.Portal.Cv.Requests.Cvs.Requests;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class UpdateCvRequestTests
{
    private readonly Mock<IGetCvByIdWithIncludesDbQuery> _getCvByIdWithIncludesDbQueryMock;
    private readonly Mock<IUpdateCvValidator> _updateCvValidatorMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly UpdateCvRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public UpdateCvRequestTests()
    {
        _getCvByIdWithIncludesDbQueryMock = new Mock<IGetCvByIdWithIncludesDbQuery>();
        _updateCvValidatorMock = new Mock<IUpdateCvValidator>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _clockServiceMock = new Mock<IClockService>();
        _handler = new UpdateCvRequest(
            _getCvByIdWithIncludesDbQueryMock.Object,
            _updateCvValidatorMock.Object,
            _cvDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task HandleWhenValidDataUpdatesCvAndReturnsCvCoreDto()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var now = DateTime.UtcNow;
        var requestDto = new UpdateCvDto
        {
            Id = cvId,
            CvName = "UpdatedCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            UserProjectPosition = "Developer",
            UserLocation = "Minsk",
            UserEducation = "BS in CS",
            UserSkills = "C#, SQL",
            UserAbout = "Experienced developer",
            Website = "https://modsen-software.com",
            IsCorporateDesign = true,
            WorkExperiences = [new() { Specialization = "Developer", ExperienceInYears = 5 }],
            Languages = [new() { Name = "English", Level = CvLanguageLevel.B2 }],
            ProjectExperiences =
                [new() { Position = "Lead Developer", TeamSize = "5", StartDate = DateOnly.FromDateTime(now) }],
            SkillGroups =
            [
                new()
                {
                    Name = "Tech",
                    NameRu = "Технологии",
                    IsCustom = false,
                    Skills = [new() { Name = "C#", Level = CvSkillLevel.Professional }]
                }
            ]
        };
        var command = new UpdateCvRequest.Command(requestDto);
        var existingCv = new CvEntity
        {
            Id = cvId,
            CvName = "OldCV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = false,
            WorkExperiences = new List<WorkExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Specialization = "OldSpecialization", ExperienceInYears = 2 }
            },
            Languages = new List<LanguageEntity>
            {
                new() { Id = 1, CvId = cvId, Name = "French", Level = CvLanguageLevel.A1 }
            },
            ProjectExperiences = new List<ProjectExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Position = "OldPosition", TeamSize = "3" }
            },
            SkillGroups = new List<SkillGroupEntity>
            {
                new()
                {
                    Id = 1,
                    CvId = cvId,
                    Name = "OldGroup",
                    NameRu = "Старые навыки",
                    Skills = new List<SkillEntity>
                    {
                        new() { Id = 1, SkillGroupId = 1, Name = "OldSkill", Level = CvSkillLevel.Beginner }
                    }
                }
            }
        };
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync(existingCv);
        _updateCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .ReturnsAsync(1);
        _cvDbContextMock.Setup(x =>
            x.WorkExperiences.RemoveRange(It.IsAny<IEnumerable<WorkExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Languages.RemoveRange(It.IsAny<IEnumerable<LanguageEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.ProjectExperiences.RemoveRange(It.IsAny<IEnumerable<ProjectExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Skills.RemoveRange(It.IsAny<IEnumerable<SkillEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.SkillGroups.RemoveRange(It.IsAny<IEnumerable<SkillGroupEntity>>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("UpdatedCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.UserProjectPosition.Should().Be("Developer");
        result.UserLocation.Should().Be("Minsk");
        result.UserEducation.Should().Be("BS in CS");
        result.UserSkills.Should().Be("C#, SQL");
        result.UserAbout.Should().Be("Experienced developer");
        result.Website.Should().Be("https://modsen-software.com");
        result.IsCorporateDesign.Should().BeTrue();
        result.WorkExperiences.Should().HaveCount(1);
        result.WorkExperiences[0].Specialization.Should().Be("Developer");
        result.WorkExperiences[0].ExperienceInYears.Should().Be(5);
        result.Languages.Should().HaveCount(1);
        result.Languages[0].Name.Should().Be("English");
        result.Languages[0].Level.Should().Be(CvLanguageLevel.B2);
        result.ProjectExperiences.Should().HaveCount(1);
        result.ProjectExperiences[0].Position.Should().Be("Lead Developer");
        result.ProjectExperiences[0].TeamSize.Should().Be("5");
        result.ProjectExperiences[0].StartDate.Should().Be(DateOnly.FromDateTime(now));
        result.ProjectExperiences[0].OrderWeight.Should().Be(0);
        result.SkillGroups.Should().HaveCount(1);
        result.SkillGroups[0].Name.Should().Be("Tech");
        result.SkillGroups[0].NameRu.Should().Be("Технологии");
        result.SkillGroups[0].IsCustom.Should().BeFalse();
        result.SkillGroups[0].OrderWeight.Should().Be(0);
        result.SkillGroups[0].Skills.Should().HaveCount(1);
        result.SkillGroups[0].Skills[0].Name.Should().Be("C#");
        result.SkillGroups[0].Skills[0].Level.Should().Be(CvSkillLevel.Professional);
        result.SkillGroups[0].Skills[0].OrderWeight.Should().Be(0);
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenCvNotFoundThrowsEntityNotFoundException()
    {
        // Arrange
        const int cvId = 1;
        var requestDto = new UpdateCvDto { Id = cvId, CvName = "UpdatedCV", UserId = 1, UserFirstName = "John", UserLastName = "Doe" };
        var command = new UpdateCvRequest.Command(requestDto);
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync((CvEntity?)null);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.CvNotFound);
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenValidatorThrowsInvalidDataAppExceptionThrowsSameException()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var requestDto = new UpdateCvDto { Id = cvId, CvName = "UpdatedCV", UserId = userId, UserFirstName = "John", UserLastName = "Doe" };
        var command = new UpdateCvRequest.Command(requestDto);
        var existingCv = new CvEntity { Id = cvId, CvName = "OldCV", UserId = userId, IsDeleted = false,
            UserFirstName = "test",
            UserLastName = "test",
        };
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync(existingCv);
        _updateCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.CvNameExists));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CvNameExists);
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenValidatorThrowsEntityNotFoundExceptionThrowsSameException()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var requestDto = new UpdateCvDto { Id = cvId, CvName = "UpdatedCV", UserId = userId, UserFirstName = "John", UserLastName = "Doe" };
        var command = new UpdateCvRequest.Command(requestDto);
        var existingCv = new CvEntity { Id = cvId, CvName = "OldCV", UserId = userId, IsDeleted = false,
            UserFirstName = "test",
            UserLastName = "test",
        };
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync(existingCv);
        _updateCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException(PortalMessages.UserNotFound));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenListsAreEmptyUpdatesCvWithEmptyCollections()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var now = DateTime.UtcNow;
        var requestDto = new UpdateCvDto
        {
            Id = cvId,
            CvName = "UpdatedCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences = [],
            Languages = [],
            ProjectExperiences = [],
            SkillGroups = []
        };
        var command = new UpdateCvRequest.Command(requestDto);
        var existingCv = new CvEntity
        {
            Id = cvId,
            CvName = "OldCV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = false,
            WorkExperiences = new List<WorkExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Specialization = "OldSpecialization", ExperienceInYears = 2 }
            },
            Languages = new List<LanguageEntity>
            {
                new() { Id = 1, CvId = cvId, Name = "French", Level = CvLanguageLevel.A1 }
            },
            ProjectExperiences = new List<ProjectExperienceEntity>
            {
                new() { Id = 1, CvId = cvId, Position = "OldPosition", TeamSize = "3" }
            },
            SkillGroups = new List<SkillGroupEntity>
            {
                new()
                {
                    Id = 1,
                    CvId = cvId,
                    Name = "OldGroup",
                    NameRu = "Старые навыки",
                    Skills = new List<SkillEntity>
                    {
                        new() { Id = 1, SkillGroupId = 1, Name = "OldSkill", Level = CvSkillLevel.Beginner }
                    }
                }
            }
        };
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync(existingCv);
        _updateCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .ReturnsAsync(1);
        _cvDbContextMock.Setup(x =>
            x.WorkExperiences.RemoveRange(It.IsAny<IEnumerable<WorkExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Languages.RemoveRange(It.IsAny<IEnumerable<LanguageEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.ProjectExperiences.RemoveRange(It.IsAny<IEnumerable<ProjectExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Skills.RemoveRange(It.IsAny<IEnumerable<SkillEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.SkillGroups.RemoveRange(It.IsAny<IEnumerable<SkillGroupEntity>>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("UpdatedCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenOptionalFieldsAreNullUpdatesCvWithNullFields()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var now = DateTime.UtcNow;
        var requestDto = new UpdateCvDto
        {
            Id = cvId,
            CvName = "UpdatedCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            UserProjectPosition = null,
            UserLocation = null,
            UserEducation = null,
            UserSkills = null,
            UserAbout = null,
            Website = null,
            IsCorporateDesign = false,
            WorkExperiences = [],
            Languages = [],
            ProjectExperiences = [],
            SkillGroups = []
        };
        var command = new UpdateCvRequest.Command(requestDto);
        var existingCv = new CvEntity
        {
            Id = cvId,
            CvName = "OldCV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            IsDeleted = false,
            UserProjectPosition = "OldPosition",
            UserLocation = "OldLocation",
            UserEducation = "OldEducation",
            UserSkills = "OldSkills",
            UserAbout = "OldAbout",
            Website = "https://old.com",
            IsCorporateDesign = true,
            WorkExperiences = new List<WorkExperienceEntity>(),
            Languages = new List<LanguageEntity>(),
            ProjectExperiences = new List<ProjectExperienceEntity>(),
            SkillGroups = new List<SkillGroupEntity>()
        };
        _getCvByIdWithIncludesDbQueryMock
            .Setup(x => x.ExecuteAsync(cvId, _cancellationToken))
            .ReturnsAsync(existingCv);
        _updateCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<UpdateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRequiredUserId()).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .ReturnsAsync(1);
        _cvDbContextMock.Setup(x =>
            x.WorkExperiences.RemoveRange(It.IsAny<IEnumerable<WorkExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Languages.RemoveRange(It.IsAny<IEnumerable<LanguageEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.ProjectExperiences.RemoveRange(It.IsAny<IEnumerable<ProjectExperienceEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.Skills.RemoveRange(It.IsAny<IEnumerable<SkillEntity>>()));
        _cvDbContextMock.Setup(x =>
            x.SkillGroups.RemoveRange(It.IsAny<IEnumerable<SkillGroupEntity>>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("UpdatedCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.UserProjectPosition.Should().BeNull();
        result.UserLocation.Should().BeNull();
        result.UserEducation.Should().BeNull();
        result.UserSkills.Should().BeNull();
        result.UserAbout.Should().BeNull();
        result.Website.Should().BeNull();
        result.IsCorporateDesign.Should().BeFalse();
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }
}
