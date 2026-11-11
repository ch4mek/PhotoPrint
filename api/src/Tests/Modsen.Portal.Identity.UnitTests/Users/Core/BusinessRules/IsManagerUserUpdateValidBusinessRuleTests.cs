using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class IsUpdateUserRequestValidBusinessRuleTests
{
    private readonly IsManagerUserUpdateValidBusinessRule _businessRule;
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IDepartmentsCache> _departmentsCacheMock = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public IsUpdateUserRequestValidBusinessRuleTests()
    {
        _businessRule = new IsManagerUserUpdateValidBusinessRule(_identityDbContextMock.Object, _departmentsCacheMock.Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee, "Js", PortalRoleNames.Cto, null, PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.Employee, "Js", PortalRoleNames.Ceo, null, PortalRoleNames.Cto, null)]
    [InlineData(PortalRoleNames.Employee, "Js", PortalRoleNames.HeadOfDepartment, "Js", PortalRoleNames.Cto, null)]
    public async Task IsCorrespondsToBusinessWhenRoleIsNotAvailableReturnBusinessRuleFailResult(
    string updatingUserRole,
    string? updatingUserDepartmentId,
    string newRole,
    string? newDepartmentId,
    string currentUserRole,
    string? currentUserDepartmentId)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: "#ffffff",
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: "Minsk",
            Email: null,
            newRole,
            LocationType.Minsk,
            newDepartmentId,
            1,
            [currentUserRole],
            currentUserDepartmentId,
            HireDate: null);

        var identityUser = new UserDto
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            RoleNames = new List<string> { updatingUserRole },
            DepartmentId = updatingUserDepartmentId
        };

        var user = new User
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            UserRoles = new List<UserRole> { new() { Role = new Role { Name = newRole, PublicName = "Test" } } },
            IsActive = true,
            DepartmentId = updatingUserDepartmentId,
            ResourceGroupId = requestModel.ResourceGroupId,
        };

        var role = new Role
        {
            Name = requestModel.Position,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { role }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        _departmentsCacheMock
            .Setup(x => x.GetAllDepartments(_cancellationToken))
            .ReturnsAsync(GetAllDepartments);

        var model = new IsManagerUserUpdateValidBusinessRule.Request(
            requestModel.Position,
            requestModel.DepartmentId,
            requestModel.ResourceGroupId,
            identityUser.Position!,
            identityUser.DepartmentId);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(model, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();

        _identityDbContextMock.Verify(
            x => x.Roles,
            Times.Once);
        _identityDbContextMock.Verify(
            x => x.Users,
            Times.Once);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task IsCorrespondsToBusinessWhenCeoOrCtoTryUpdateThereDepartmentThrowNotFoundException(string role)
    {
        // Arrange
        var requestModel = new UpdateUserRequest.Command(
            10,
            FirstName: null,
            LastName: null,
            FirstNameRu: null,
            LastNameRu: null,
            Color: "#ffffff",
            CountryId: 1,
            RegisteredLocationId: PortalSharedConstants.DefaultRegisteredLocationId,
            CurrentLocation: "Minsk",
            Email: null,
            role,
            LocationType.Minsk,
            "Js",
            null,
            [role],
            "Js",
            HireDate: null);

        var identityUser = new UserDto
        {
            Id = 10,
            FirstName = "FirstName",
            Email = "email@modsen-software.com",
            RoleNames =
            [
                role
            ]
        };

        var user = new User
        {
            Id = 10,
            LastName = "LastName",
            FirstName = "FirstName",
            Email = "example@email.com",
            UserRoles = new List<UserRole> { new() { Role = new Role { Name = role, PublicName = "Test" } } },
            IsActive = true,
            DepartmentId = requestModel.DepartmentId,
            ResourceGroupId = requestModel.ResourceGroupId,
        };

        var roleEntity = new Role
        {
            Name = requestModel.Position,
            IsCompanyManager = true,
            PublicName = "Test"
        };

        _identityDbContextMock
            .Setup(x => x.Roles)
            .Returns(new List<Role> { roleEntity }.BuildMockDbSet().Object);

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(new List<User> { user }.BuildMockDbSet().Object);

        var model = new IsManagerUserUpdateValidBusinessRule.Request(
            requestModel.Position,
            requestModel.DepartmentId,
            requestModel.ResourceGroupId,
            identityUser.Position!,
            identityUser.DepartmentId);

        // Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            model,
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    private static List<DepartmentCacheRecord> GetAllDepartments()
    {
        return
        [
            GetDepartmentDataDto("Js", "Js", "JavaScript", "Js"),
            GetDepartmentDataDto("Net", "Net", ".NET", "Net"),
            GetDepartmentDataDto("Java", "Java", "Java", "Java"),
            GetDepartmentDataDto("Python", "Python", "Python", "Python"),
            GetDepartmentDataDto("QA", "QA", "QA", "QA"),
            GetDepartmentDataDto("BA", "BA", "BA", "BA"),
            GetDepartmentDataDto("HR", "HR", "HR", "HR"),
            GetDepartmentDataDto("Marketing", "Marketing", "Marketing", "Marketing"),
            GetDepartmentDataDto("Sales", "Sales", "Sales", "Sales"),
            GetDepartmentDataDto("PMO", "PMO", "PMO", "PMO"),
            GetDepartmentDataDto("Mobile", "Mobile", "Mobile", "Mobile"),
            GetDepartmentDataDto("PHP", "PHP", "PHP", "PHP"),
            GetDepartmentDataDto("DevOps", "DevOps", "DevOps", "DevOps"),
            GetDepartmentDataDto("Design", "Design", "Design", "Design"),
            GetDepartmentDataDto("Recruitment", "Recruitment", "Recruitment", "Recruitment")
        ];
    }

    private static DepartmentCacheRecord GetDepartmentDataDto(string id, string shortName, string fullName, string modsenApiValue)
    {
        return new DepartmentCacheRecord
        {
            Id = id,
            FullName = fullName,
            ModsenApiValue = modsenApiValue,
            ShortName = shortName,
            OrderWeight = 1
        };
    }
}
