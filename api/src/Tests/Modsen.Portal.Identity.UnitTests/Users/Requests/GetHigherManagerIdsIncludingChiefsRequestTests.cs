using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetHigherManagerIdsIncludingChiefsRequestTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLoader> _userLoaderMock = new();
    private readonly GetHigherManagerIdsIncludingChiefsRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetHigherManagerIdsIncludingChiefsRequestTests()
    {
        _handler = new GetHigherManagerIdsIncludingChiefsRequest(
            _identityDbContextMock.Object,
            _userLoaderMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int userId = 1;

        _userLoaderMock
            .Setup(x => x.LoadAsync(1, _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException($"User with id {userId} does not exist"));

        // Act
        var actionUnderTest = () => _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(userId),
            _cancellationToken);

        // Assert
        await actionUnderTest.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenUserIsInDepartmentShouldReturnHeadOfDepartmentAndReportingChief(string role)
    {
        // Arrange
        var employeeRole = new Role { Name = role, PublicName = "Test" };
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };
        var ctoRole = new Role { Id = 55, Name = PortalRoleNames.Cto, PublicName = "Test" };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [role],
            DepartmentId = "Net"
        };

        var user = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = employeeRole }],
            DepartmentId = "Net"
        };

        var head = new User
        {
            Id = 2,
            FirstName = "managerName",
            LastName = "test",
            Email = "manager@example.com",
            IsActive = true,
            UserRoles = [new() { Role = headRole }],
            DepartmentId = "Net"
        };

        var cto = new User
        {
            Id = 3,
            FirstName = "ctoName",
            LastName = "test",
            Email = "ctoName@example.com",
            IsActive = true,
            UserRoles = [new() { RoleId = ctoRole.Id}],
            DepartmentId = "Net"
        };

        var department = new Department
        {
            Id = "Net",
            FullName = ".NET",
            ShortName = "Net",
            DepartmentReportingToRoleId = ctoRole.Id
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user, head, cto }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        _identityDbContextMock
            .SetupGet(x => x.Departments)
            .Returns(new List<Department> { department }.BuildMockDbSet().Object);

        int[] ids = [head.Id, cto.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(user.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task HandleWhenUserIsInDepartmentAndGroupShouldReturnGroupManagerIdAndHeadOfDepartmentAndReportingChief(string role)
    {
        // Arrange
        var employeeRole = new Role { Name = role , PublicName = "Test" };
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };
        var gmRole = new Role { Name = PortalRoleNames.GroupManager, PublicName = "Test" };
        var ctoRole = new Role { Id = 55, Name = PortalRoleNames.Cto, PublicName = "Test" };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [role],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var user = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = employeeRole }],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var groupManager = new User
        {
            Id = 2,
            FirstName = "groupManagerName",
            LastName = "test",
            Email = "groupManager@example.com",
            IsActive = true,
            UserRoles = [new() { Role = gmRole }],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var head = new User
        {
            Id = 3,
            FirstName = "managerName",
            LastName = "test",
            Email = "manager@example.com",
            IsActive = true,
            UserRoles = [new() { Role = headRole }],
            DepartmentId = "Net"
        };

        var cto = new User
        {
            Id = 4,
            FirstName = "ctoName",
            LastName = "test",
            Email = "ctoName@example.com",
            IsActive = true,
            UserRoles = [new() { RoleId = ctoRole.Id }],
            DepartmentId = "Net"
        };

        var department = new Department
        {
            Id = "Net",
            FullName = ".NET",
            ShortName = "Net",
            DepartmentReportingToRoleId = ctoRole.Id
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user, head, groupManager, cto }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        _identityDbContextMock
            .SetupGet(x => x.Departments)
            .Returns(new List<Department> { department }.BuildMockDbSet().Object);

        int[] ids = [groupManager.Id, head.Id, cto.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(user.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerShouldReturnHeadOfDepartmentAndReportingChief()
    {
        // Arrange
        var employeeRole = new Role { Name = PortalRoleNames.GroupManager, PublicName = "Test" };
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };
        var ctoRole = new Role { Id = 55, Name = PortalRoleNames.Cto, PublicName = "Test" };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [employeeRole.Name],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var user = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = employeeRole }],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var head = new User
        {
            Id = 2,
            FirstName = "managerName",
            LastName = "test",
            Email = "manager@example.com",
            IsActive = true,
            UserRoles = [new() { Role = headRole }],
            DepartmentId = "Net"
        };

        var cto = new User
        {
            Id = 3,
            FirstName = "ctoName",
            LastName = "test",
            Email = "ctoName@example.com",
            IsActive = true,
            UserRoles = [new() { RoleId = ctoRole.Id }],
            DepartmentId = "Net"
        };

        var department = new Department
        {
            Id = "Net",
            FullName = ".NET",
            ShortName = "Net",
            DepartmentReportingToRoleId = ctoRole.Id
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user, head, cto }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        _identityDbContextMock
            .SetupGet(x => x.Departments)
            .Returns(new List<Department> { department }.BuildMockDbSet().Object);

        int[] ids = [head.Id, cto.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(user.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentShouldReturnCtoId()
    {
        // Arrange
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };
        var ctoRole = new Role { Id = 55, Name = PortalRoleNames.Cto, PublicName = "Test" };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [headRole.Name],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var user = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = headRole }],
            DepartmentId = "Net"
        };

        var cto = new User
        {
            Id = 2,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { RoleId = ctoRole.Id, Role = ctoRole }],
        };

        var department = new Department
        {
            Id = "Net",
            FullName = ".NET",
            ShortName = "Net",
            DepartmentReportingToRoleId = ctoRole.Id
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user, cto }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        _identityDbContextMock
            .SetupGet(x => x.Departments)
            .Returns(new List<Department> { department }.BuildMockDbSet().Object);

        int[] ids = [cto.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(user.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task HandleWhenUserIsCtoShouldReturnCeoId()
    {
        // Arrange
        var ctoRole = new Role { Name = PortalRoleNames.Cto, PublicName = "Test" };
        var ceoRole = new Role { Name = PortalRoleNames.Ceo, PublicName = "Test" };

        var userDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [ctoRole.Name],
            DepartmentId = "Net",
            ResourceGroupId = 1,
        };

        var user = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = ctoRole }],
            DepartmentId = "Net"
        };

        var ceo = new User
        {
            Id = 2,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = ceoRole }],
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user, ceo }.BuildMockDbSet().Object);

        _userLoaderMock
            .Setup(x => x.LoadAsync(userDto.Id, _cancellationToken))
            .ReturnsAsync(userDto);

        int[] ids = [ceo.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(user.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }

    [Fact]
    public async Task HandleWhenUserIsCeoShouldReturnCeo()
    {
        // Arrange
        var ceoRole = new Role { Name = PortalRoleNames.Ceo, PublicName = "Test" };

        var ceoDto = new UserDto
        {
            Id = 1,
            FirstName = "FirstName",
            Email = "example@example.com",
            IsActive = true,
            RoleNames = [ceoRole.Name],
            DepartmentId = null,
            ResourceGroupId = null,
        };

        var ceo = new User
        {
            Id = 1,
            FirstName = "FirstName",
            LastName = "test",
            Email = "example@example.com",
            IsActive = true,
            UserRoles = [new() { Role = ceoRole }],
        };

        _userLoaderMock
            .Setup(x => x.LoadAsync(ceoDto.Id, _cancellationToken))
            .ReturnsAsync(ceoDto);

        int[] ids = [ceo.Id];

        // Act
        var result = await _handler.Handle(
            new GetHigherManagerIdsIncludingChiefsRequest.Query(ceo.Id),
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(ids);
    }
}
