using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Attendance.Requests.Attendances.Requests;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Attendance;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;

namespace Modsen.Portal.Attendance.UnitTests.Attendances.Requests;

public class GetUserAttendancesRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<IAttendanceDbContext> _attendanceDbContextMock;
    private readonly GetUserAttendancesRequest _handler;
    private readonly CancellationToken _cancellationToken;
    private readonly ICollection<UserEntity> _usersSet;
    private readonly ICollection<AttendanceEntity> _attendancesSet;

    public GetUserAttendancesRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _attendanceDbContextMock = new Mock<IAttendanceDbContext>();
        _cancellationToken = CancellationToken.None;
        _handler = new GetUserAttendancesRequest(
            _attendanceDbContextMock.Object,
            _authenticatedUserContextMock.Object,
            new AttendanceTotalCalculator());

        _usersSet =
        [
            new UserEntity
            {
                UserId = 1,
                FirstName = "Employee",
                LastName = "One",
                Email = "employee.one@modsen-software.com",
                DepartmentId = "Net",
                ResourceGroupId = 1,
                ResourceGroup = new ResourceGroupEntity
                {
                    ResourceGroupId = 1,
                    Name = "G1",
                },
                IsActive = true,
            },
            new UserEntity
            {
                UserId = 2,
                FirstName = "Employee",
                LastName = "Two",
                Email = "employee.two@modsen-software.com",
                DepartmentId = "Net",
                ResourceGroupId = 2,
                ResourceGroup = new ResourceGroupEntity
                {
                    ResourceGroupId = 2,
                    Name = "G2",
                },
                IsActive = true,
            },
            new UserEntity
            {
                UserId = 3,
                FirstName = "Employee",
                LastName = "Three",
                Email = "employee.three@modsen-software.com",
                DepartmentId = "Java",
                ResourceGroupId = 3,
                ResourceGroup = new ResourceGroupEntity
                {
                    ResourceGroupId = 3,
                    Name = "G3",
                },
                IsActive = true,
            },
        ];

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
                User = _usersSet.First(x => x.UserId == 1),
            },
            new AttendanceEntity
            {
                Id = 2,
                Door = "Door 2",
                Location = LocationType.Minsk,
                Direction = DirectionType.Exit,
                CheckedAtUtc = DateTime.UtcNow.AddMinutes(10),
                UserId = 1,
                User = _usersSet.First(x => x.UserId == 1),
            },
            new AttendanceEntity
            {
                Id = 3,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 2,
                User = _usersSet.First(x => x.UserId == 2),
            },
            new AttendanceEntity
            {
                Id = 4,
                Door = "Door 1",
                Location = LocationType.Minsk,
                Direction = DirectionType.Enter,
                CheckedAtUtc = DateTime.UtcNow,
                UserId = 3,
                User = _usersSet.First(x => x.UserId == 3),
            },
        ];

        _attendanceDbContextMock.Setup(x => x.Users).Returns(_usersSet.BuildMockDbSet().Object);
        _attendanceDbContextMock.Setup(x => x.Attendances).Returns(_attendancesSet.BuildMockDbSet().Object);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    public async Task HandleWhenAuthUserIsChiefReturnsRequestedUserAttendances(string role)
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([role]);
        var request = new GetUserAttendancesRequest.Query(3, CreateODataQueryOptions());

        var result = await _handler.Handle(request, _cancellationToken);

        result.Items.Should().HaveCount(1);
        result.Items.Should().OnlyContain(x => x.Location == LocationType.Minsk.ToString());
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenAuthUserRequestsSelfReturnsOwnAttendances()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        var request = new GetUserAttendancesRequest.Query(1, CreateODataQueryOptions());

        var result = await _handler.Handle(request, _cancellationToken);

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentRequestsUserFromSameDepartmentReturnsAttendances()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        var request = new GetUserAttendancesRequest.Query(2, CreateODataQueryOptions());

        var result = await _handler.Handle(request, _cancellationToken);

        result.Items.Should().HaveCount(1);
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenHeadOfDepartmentRequestsUserFromAnotherDepartmentThrowsActionNotAllowedException()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        var request = new GetUserAttendancesRequest.Query(3, CreateODataQueryOptions());

        var action = () => _handler.Handle(request, _cancellationToken);

        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.HeadDoNotHaveAccessToOtherDepartments);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsUserFromSameResourceGroupReturnsAttendances()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        var request = new GetUserAttendancesRequest.Query(1, CreateODataQueryOptions());

        var result = await _handler.Handle(request, _cancellationToken);

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsUserFromAnotherResourceGroupThrowsActionNotAllowedException()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns("Net");
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(1);
        var request = new GetUserAttendancesRequest.Query(2, CreateODataQueryOptions());

        var action = () => _handler.Handle(request, _cancellationToken);

        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.GroupManagerDoNotHaveAccessToOtherGroup);
    }

    [Fact]
    public async Task HandleWhenEmployeeRequestsOtherUserThrowsActionNotAllowedException()
    {
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Employee]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);
        var request = new GetUserAttendancesRequest.Query(2, CreateODataQueryOptions());

        var action = () => _handler.Handle(request, _cancellationToken);

        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.CannotAccessOtherUserData);
    }

    private static ODataQueryOptions<OdpUserAttendanceRow> CreateODataQueryOptions(
        IReadOnlyDictionary<string, string?>? extra = null)
    {
        var q = new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0",
        };
        if (extra is not null)
        {
            foreach (var kv in extra)
            {
                q[kv.Key] = kv.Value;
            }
        }

        return ODataQueryOptionsMock.BuildMock<OdpUserAttendanceRow>(q);
    }
}
