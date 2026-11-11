using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances.Core.Calculators;
using Modsen.Portal.Attendance.Requests.Attendances.Requests;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;

namespace Modsen.Portal.Attendance.UnitTests.Attendances.Requests;

public class GetOdpAttendancesTotalRequestTests
{
    private readonly GetOdpAttendancesTotalRequest _handler;
    private readonly Mock<IAttendanceDbContext> _attendanceDbContext = new();
    private readonly Mock<IAttendanceTotalCalculator> _attendanceTotalCalculator = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetOdpAttendancesTotalRequestTests()
    {
        _handler = new GetOdpAttendancesTotalRequest(_attendanceDbContext.Object, _attendanceTotalCalculator.Object,
            _authenticatedUserContext.Object);
    }

    private readonly List<OdpAttendanceTotal> _attendancesTotal =
    [
        new OdpAttendanceTotal()
        {
            UserId = 1,
            UserDepartmentId = "Net",
            UserResourceGroupId = null,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TotalTimeInMinutes = 10,
        },
        new OdpAttendanceTotal()
        {
            UserId = 2,
            UserDepartmentId = "Net",
            UserResourceGroupId = 1,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TotalTimeInMinutes = 20,
        },
        new OdpAttendanceTotal()
        {
            UserId = 3,
            UserDepartmentId = "Java",
            UserResourceGroupId = null,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            TotalTimeInMinutes = 30,
        },
    ];

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnValid()
    {
        // Arrange
        var attendances = new List<AttendanceEntity>();
        _attendanceDbContext.SetupGet(x => x.Attendances).Returns(attendances.BuildMockDbSet().Object);
        _authenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.Cto]);

        _attendanceTotalCalculator.Setup(x => x.Calculate(attendances))
            .Returns(_attendancesTotal);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpAttendanceTotal>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        });
        var query = new GetOdpAttendancesTotalRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Total.Should().Be(_attendancesTotal.Count);
        result.Items.Should().HaveCount(_attendancesTotal.Count);
    }
}
