using System.Linq.Expressions;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUserLookupRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<IUserLookupLoader> _userLookupLoaderMock = new();
    private readonly CancellationToken _cancellationToken;
    private readonly GetUserLookupRequest _handler;

    public GetUserLookupRequestTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new GetUserLookupRequest(
            _authenticatedUserContextMock.Object,
            _userLookupLoaderMock.Object,
            _identityDbContextMock.Object);
    }

    [Theory]
    [InlineData("name", true, "Net")]
    [InlineData(null, false, "Js")]
    [InlineData(null, null, null)]
    public async Task GetUsersLookupWhenRolesCollectionIsEmptyAndUserIsHeadOdDepartmentShouldReturnLookupWithDefinedDepartmentId(
        string? keyWord,
        bool? isActive,
        string? departmentId)
    {
        //Arrange
        List<string> roles = [];

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                IsCompanyManager = false,
                Name = PortalRoleNames.Employee,
                PublicName = "Test"
            },
        };

        var users = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                Color = "#FFFFFF",
                PositionDescription = PortalRoleNames.Employee
            },
            new()
            {
                Id = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                Color = "#FFFFFF",
                PositionDescription = PortalRoleNames.Employee
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> dbRequestMatch = x =>
            x.KeyWord == keyWord
            && x.Roles.Count == 0
            && x.DepartmentId == departmentId
            && x.IsActive == isActive;

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);

        _identityDbContextMock.Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _userLookupLoaderMock.Setup(x =>
                x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(users);

        //Act
        var result = await _handler.Handle(
            new GetUserLookupRequest.Query(keyWord, isActive, roles), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        _userLookupLoaderMock.Verify(x =>
            x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken), Times.Once);

        result.Count.Should().Be(2);
    }

    [Theory]
    [InlineData("name", true)]
    [InlineData(null, false)]
    [InlineData(null, null)]
    public async Task GetUsersLookupWhenRolesCollectionIsNotEmptyShouldReturnLookupWithDepartmentIdEqualToNull(
        string? keyWord,
        bool? isActive)
    {
        //Arrange
        List<string> roles = [PortalRoleNames.Ceo, PortalRoleNames.Cto];

        var dbRoles = new List<Role>
        {
            new()
            {
                Id = 1,
                IsCompanyManager = false,
                Name = PortalRoleNames.Employee,
                PublicName = "Test"
            },
        };

        var users = new List<UserLookupDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Anna",
                LastName = "Doyle",
                Color = "#FFFFFF",
                PositionDescription = PortalRoleNames.Employee
            },
            new()
            {
                Id = 2,
                FirstName = "Veronica",
                LastName = "Smith",
                Color = "#FFFFFF",
                PositionDescription = PortalRoleNames.Employee
            }
        };

        Expression<Func<UserLookupLoaderRequestDto, bool>> dbRequestMatch = x =>
            x.KeyWord == keyWord
            && x.Roles.Count > 0
            && x.DepartmentId == null
            && x.IsActive == isActive;

        _identityDbContextMock.Setup(x => x.Roles)
            .Returns(dbRoles.BuildMockDbSet().Object);

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(It.IsNotNull<string>());

        _userLookupLoaderMock.Setup(x =>
                x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(users);

        //Act
        var result = await _handler.Handle(
            new GetUserLookupRequest.Query(keyWord, isActive, roles), _cancellationToken);

        //Assert
        result.Should().NotBeNull();

        _userLookupLoaderMock.Verify(x =>
            x.LoadAsync(It.Is(dbRequestMatch), _cancellationToken), Times.Once);

        result.Count.Should().Be(2);
    }
}
