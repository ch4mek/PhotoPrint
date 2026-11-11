using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;
using Modsen.Portal.Cv.Requests.Cvs.Requests;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class CreateCvRequestTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly Mock<ICreateCvValidator> _createCvValidatorMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly CreateCvRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public CreateCvRequestTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _createCvValidatorMock = new Mock<ICreateCvValidator>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _clockServiceMock = new Mock<IClockService>();
        _cancellationToken = CancellationToken.None;

        _handler = new CreateCvRequest(
            _cvDbContextMock.Object,
            _createCvValidatorMock.Object,
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenValidDataReturnsCvCoreDto()
    {
        // Arrange
        const int userId = 1;
        var now = DateTime.UtcNow;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
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
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        var addedCv = (CvEntity?)null;
        _cvDbContextMock.Setup(x => x.Cvs.Add(It.IsAny<CvEntity>()))
            .Callback<CvEntity>(cv => addedCv = cv);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Callback(() =>
            {
                if (addedCv != null)
                {
                    addedCv.Id = 1;
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.Status.Should().Be(CvStatus.Draft);
        result.IsVisibleToUser.Should().BeFalse();
        result.UserProjectPosition.Should().Be("Developer");
        result.UserLocation.Should().Be("Minsk");
        result.UserEducation.Should().Be("BS in CS");
        result.UserSkills.Should().Be("C#, SQL");
        result.UserAbout.Should().Be("Experienced developer");
        result.Website.Should().Be("https://modsen-software.com");
        result.IsCorporateDesign.Should().BeTrue();
        result.CreatedById.Should().Be(userId);
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
    public async Task HandleWhenCvNameNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.CvNameExists));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CvNameExists);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenUserNotFoundThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException(PortalMessages.UserNotFound));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenListsAreEmptyReturnsCvCoreDtoWithEmptyLists()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences = [],
            Languages = [],
            ProjectExperiences = [],
            SkillGroups = []
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        var addedCv = (CvEntity?)null;
        _cvDbContextMock.Setup(x => x.Cvs.Add(It.IsAny<CvEntity>()))
            .Callback<CvEntity>(cv => addedCv = cv);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Callback(() =>
            {
                if (addedCv != null)
                {
                    addedCv.Id = 1;
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.Status.Should().Be(CvStatus.Draft);
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenOptionalFieldsAreNullReturnsCvCoreDtoWithNullFields()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
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
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
        var addedCv = (CvEntity?)null;
        _cvDbContextMock.Setup(x => x.Cvs.Add(It.IsAny<CvEntity>()))
            .Callback<CvEntity>(cv => addedCv = cv);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Callback(() =>
            {
                if (addedCv != null)
                {
                    addedCv.Id = 1;
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.Status.Should().Be(CvStatus.Draft);
        result.IsVisibleToUser.Should().BeFalse();
        result.UserProjectPosition.Should().BeNull();
        result.UserLocation.Should().BeNull();
        result.UserEducation.Should().BeNull();
        result.UserSkills.Should().BeNull();
        result.UserAbout.Should().BeNull();
        result.Website.Should().BeNull();
        result.IsCorporateDesign.Should().BeFalse();
        result.CreatedById.Should().Be(userId);
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenWorkExperiencesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences =
            [
                new() { Specialization = "Developer", ExperienceInYears = 5 },
                new() { Specialization = "Developer", ExperienceInYears = 3 }
            ]
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSpecialization));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSpecialization);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenLanguagesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            Languages =
            [
                new() { Name = "English", Level = CvLanguageLevel.B2 },
                new() { Name = "English", Level = CvLanguageLevel.C1 }
            ]
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.EnteredIdenticalCvLanguages));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvLanguages);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenSkillGroupNamesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            SkillGroups =
            [
                new() { Name = "Tech", NameRu = "Технологии1", IsCustom = false },
                new() { Name = "Tech", NameRu = "Технологии2", IsCustom = false }
            ]
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkillGroupNames));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkillGroupNames);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenSkillGroupRuNamesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            SkillGroups =
            [
                new() { Name = "Tech1", NameRu = "Технологии", IsCustom = false },
                new() { Name = "Tech2", NameRu = "Технологии", IsCustom = false }
            ]
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkillGroupRuNames));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkillGroupRuNames);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenSkillsNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            SkillGroups =
            [
                new()
                {
                    Name = "Tech",
                    NameRu = "Технологии",
                    IsCustom = false,
                    Skills =
                    [
                        new() { Name = "C#", Level = CvSkillLevel.Professional },
                        new() { Name = "C#", Level = CvSkillLevel.Beginner }
                    ]
                }
            ]
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new InvalidDataAppException(PortalMessages.EnteredIdenticalCvSkills));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkills);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeAndCreatesCvForSelfReturnsCvCoreDto()
    {
        // Arrange
        const int userId = 1;
        var now = DateTime.UtcNow;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences = [],
            Languages = [],
            ProjectExperiences = [],
            SkillGroups = []
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        var addedCv = (CvEntity?)null;
        _cvDbContextMock.Setup(x => x.Cvs.Add(It.IsAny<CvEntity>()))
            .Callback<CvEntity>(cv => addedCv = cv);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Callback(() =>
            {
                if (addedCv != null)
                {
                    addedCv.Id = 1;
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.Status.Should().Be(CvStatus.Draft);
        result.IsVisibleToUser.Should().BeFalse();
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentCreatesCvForUserInDepartmentReturnsCvCoreDto()
    {
        // Arrange
        const int userId = 1;
        const int creatorUserId = 2;
        const string departmentId = "Net";
        var now = DateTime.UtcNow;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences = [],
            Languages = [],
            ProjectExperiences = [],
            SkillGroups = []
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .Returns(Task.CompletedTask);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(creatorUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(now);
        var addedCv = (CvEntity?)null;
        _cvDbContextMock.Setup(x => x.Cvs.Add(It.IsAny<CvEntity>()))
            .Callback<CvEntity>(cv => addedCv = cv);
        _cvDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken))
            .Callback(() =>
            {
                if (addedCv != null)
                {
                    addedCv.Id = 1;
                }
            })
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.CvName.Should().Be("MyCV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.Status.Should().Be(CvStatus.Draft);
        result.IsVisibleToUser.Should().BeFalse();
        result.CreatedById.Should().Be(creatorUserId);
        result.WorkExperiences.Should().BeEmpty();
        result.Languages.Should().BeEmpty();
        result.ProjectExperiences.Should().BeEmpty();
        result.SkillGroups.Should().BeEmpty();
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Once());
    }

    [Fact]
    public async Task HandleWhenEmployeeCreatesCvForAnotherUserThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int creatorUserId = 2;
        var requestDto = new CreateCvDto
        {
            CvName = "MyCV",
            UserId = userId,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var command = new CreateCvRequest.Command(requestDto);
        _createCvValidatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<CreateCvValidatorCoreDto>(), _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException(PortalMessages.UserNotFound));
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(creatorUserId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        // Act
        var action = () => _handler.Handle(command, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
        _cvDbContextMock.Verify(x => x.Cvs.Add(It.IsAny<CvEntity>()), Times.Never());
        _cvDbContextMock.Verify(x => x.SaveChangesAsync(_cancellationToken), Times.Never());
    }
}
