using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.CvUsers.Requests;

namespace Modsen.Portal.Cv.UnitTests.CvUsers.Requests;

public class GetCvUsersLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetCvUsersLookupRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetCvUsersLookupRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _handler = new GetCvUsersLookupRequest(_authenticatedUserContextMock.Object, _cvDbContextMock.Object);
        _cancellationToken = CancellationToken.None;
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentReturnsUsersForDepartment()
    {
        // Arrange
        const string departmentId = "Net";
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, FirstName = "John", LastName = "Doe", FirstNameRu = "Иван", LastNameRu = "Иванов", DepartmentId = departmentId, IsActive = true, Email = "test" },
            new() { UserId = 2, FirstName = "Jane", LastName = "Smith", FirstNameRu = "Анна", LastNameRu = "Смит", DepartmentId = departmentId, IsActive = true, Email = "test" }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(cvUsers.BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainSingle(x => x.Id == 1 && x.FullName == "Doe John" && x.FullNameRu == "Иванов Иван");
        result.Should().ContainSingle(x => x.Id == 2 && x.FullName == "Smith Jane" && x.FullNameRu == "Смит Анна");
        result.Should().BeInAscendingOrder(x => x.Id);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerReturnsUsersForResourceGroup()
    {
        // Arrange
        const int resourceGroupId = 20;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, FirstName = "John", LastName = "Doe", FirstNameRu = "Иван", LastNameRu = "Иванов", ResourceGroupId = resourceGroupId, IsActive = true, Email = "test" }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(cvUsers.BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == 1 && x.FullName == "Doe John" && x.FullNameRu == "Иванов Иван");
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeReturnsUsersForUser()
    {
        // Arrange
        const int userId = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, FirstName = "John", LastName = "Doe", FirstNameRu = "Иван", LastNameRu = "Иванов", IsActive = true, Email = "test"}
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(cvUsers.BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(x => x.Id == 1 && x.FullName == "Doe John" && x.FullNameRu == "Иванов Иван");
    }

    [Fact]
    public async Task HandleWhenUserIsCtoReturnsUsersForUser()
    {
        // Arrange
        const int userId1 = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, FirstName = "John1", LastName = "Doe", FirstNameRu = "Иван1", LastNameRu = "Иванов", IsActive = true, Email = "test" },
            new() { UserId = 2, FirstName = "John2", LastName = "Doe", FirstNameRu = "Иван2", LastNameRu = "Иванов", IsActive = true, Email = "test" }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["CTO"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId1);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(cvUsers.BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == 1 && x.FullName == "Doe John1" && x.FullNameRu == "Иванов Иван1");
        result.Should().Contain(x => x.Id == 2 && x.FullName == "Doe John2" && x.FullNameRu == "Иванов Иван2");
    }

    [Fact]
    public async Task HandleWhenNoUsersFoundReturnsEmptyList()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(new List<CvUserEntity>().BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenNoActiveUsersFoundReturnsEmptyList()
    {
        // Arrange
        const int userId = 1;
        var cvUsers = new List<CvUserEntity>
        {
            new() { UserId = 1, FirstName = "John", LastName = "Doe", IsActive = false, Email = "test" }
        };
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.CvUsers).Returns(cvUsers.BuildMockDbSet().Object);

        var query = new GetCvUsersLookupRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
    }
}
