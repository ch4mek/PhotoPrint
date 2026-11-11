using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Requests;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class GetCvByIdRequestTests
{
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly GetCvByIdRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetCvByIdRequestTests()
    {
        _cvDbContextMock = new Mock<ICvDbContext>();
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new GetCvByIdRequest(
            _cvDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenCvNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int cvId = 1;
        var query = new GetCvByIdRequest.Query(cvId);

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(new List<CvEntity>().BuildMockDbSet().Object);

        // Act
        var action = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.CvNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndCvInDepartmentReturnsCvCoreDto()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        const string departmentId = "Net";
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserId = userId,
            CvUser = new CvUserEntity { UserId = userId, DepartmentId = departmentId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false,
            Status = CvStatus.Draft,
            UserFirstName = "John",
            UserLastName = "Doe",
            WorkExperiences = new List<WorkExperienceEntity>
            {
                new() { Specialization = "Developer", ExperienceInYears = 5 }
            },
            Languages = new List<LanguageEntity>
            {
                new() { Name = "English", Level = CvLanguageLevel.B2 }
            },
            ProjectExperiences = new List<ProjectExperienceEntity>
            {
                new() { Position = "Lead Developer", TeamSize = "5" }
            },
            SkillGroups = new List<SkillGroupEntity>
            {
                new()
                {
                    Name = "Tech",
                    NameRu = "test",
                    Skills = new List<SkillEntity>
                    {
                        new() { Name = "C#", Level = CvSkillLevel.Professional }
                    }
                }
            }
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("Test CV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
        result.WorkExperiences.Should().HaveCount(1);
        result.WorkExperiences[0].Specialization.Should().Be("Developer");
        result.Languages.Should().HaveCount(1);
        result.Languages[0].Name.Should().Be("English");
        result.ProjectExperiences.Should().HaveCount(1);
        result.ProjectExperiences[0].Position.Should().Be("Lead Developer");
        result.SkillGroups.Should().HaveCount(1);
        result.SkillGroups[0].Skills.Should().HaveCount(1);
        result.SkillGroups[0].Skills[0].Name.Should().Be("C#");
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentAndCvNotInDepartmentShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        const string departmentId = "Net";
        const string otherDepartmentId = "Mobile";
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = userId,
            CvUser = new CvUserEntity { UserId = userId, DepartmentId = otherDepartmentId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);

        // Act
        var action = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.CvNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerAndCvInResourceGroupReturnsCvCoreDto()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        const int resourceGroupId = 100;
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserId = userId,
            CvUser = new CvUserEntity { UserId = userId, ResourceGroupId = resourceGroupId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false,
            Status = CvStatus.Draft,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("Test CV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeAndCvBelongsToUserReturnsCvCoreDto()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserId = userId,
            CvUser = new CvUserEntity { UserId = userId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false,
            Status = CvStatus.Draft,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("Test CV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeAndCvDoesNotBelongToUserShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        const int otherUserId = 2;
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserFirstName = "test",
            UserLastName = "test",
            UserId = otherUserId,
            CvUser = new CvUserEntity { UserId = otherUserId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);

        // Act
        var action = () => _handler.Handle(query, _cancellationToken);

        // Assert
        await action.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.CvNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsCtoAndCvExistsReturnsCvCoreDto()
    {
        // Arrange
        const int cvId = 1;
        const int userId = 1;
        var query = new GetCvByIdRequest.Query(cvId);
        var cv = new CvEntity
        {
            Id = cvId,
            CvName = "Test CV",
            UserId = userId,
            CvUser = new CvUserEntity { UserId = userId,
                FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com"
            },
            IsDeleted = false,
            Status = CvStatus.Draft,
            UserFirstName = "John",
            UserLastName = "Doe"
        };
        var cvs = new List<CvEntity> { cv };

        _cvDbContextMock
            .Setup(x => x.Cvs)
            .Returns(cvs.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cvId);
        result.CvName.Should().Be("Test CV");
        result.UserId.Should().Be(userId);
        result.UserFirstName.Should().Be("John");
        result.UserLastName.Should().Be("Doe");
    }
}
