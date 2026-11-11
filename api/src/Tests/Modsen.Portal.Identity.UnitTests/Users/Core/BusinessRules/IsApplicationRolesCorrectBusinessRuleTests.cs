using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Core.BusinessRules;

public class IsApplicationRolesCorrectBusinessRuleTests
{
    private readonly Mock<ILogger<IsApplicationRolesCorrectBusinessRule>> _loggerMock = new();
    private readonly Mock<IIdentityPortalSettingsAccessor> _settingsAccessor = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    public IsApplicationRolesCorrectBusinessRuleTests()
    {
        _settingsAccessor
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityPortalSettingsDto
            {
                MaxCeoCount = 1,
                MaxCtoCount = 1,
                MaxHeadOfSpecificDepartmentCount = 1
            });
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCTOUsersMoreThanOneShouldReturnFalse()
    {
        //Assert
        var ctoRole = new Role { Name = PortalRoleNames.Cto, PublicName = "Test" };

        var users = new List<User>
        {
            new() {
                UserRoles = [new UserRole { Role = ctoRole }],
                IsActive = true,
                FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = ctoRole }],
                IsActive = true,FirstName = "test",
                LastName = "test"
            }
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        var rule = new IsApplicationRolesCorrectBusinessRule(_loggerMock.Object, _settingsAccessor.Object, _identityDbContextMock.Object);

        //Act
        var result = await rule.IsCorrespondsToBusiness(null, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenCEOUsersMoreThanOneShouldReturnFalse()
    {
        //Assert
        var ctoRole = new Role { Name = PortalRoleNames.Cto, PublicName = "Test" };
        var ceoRole = new Role { Name = PortalRoleNames.Ceo, PublicName = "Test" };

        var users = new List<User>
        {
            new() {
                UserRoles = [new UserRole { Role = ctoRole }],
                IsActive = true,FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = ceoRole }],
                IsActive = true,FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = ceoRole }],
                IsActive = true,FirstName = "test",
                LastName = "test"
            }
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        var rule = new IsApplicationRolesCorrectBusinessRule(_loggerMock.Object, _settingsAccessor.Object, _identityDbContextMock.Object);

        //Act
        var result = await rule.IsCorrespondsToBusiness(null, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenHeadOfUniqueDepartmentUsersMoreThanOneShouldReturnFalse()
    {
        //Assert
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };

        var users = new List<User>
        {
            new() {
                UserRoles = [new UserRole { Role = headRole }],
                IsActive = true,
                DepartmentId = ".Net",
                FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = headRole }],
                IsActive = true,
                DepartmentId = ".Net",
                FirstName = "test",
                LastName = "test"
            }
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        var rule = new IsApplicationRolesCorrectBusinessRule(_loggerMock.Object, _settingsAccessor.Object, _identityDbContextMock.Object);

        //Act
        var result = await rule.IsCorrespondsToBusiness(null, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenAllRolesAreCorrectShouldReturnTrue()
    {
        //Assert
        var ctoRole = new Role { Name = PortalRoleNames.Cto, PublicName = "Test" };
        var ceoRole = new Role { Name = PortalRoleNames.Ceo, PublicName = "Test" };
        var headRole = new Role { Name = PortalRoleNames.HeadOfDepartment, PublicName = "Test" };

        var users = new List<User>
        {
            new() {
                UserRoles = [new UserRole { Role = ctoRole }],
                IsActive = true,
                FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = ceoRole }],
                IsActive = true,
                FirstName = "test",
                LastName = "test"
            },
            new() {
                UserRoles = [new UserRole { Role = headRole }],
                IsActive = true,
                DepartmentId = ".Net",
                FirstName = "test",
                LastName = "test"
            }
        };

        _identityDbContextMock
            .Setup(x => x.Users)
            .Returns(users.BuildMockDbSet().Object);

        var rule = new IsApplicationRolesCorrectBusinessRule(_loggerMock.Object, _settingsAccessor.Object, _identityDbContextMock.Object);

        //Act
        var result = await rule.IsCorrespondsToBusiness(null, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }
}
