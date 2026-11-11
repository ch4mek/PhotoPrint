using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.CvProjects.Requests;

namespace Modsen.Portal.Cv.UnitTests.CvProjects.Requests;

public class GetCvProjectsLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetCvProjectsLookupRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetCvProjectsLookupRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new GetCvProjectsLookupRequest(
            _authenticatedUserContextMock.Object,
            _cvDbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentReturnsProjectsForDepartment()
    {
        // Arrange
        const int userId = 1;
        const string departmentId = "Net";
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
                CvUser =
                    new CvUserEntity
                    {
                        DepartmentId = departmentId,
                        FirstName = "test",
                        LastName = "test",
                        Email = "test@modsen-software.com"
                    },
                IsDeleted = false
            },
            new()
            {
                Id = 2,
                UserId = userId + 1,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
                CvUser = new CvUserEntity
                {
                    DepartmentId = departmentId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com"
                },
                IsDeleted = false
            }
        };
        var cvProjects = new List<CvProjectEntity>
        {
            new() { ProjectId = "project1", Name = "Project One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow },
            new() { ProjectId = "project2", Name = "Project Two", IsActive = true, Cvs = new List<CvEntity> { cvs[1] }, CreatedAtUtc = DateTime.UtcNow.AddDays(1) }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(cvProjects.BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(x => x.Id == "project1" && x.Name == "Project One");
        result.Should().ContainSingle(x => x.Id == "project2" && x.Name == "Project Two");
        // Proxy for CreatedAtUtc ordering
        result.Should().BeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerReturnsProjectsForResourceGroup()
    {
        // Arrange
        const int userId = 1;
        const int resourceGroupId = 20;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
                CvUser = new CvUserEntity { ResourceGroupId = resourceGroupId, FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com" },
                IsDeleted = false
            }
        };
        var cvProjects = new List<CvProjectEntity>
        {
            new() { ProjectId = "project1", Name = "Project One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(cvProjects.BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == "project1" && x.Name == "Project One");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeReturnsProjectsForUser()
    {
        // Arrange
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                IsDeleted = false,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        var cvProjects = new List<CvProjectEntity>
        {
            new() { ProjectId = "project1", Name = "Project One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(cvProjects.BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == "project1" && x.Name == "Project One");
    }

    [Fact]
    public async Task HandleWhenUserIsCtoReturnsProjectsForUser()
    {
        // Arrange
        const int userId1 = 1;
        const int userId2 = 1;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                UserId = userId1,
                IsDeleted = false,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
            },
            new()
            {
                Id = 2,
                UserId = userId2,
                IsDeleted = false,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        var cvProjects = new List<CvProjectEntity>
        {
            new() { ProjectId = "project1", Name = "Project One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow },
            new() { ProjectId = "project2", Name = "Project Two", IsActive = true, Cvs = new List<CvEntity> { cvs[1] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["CTO"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(cvProjects.BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == "project1" && x.Name == "Project One");
        result.Should().Contain(x => x.Id == "project2" && x.Name == "Project Two");
    }

    [Fact]
    public async Task HandleWhenNoCvsFoundReturnsEmptyList()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(new List<CvEntity>().BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(new List<CvProjectEntity>().BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenNoActiveProjectsFoundReturnsEmptyList()
    {
        // Arrange
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                IsDeleted = false,
                CvName = "test cv",
                UserFirstName = "test",
                UserLastName = "test",
            }
        };
        var cvProjects = new List<CvProjectEntity>
        {
            new() { ProjectId = "project1", Name = "Project One", IsActive = false, Cvs = new List<CvEntity> { cvs[0] } }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvProjects).Returns(cvProjects.BuildMockDbSet().Object);

        var query = new GetCvProjectsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }
}
