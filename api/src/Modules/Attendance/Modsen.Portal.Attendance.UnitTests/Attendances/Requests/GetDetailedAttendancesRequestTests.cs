using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Requests;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;

namespace Modsen.Portal.Attendance.UnitTests.Attendances.Requests;

public class GetDetailedAttendancesRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<IAttendanceDbContext> _attendancesDbContextMock;
    private readonly GetDetailedAttendancesRequest _handler;
    private readonly CancellationToken _cancellationToken;
    private readonly ICollection<AttendanceEntity> _attendancesSet;

    public GetDetailedAttendancesRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _attendancesDbContextMock = new Mock<IAttendanceDbContext>();
        _cancellationToken = CancellationToken.None;
        _handler = new GetDetailedAttendancesRequest(_attendancesDbContextMock.Object,
            _authenticatedUserContextMock.Object);

        _attendancesSet =
        [
            new AttendanceEntity
            {
                Id = 1,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 1,
                User = new UserEntity
                {
                    UserId = 1,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test.test@modsen-software.com",
                    FirstNameRu = "Тест",
                    LastNameRu = "Тест",
                    DepartmentId = null,
                    ResourceGroupId = null,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 2,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 2,
                User = new UserEntity
                {
                    UserId = 2,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test.test@modsen-software.com",
                    FirstNameRu = "Тест",
                    LastNameRu = "Тест",
                    DepartmentId = "Java",
                    ResourceGroupId = null,
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 3,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 3,
                User = new UserEntity
                {
                    UserId = 3,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test.test@modsen-software.com",
                    FirstNameRu = "Тест",
                    LastNameRu = "Тест",
                    DepartmentId = ".Net",
                    ResourceGroupId = 1,
                    ResourceGroup = new ResourceGroupEntity
                    {
                        ResourceGroupId = 1,
                        Name = "G1"
                    },
                    IsActive = true,
                }
            },
            new AttendanceEntity
            {
                Id = 4,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 4,
                User = new UserEntity
                {
                    UserId = 4,
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "test.test@modsen-software.com",
                    FirstNameRu = "Тест",
                    LastNameRu = "Тест",
                    DepartmentId = ".Net",
                    ResourceGroupId = 2,
                    ResourceGroup = new ResourceGroupEntity
                    {
                        ResourceGroupId = 2,
                        Name = "G2"
                    },
                    IsActive = true,
                }
            }
        ];

        _attendancesDbContextMock.Setup(x => x.Attendances).Returns(_attendancesSet.BuildMockDbSet().Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenAuthUserIsCeoOrCtoReturnsAllAttendances(string role)
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        var query = new GetDetailedAttendancesRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(4);
        result.Items.Should().Contain(x => x.UserDepartmentId == "Java");
        result.Items.Should().Contain(x => x.UserDepartmentId == ".Net");
        result.Items.Should().Contain(x => x.UserDepartmentId == null);
        result.Total.Should().Be(4);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleWhenAuthUserIsHeadOfDepartmentReturnsAttendancesForDepartmentUsers()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(".Net");
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        var query = new GetDetailedAttendancesRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(x => x.UserDepartmentId == ".Net");
        result.Items.Should().Contain(x => x.UserResourceGroupName == "G1");
        result.Items.Should().Contain(x => x.UserResourceGroupName == "G2");
        result.Items.Should().NotContain(x => x.UserDepartmentId == "Java");
        result.Items.Should().NotContain(x => x.UserDepartmentId == null);
        result.Total.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task HandleWhenAuthUserIsGroupManagerReturnsAttendancesForDepartmentUsers()
    {
        // Arrange
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(".Net");
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        var query = new GetDetailedAttendancesRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().Contain(x => x.UserDepartmentId == ".Net");
        result.Items.Should().Contain(x => x.UserResourceGroupName == "G1");
        result.Items.Should().NotContain(x => x.UserResourceGroupName == "G2");
        result.Items.Should().NotContain(x => x.UserDepartmentId == "Java");
        result.Items.Should().NotContain(x => x.UserDepartmentId == null);
        result.Total.Should().Be(1);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    private static ODataQueryOptions<OdpAttendance> CreateODataQueryOptions(Dictionary<string, string?> queryOptions)
    {
        return ODataQueryOptionsMock.BuildMock<OdpAttendance>(queryOptions);
    }
}
