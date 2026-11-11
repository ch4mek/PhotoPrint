using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Requests;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class GetCvsLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetCvsLookupRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetCvsLookupRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _handler = new GetCvsLookupRequest(_authenticatedUserContextMock.Object, _cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentReturnsCvsForDepartment()
    {
        // Arrange
        const string departmentId = "Net";
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, DepartmentId = departmentId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" },
            new() { UserId = 2, DepartmentId = departmentId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        var cvs = new List<CvEntity>
        {
            new() { Id = 1, CvName = "CV1", UserId = 1, IsDeleted = false, CvUser = cvUsers[0],
                UserFirstName = "test",
                UserLastName = "test", },
            new() { Id = 2, CvName = "CV2", UserId = 2, IsDeleted = false, CvUser = cvUsers[1],
                UserFirstName = "test",
                UserLastName = "test", }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(x => x.Id == 1 && x.CvName == "CV1");
        result.Should().ContainSingle(x => x.Id == 2 && x.CvName == "CV2");
        result.Should().BeInAscendingOrder(x => x.Id);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerReturnsCvsForResourceGroup()
    {
        // Arrange
        const int resourceGroupId = 20;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, ResourceGroupId = resourceGroupId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        var cvs = new List<CvEntity>
        {
            new() { Id = 1, CvName = "CV1", UserId = 1, IsDeleted = false, CvUser = cvUsers[0],
                UserFirstName = "test",
                UserLastName = "test", }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == 1 && x.CvName == "CV1");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeReturnsCvsForUser()
    {
        // Arrange
        const int userId = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        var cvs = new List<CvEntity>
        {
            new() { Id = 1, CvName = "CV1", UserId = userId, IsDeleted = false, CvUser = cvUsers[0],
                UserFirstName = "test",
                UserLastName = "test", }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == 1 && x.CvName == "CV1");
    }

    [Fact]
    public async Task HandleWhenUserIsCtoReturnsCvsForUser()
    {
        // Arrange
        const int userId1 = 1;
        const int userId2 = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId1, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" },
            new() { UserId = userId2, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        var cvs = new List<CvEntity>
        {
            new() { Id = 1, CvName = "CV1", UserId = userId1, IsDeleted = false, CvUser = cvUsers[0], 
                UserFirstName = "test",
                UserLastName = "test", },
            new() { Id = 2, CvName = "CV2", UserId = userId2, IsDeleted = false, CvUser = cvUsers[1], 
                UserFirstName = "test",
                UserLastName = "test", }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["CTO"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == 1 && x.CvName == "CV1");
        result.Should().Contain(x => x.Id == 2 && x.CvName == "CV2");
    }

    [Fact]
    public async Task HandleWhenNoCvsFoundReturnsEmptyList()
    {
        // Arrange
        const int userId = 1;
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(new List<CvEntity>().BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenNoActiveCvsFoundReturnsEmptyList()
    {
        // Arrange
        const int userId = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = userId, IsActive = true, FirstName = "test",
                LastName = "test",
                Email = "test@modsen-software.com" }
        };
        var cvs = new List<CvEntity>
        {
            new() { Id = 1, CvName = "CV1", UserId = userId, IsDeleted = true, CvUser = cvUsers[0],
                UserFirstName = "test",
                UserLastName = "test", }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetCvsLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }
}
