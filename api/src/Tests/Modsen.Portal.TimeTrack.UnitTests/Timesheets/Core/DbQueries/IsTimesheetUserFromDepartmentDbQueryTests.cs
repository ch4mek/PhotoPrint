using MockQueryable.Moq;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Core.DbQueries;

public class IsTimesheetUserFromDepartmentDbQueryTests
{
    [Fact]
    public async Task IsTimesheetUserFromDepartmentAsyncWhenUserByIdHasSameDepartmentReturnTrue()
    {
        //Arrange
        const int userId = 1;
        const string departmentId = "Net";

        var timesheetsDbSetMock = new List<Timesheet>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                TaskName = "Test 1",
                User = new TimeTrackUser
                {
                    UserId = userId,
                    DepartmentId = departmentId,
                    FirstName = "test", LastName = "test", Email = "test"
                }
            }
        }.BuildMockDbSet();

        var dbContextMock = new Mock<ITimeTrackDbContext>();

        dbContextMock.Setup(x => x.Timesheets)
            .Returns(timesheetsDbSetMock.Object);

        var isTimesheetUserFromDepartmentDbQuery = new IsTimesheetUserFromDepartmentDbQuery(dbContextMock.Object);

        //Act
        var result = await isTimesheetUserFromDepartmentDbQuery.IsTimesheetUserFromDepartmentAsync(userId, departmentId, CancellationToken.None);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsTimesheetUserFromDepartmentAsyncWhenUserByIdHasDifferentDepartmentReturnFalse()
    {
        //Arrange
        const int userId = 1;
        const string userDepartment = "Net";
        const string providedDepartmentId = "QA";


        var timesheetsDbSetMock = new List<Timesheet>
        {
            new()
            {
                Id = 1,
                UserId = userId,
                TaskName = "Test 1",
                User = new TimeTrackUser
                {
                    UserId = userId,
                    DepartmentId = userDepartment,
                    FirstName = "test", LastName = "test", Email = "test"
                }
            }
        }.BuildMockDbSet();

        var dbContextMock = new Mock<ITimeTrackDbContext>();

        dbContextMock.Setup(x => x.Timesheets)
            .Returns(timesheetsDbSetMock.Object);

        var isTimesheetUserFromDepartmentDbQuery = new IsTimesheetUserFromDepartmentDbQuery(dbContextMock.Object);

        //Act
        var result = await isTimesheetUserFromDepartmentDbQuery.IsTimesheetUserFromDepartmentAsync(userId, providedDepartmentId, CancellationToken.None);

        //Assert
        result.Should().BeFalse();
    }
}
