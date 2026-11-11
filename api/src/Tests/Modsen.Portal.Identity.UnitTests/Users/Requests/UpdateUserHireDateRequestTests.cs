using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class UpdateUserHireDateRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IIdentityDbContext> _identityDbContext = new();
    private readonly Mock<ICommunicationBus> _communicationBus = new();
    private readonly Mock<IUserLoader> _userLoader = new();

    private readonly UpdateUserHireDateRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Mock<DbSet<User>> _userDbSet;

    public UpdateUserHireDateRequestTests()
    {
        _handler = new(_identityDbContext.Object, _communicationBus.Object,
            _authenticatedUserContext.Object, _userLoader.Object);
        _userDbSet = _users.BuildMockDbSet();
    }

    private readonly User[] _users =
    [
        new User
        {
            Id = 1,
            DepartmentId = "dep 1",
            ResourceGroupId = 1,
            FirstName = "firstname",
            LastName = "lastname",
            IsActive = true,
            HireDate = null,
            UserRoles = [new UserRole { UserId = 1, Role = new Role { PublicName = "role" } }]
        },
        new User
        {
            Id = 2,
            DepartmentId = "dep 2",
            ResourceGroupId = 2,
            FirstName = "firstname2",
            LastName = "lastname2",
            IsActive = true,
            HireDate = null,
            UserRoles = [new UserRole { UserId = 1, Role = new Role { PublicName = "role" } }]
        }
    ];
        
    [Fact]
    public async Task UpdateHireDateWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        _identityDbContext
            .Setup(x => x.Users)
            .Returns(_userDbSet.Object);

        var userId = 3;
        var request = new UpdateUserHireDateRequest.Command(userId, It.IsAny<DateOnly>());

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task UpdateHireDateWhenUserNotFromDepartmentThrowActionNotAllowedException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        _identityDbContext
            .Setup(x => x.Users)
            .Returns(_userDbSet.Object);

        var hodClaim = new Claim(ClaimTypes.Role, PortalRoleNames.HeadOfDepartment);

        _authenticatedUserContext
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContext
            .Setup(x => x.DepartmentId)
            .Returns("dep 3");

        var userId = 2;
        var request = new UpdateUserHireDateRequest.Command(userId, It.IsAny<DateOnly>());

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task UpdateHireDateWhenUserNotFromGroupThrowActionNotAllowedException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        _identityDbContext
            .Setup(x => x.Users)
            .Returns(_userDbSet.Object);

        _authenticatedUserContext
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContext
            .Setup(x => x.ResourceGroupId)
            .Returns(3);

        var userId = 2;
        var request = new UpdateUserHireDateRequest.Command(userId, It.IsAny<DateOnly>());

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task UpdateHireDateWhenRequestIsValidShouldReturnHireDate()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        _identityDbContext
            .Setup(x => x.Users)
            .Returns(_userDbSet.Object);

        _authenticatedUserContext
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        var userId = 2;

        var hireDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var userEntity = _userDbSet.Object.First();

        var userDto = new Identity.Requests.Dto.UserDto
        {
            Id = 2,
            DepartmentId = "dep 2",
            ResourceGroupId = 2,
            FirstName = "firstname2",
            LastName = "lastname2",
            IsActive = true,
            HireDate = hireDate,
            Email = "email"
        };

        _userLoader
            .Setup(x => x.LoadAsync(userId, _cancellationToken))
            .ReturnsAsync(userDto);

        var request = new UpdateUserHireDateRequest.Command(userId, It.IsAny<DateOnly>());

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().Be(hireDate);
        _identityDbContext.Verify(x => x.SaveChanges(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateHireDateWhenUserUpdatesThemselfThrowActionNotAllowedException()
    {
        // Arrange
        var initiatotId = 1;
        _authenticatedUserContext
            .Setup(x => x.GetRequiredUserId())
            .Returns(initiatotId);

        var request = new UpdateUserHireDateRequest.Command(initiatotId, It.IsAny<DateOnly>());

        // Act
        var act = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await act.Should().ThrowAsync<ActionNotAllowedException>("You are not allowed to edit your own hire date");
    }
}
