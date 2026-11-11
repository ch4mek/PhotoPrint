using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.BusinessRules;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Core.Validators;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Core.Validators;

public class UpdateCvValidatorTests
{
    private readonly Mock<ICvNameUniqueWithinUserBusinessRule> _cvNameUniqueWithinUserBusinessRuleMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly UpdateCvValidator _validator;
    private readonly CancellationToken _cancellationToken;

    public UpdateCvValidatorTests()
    {
        _cvNameUniqueWithinUserBusinessRuleMock = new Mock<ICvNameUniqueWithinUserBusinessRule>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _validator = new UpdateCvValidator(
            _cvNameUniqueWithinUserBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object,
            _cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task ValidateAsyncWhenValidDataPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        const string departmentId = "Net";
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences =
            [
                new() { Specialization = "Developer", ExperienceInYears = 5 },
                new() { Specialization = "Analyst", ExperienceInYears = 3 }
            ],
            Languages =
            [
                new() { Name = "English", Level = CvLanguageLevel.B2 },
                new() { Name = "Spanish", Level = CvLanguageLevel.A1 }
            ],
            SkillGroups =
            [
                new()
                {
                    Name = "Tech",
                    NameRu = "Технологии",
                    Skills =
                    [
                        new() { Name = "C#", Level = CvSkillLevel.Professional },
                        new() { Name = "Java", Level = CvSkillLevel.Medium }
                    ]
                },

                new()
                {
                    Name = "SoftSkills",
                    NameRu = "Софт скиллы",
                    Skills = [new() { Name = "Communication", Level = CvSkillLevel.Professional }]
                }
            ]
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, DepartmentId = departmentId, IsActive = true,FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsyncWhenCvNameNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true,FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(false);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.CvNameExists);
    }

    [Fact]
    public async Task ValidateAsyncWhenWorkExperiencesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences =
            [
                new() { Specialization = "Developer", ExperienceInYears = 5 },
                new() { Specialization = "Developer", ExperienceInYears = 3 }
            ],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSpecialization);
    }

    [Fact]
    public async Task ValidateAsyncWhenLanguagesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages =
            [
                new() { Name = "English", Level = CvLanguageLevel.B2 },
                new() { Name = "English", Level = CvLanguageLevel.C1 }
            ],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true,FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvLanguages);
    }

    [Fact]
    public async Task ValidateAsyncWhenSkillGroupNamesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups =
            [
                new() { Name = "Tech", NameRu = "Технологии1", Skills = [] },
                new() { Name = "Tech", NameRu = "Технологии2", Skills = [] }
            ]
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkillGroupNames);
    }

    [Fact]
    public async Task ValidateAsyncWhenSkillGroupRuNamesNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups =
            [
                new() { Name = "Tech1", NameRu = "Технологии", Skills = [] },
                new() { Name = "Tech2", NameRu = "Технологии", Skills = [] }
            ]
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkillGroupRuNames);
    }

    [Fact]
    public async Task ValidateAsyncWhenSkillsNotUniqueThrowsInvalidDataAppException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups =
            [
                new()
                {
                    Name = "Tech",
                    NameRu = "Технологии",
                    Skills =
                    [
                        new() { Name = "C#", Level = CvSkillLevel.Professional },
                        new() { Name = "C#", Level = CvSkillLevel.Medium }
                    ]
                }
            ]
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = async () => await _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.EnteredIdenticalCvSkills);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserNotFoundThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(2);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(new List<CvUserEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserNotActiveThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = false, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task ValidateAsyncWhenListsAreEmptyPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsHeadOfDepartmentAndUserInDepartmentPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        const string departmentId = "Net";
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, DepartmentId = departmentId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = async () => await _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsHeadOfDepartmentAndUserNotInDepartmentThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        const string departmentId = "Net";
        const string otherDepartmentId = "Mobile";
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, DepartmentId = otherDepartmentId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = async () => await _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsGroupManagerAndUserInResourceGroupPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        const int resourceGroupId = 100;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, ResourceGroupId = resourceGroupId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsGroupManagerAndUserNotInResourceGroupThrowsEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        const int resourceGroupId = 100;
        const int otherResourceGroupId = 200;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, ResourceGroupId = otherResourceGroupId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = async () => await _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsSystemAdministratorAndUserMatchesPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.SystemAdministrator]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = async () => await _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateAsyncWhenUserIsCtoAndUserExistsPassesWithoutException()
    {
        // Arrange
        const int userId = 1;
        const int cvId = 1;
        var coreDto = new UpdateCvValidatorCoreDto
        {
            Id = cvId,
            Name = "UpdatedCV",
            UserId = userId,
            WorkExperiences = [],
            Languages = [],
            SkillGroups = []
        };
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        _cvNameUniqueWithinUserBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(coreDto.Name, userId, cvId, _cancellationToken))
            .ReturnsAsync(true);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);
        _cvDbContextMock.Setup(x => x.CvUsers)
            .Returns(cvUsers.BuildMockDbSet().Object);

        // Act
        var action = () => _validator.ValidateAsync(coreDto, _cancellationToken);

        // Assert
        await action.Should().NotThrowAsync();
    }
}
