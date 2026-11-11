using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Requests;

public class GetUsersRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IIdentityDbContext> _identityDbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetUsersRequest _handler;

    public GetUsersRequestTests()
    {
        _handler = new GetUsersRequest(
            _authenticatedUserContextMock.Object,
            _identityDbContextMock.Object);
    }

    [Theory]
    [InlineData(null, PortalRoleNames.Ceo)]
    [InlineData("Net", PortalRoleNames.Employee)]
    public async Task HandleShouldReturnPageableListOfItems(string? departmentId, string roleType)
    {
        // Arrange
        const int userId = 64;

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
            [nameof(ODataRawQueryOptions.Filter)] = $"id eq {userId}"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<UserCoreDto>(queryParameters);
        var request = new GetUsersRequest.Query(queryOptions);

        var users = new List<User>
        {
            new()
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                DepartmentId = departmentId,
                Email = "John.Doe@modsen-software.com",
                UserRoles = new List<UserRole>
                {
                    new UserRole() {Role = new Role{ Name = roleType, PublicName = roleType } }
                },
                ResourceGroupId = 1,
                ResourceGroup = new ResourceGroup
                {
                    Id = 1,
                    DepartmentId = departmentId,
                    Name = "rg1",
                    IsActive = true
                },
                Department = departmentId == null ? null : new Department
                {
                    FullName = ".Net",
                    Id = departmentId,
                    ShortName = ".Net",
                },
                RegisteredLocationId = 1,
                Location = LocationType.Warsaw
            }
        };

        var equivalentUserCoreDto = new UserCoreDto
        {
            Id = users[0].Id,
            IsActive = users[0].IsActive,
            Color = users[0].Color,
            AvatarColor = users[0].AvatarColor,
            AvatarFileId = users[0].AvatarFileId,
            FirstName = users[0].FirstName,
            LastName = users[0].LastName,
            FullName = users[0].LastName + " " + users[0].FirstName,
            FirstNameRu = users[0].FirstNameRu,
            LastNameRu = users[0].LastNameRu,
            FullNameRu = null,
            CombinedFullName = users[0].LastName + " " + users[0].FirstName,
            Email = users[0].Email!,
            TimeZone = users[0].TimeZone,
            DepartmentId = users[0].DepartmentId,
            DepartmentFullName = users[0].Department?.FullName,
            ResourceGroupId = users[0].ResourceGroupId,
            ResourceGroupName = users[0].ResourceGroup.IsActive ? users[0].ResourceGroup.Name : null,
            Location = users[0].Location,
            RegisteredLocationId = users[0].RegisteredLocationId,
            Position = users[0].UserRoles.First().Role!.Name!,
            PositionPublicName = users[0].UserRoles.First().Role!.PublicName,
            PositionOrderWeight = users[0].UserRoles.First().Role!.OrderWeight
        };

        var output = new List<UserCoreDto> { equivalentUserCoreDto }
            .ToPageable()
            .WithPaging(1, 50)
            .WithTotal(1);

        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns(departmentId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([roleType]);

        var dbSetMock = users.BuildMockDbSet();

        _identityDbContextMock.Setup(x => x.Users)
            .Returns(dbSetMock.Object);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(output);
        _identityDbContextMock.Verify(
            x => x.Users,
            Times.Once);
    }
}
