using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.CvClients.Requests;

namespace Modsen.Portal.Cv.UnitTests.CvClients.Requests;

public class GetCvClientsLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetCvClientsLookupRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetCvClientsLookupRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _handler = new GetCvClientsLookupRequest(_authenticatedUserContextMock.Object, _cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentReturnsClientsForDepartment()
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
        var cvClients = new List<CvClientEntity>
        {
            new() { ClientId = "client1", Name = "Client One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow },
            new() { ClientId = "client2", Name = "Client Two", IsActive = true, Cvs = new List<CvEntity> { cvs[1] }, CreatedAtUtc = DateTime.UtcNow.AddDays(1) }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(cvClients.BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(x => x.Id == "client1" && x.Name == "Client One");
        result.Should().ContainSingle(x => x.Id == "client2" && x.Name == "Client Two");
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerReturnsClientsForResourceGroup()
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
                CvUser = new CvUserEntity
                {
                    ResourceGroupId = resourceGroupId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com"
                },
                IsDeleted = false
            }
        };
        var cvClients = new List<CvClientEntity>
        {
            new() { ClientId = "client1", Name = "Client One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(cvClients.BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == "client1" && x.Name == "Client One");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeReturnsClientsForUser()
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
        var cvClients = new List<CvClientEntity>
        {
            new() { ClientId = "client1", Name = "Client One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(cvClients.BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == "client1" && x.Name == "Client One");
    }

    [Fact]
    public async Task HandleWhenUserIsCtoReturnsClientsForUser()
    {
        // Arrange
        const int userId1 = 1;
        const int userId2 = 2;
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
        var cvClients = new List<CvClientEntity>
        {
            new() { ClientId = "client1", Name = "Client One", IsActive = true, Cvs = new List<CvEntity> { cvs[0] }, CreatedAtUtc = DateTime.UtcNow },
            new() { ClientId = "client2", Name = "Client Two", IsActive = true, Cvs = new List<CvEntity> { cvs[1] }, CreatedAtUtc = DateTime.UtcNow }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["CTO"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(cvClients.BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == "client1" && x.Name == "Client One");
        result.Should().Contain(x => x.Id == "client2" && x.Name == "Client Two");
    }

    [Fact]
    public async Task HandleWhenNoCvsFoundReturnsEmptyList()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(new List<CvEntity>().BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(new List<CvClientEntity>().BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenNoActiveClientsFoundReturnsEmptyList()
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
        var cvClients = new List<CvClientEntity>
        {
            new() { ClientId = "client1", Name = "Client One", IsActive = false, Cvs = new List<CvEntity> { cvs[0] } }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);
        _cvDbContextMock.Setup(x => x.CvClients).Returns(cvClients.BuildMockDbSet().Object);

        var query = new GetCvClientsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }
}
