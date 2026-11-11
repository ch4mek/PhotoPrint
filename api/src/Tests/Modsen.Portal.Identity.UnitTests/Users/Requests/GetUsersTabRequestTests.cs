using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TimeTrack.Contract;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUsersTabRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();
    private readonly Mock<ITimeTrackContract> _timeTrackContractMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetUsersTabRequest _handler;

    public GetUsersTabRequestTests()
    {
        _handler = new GetUsersTabRequest(
            _authenticatedUserContextMock.Object,
            _identityDbContextMock.Object,
            _timeTrackContractMock.Object
            );
    }

    [Fact]
    public async Task HandleShouldReturnAllUsersWhenUserHasManageHrClaim()
    {
        // Arrange
        const int user1Id = 1;
        const int user2Id = 2;
        const int user3Id = 3;
        const string departmentId1 = "dept1";
        const string departmentId2 = "dept2";
        const int resourceGroupId1 = 1;
        const int resourceGroupId2 = 2;

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserCoreDto>(queryParameters);
        var request = new GetUsersTabRequest.Query(queryOptions);

        var users = new List<User>
        {
            new()
            {
                Id = user1Id,
                FirstName = "User1",
                LastName = "User1",
                DepartmentId = departmentId1,
                ResourceGroupId = resourceGroupId1,
                UserRoles = new List<UserRole>
                {
                    new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Employee" } }
                }
            },
            new()
            {
                Id = user2Id,
                FirstName = "User2",
                LastName = "User2",
                DepartmentId = departmentId2,
                ResourceGroupId = resourceGroupId2,
                UserRoles = new List<UserRole>
                {
                    new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Employee" } }
                }
            },
            new()
            {
                Id = user3Id,
                FirstName = "User3",
                LastName = "User3",
                DepartmentId = departmentId1,
                ResourceGroupId = resourceGroupId1,
                UserRoles = new List<UserRole>
                {
                    new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Employee" } }
                }
            }
        };

        const string authenticatedUserDepartmentId = "other-dept";
        const int authenticatedUserResourceGroupId = 999;

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(authenticatedUserDepartmentId);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
            .Returns(authenticatedUserResourceGroupId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
            .Returns(true);

        var dbSetMock = users.BuildMockDbSet();
        _identityDbContextMock.Setup(x => x.Users)
            .Returns(dbSetMock.Object);

        var expectedOutput = new List<UserCoreDto>
        {
            CreateUserCoreDto(users[0]),
            CreateUserCoreDto(users[1]),
            CreateUserCoreDto(users[2])
        }.ToPageable()
            .WithPaging(1, 50)
            .WithTotal(3);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedOutput);
        _identityDbContextMock.Verify(x => x.Users, Times.Once);
    }

    [Fact]
    public async Task HandleShouldReturnOnlyDepartmentUsersWhenUserIsHeadOfDepartmentWithoutManageHrClaim()
    {
        // Arrange
        const int user1Id = 1;
        const int user2Id = 2;
        const string departmentId1 = "dept1";
        const string departmentId2 = "dept2";
        const int resourceGroupId1 = 1;
        const int resourceGroupId2 = 2;

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserCoreDto>(queryParameters);
        var request = new GetUsersTabRequest.Query(queryOptions);

        var users = new List<User>
        {
            new()
            {
                Id = user1Id,
                FirstName = "User1",
                LastName = "User1",
                DepartmentId = departmentId1,
                ResourceGroupId = resourceGroupId1,
                UserRoles = new List<UserRole>
                {
                    new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Employee" } }
                }
            },
            new()
            {
                Id = user2Id,
                FirstName = "User2",
                LastName = "User2",
                DepartmentId = departmentId2,
                ResourceGroupId = resourceGroupId2,
                UserRoles = new List<UserRole>
                {
                    new() { Role = new Role { Name = PortalRoleNames.Employee, PublicName = "Employee" } }
                }
            }
        };

        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId1);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.IsHeadOfDepartment()).Returns(true);
        _authenticatedUserContextMock.Setup(x => x.HasClaim(HrClaimTypeNames.ManageHrModule.Name)).Returns(false);

        var dbSetMock = users.BuildMockDbSet();
        _identityDbContextMock.Setup(x => x.Users)
            .Returns(dbSetMock.Object);

        var expectedOutput = new List<UserCoreDto>
        {
            CreateUserCoreDto(users[0])
        }.ToPageable()
            .WithPaging(1, 50)
            .WithTotal(1);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedOutput);
        _identityDbContextMock.Verify(x => x.Users, Times.Once);
    }

    private static UserCoreDto CreateUserCoreDto(User user)
    {
        return new UserCoreDto
        {
            Id = user.Id,
            IsActive = user.IsActive,
            Color = user.Color,
            AvatarColor = user.AvatarColor,
            AvatarFileId = user.AvatarFileId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.LastName + " " + user.FirstName,
            FirstNameRu = user.FirstNameRu,
            LastNameRu = user.LastNameRu,
            FullNameRu = user.LastNameRu == null || user.FirstNameRu == null ? null : user.LastNameRu + " " + user.FirstNameRu,
            CombinedFullName = user.LastNameRu == null && user.FirstNameRu == null
                ? user.LastName + " " + user.FirstName
                : user.LastNameRu != null && user.FirstNameRu != null
                    ? user.LastNameRu + " " + user.FirstNameRu + " (" + user.LastName + " " + user.FirstName + ")"
                    : (user.LastNameRu ?? user.FirstNameRu) + " (" + user.LastName + " " + user.FirstName + ")",

            Email = user.Email!,
            TimeZone = user.TimeZone,
            DepartmentId = user.DepartmentId,
            DepartmentFullName = user.Department?.FullName,
            ResourceGroupId = user.ResourceGroupId,
            ResourceGroupName = user.ResourceGroup?.IsActive == true ? user.ResourceGroup.Name : null,
            Location = user.Location,
            RegisteredLocationId = user.RegisteredLocationId,
            Position = user.UserRoles.First().Role!.Name!,
            PositionPublicName = user.UserRoles.First().Role!.PublicName,
            PositionOrderWeight = user.UserRoles.First().Role!.OrderWeight
        };
    }
}
