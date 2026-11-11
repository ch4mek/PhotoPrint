using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.ResourceGroups.Core.BusinessRules;

public class CanUserUpdateGroupBusinessRuleTests
{
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly CanUserUpdateGroupBusinessRule _canUserUpdateGroupBusinessRule;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public CanUserUpdateGroupBusinessRuleTests()
    {
        _canUserUpdateGroupBusinessRule = new(
            _identityDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenDepartmentIsNotExistShouldReturnFail()
    {
        // Arrange
        var department = ".Net";

        SetupDepartments([]);

        // Act
        var result = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(new(department), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.DepartmentNotFound);
    }

    [Fact]
    public async Task HandleWhenHeadFromAnotherDepartmentShouldReturnFail()
    {
        // Arrange
        var departmentId = ".Net";
        var department = new Department
        {
            Id = departmentId,
            ShortName = departmentId,
            FullName = departmentId
        };

        SetupDepartments([department]);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);
        SetupUserDepartment("Other");

        // Act
        var result = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(new(departmentId), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(false);
        result.Reason.Should().Be(PortalMessages.HeadCannotUpdateGroupFromAnotherDepartment);
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnSuccess()
    {
        // Arrange
        var departmentId = ".Net";
        var department = new Department
        {
            Id = departmentId,
            ShortName = departmentId,
            FullName = departmentId
        };

        SetupDepartments([department]);
        SetupUserRoles([PortalRoleNames.HeadOfDepartment]);
        SetupUserDepartment(departmentId);

        // Act
        var result = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(new(departmentId), _cancellationToken);

        // Assert
        result.IsSuccess.Should().Be(true);
    }

    private void SetupDepartments(List<Department> departments)
    {
        _identityDbContextMock
            .Setup(mock => mock.Departments)
            .Returns(departments.BuildMockDbSet().Object);
    }

    private void SetupUserRoles(string[] roles)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.GetRoles())
            .Returns(roles);
    }

    private void SetupUserDepartment(string departmentId)
    {
        _authenticatedUserContextMock
            .Setup(mock => mock.DepartmentId)
            .Returns(departmentId);
    }
}
