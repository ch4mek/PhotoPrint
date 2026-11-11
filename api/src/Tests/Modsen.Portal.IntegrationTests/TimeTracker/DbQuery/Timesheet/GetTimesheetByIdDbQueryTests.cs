using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
namespace Modsen.Portal.IntegrationTests.TimeTracker.DbQuery.Timesheet;

[Collection("TimeTrackerCollection")]
public class GetTimesheetByIdDbQueryTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IServiceScope _serviceScope;
    private readonly IGetTimesheetDbQuery _query;

    public GetTimesheetByIdDbQueryTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serviceScope = _factory.Services.CreateScope();
        _query = _serviceScope.ServiceProvider.GetRequiredService<IGetTimesheetDbQuery>();
    }

    [Fact]
    public async Task GetByIdAsyncWhenTimesheetExistsReturnsTimesheet()
    {
        // Arrange
        using var arrangeScope = _factory.Services.CreateScope();

        var context = arrangeScope.ServiceProvider.GetRequiredService<TimeTrackDbContext>();

        var project = context
            .Projects
            .Include(x => x.ProjectUsers)
            .OrderBy(x => x.Id)
            .First();

        project
            .ProjectUsers
            .Add(new TimeTrack.DataAccess.Npgsql.DatabaseEntities.ProjectUser
            {
                UserId = _factory.EmployeeUserId!.Value
            });

        await context.SaveChangesAsync(CancellationToken.None);

        var timesheet = new TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet
        {
            TaskName = Guid.CreateVersion7().ToAlphabetic(),
            FactTrackedInMinutes = 777,
            BillableTrackedInMinutes = 0,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ReportYear = DateTime.UtcNow.Year,
            ReportMonthOfYear = DateTime.UtcNow.Month,
            ReportWeekOfYear = 10,
            ReportDayOfYear = DateTime.UtcNow.DayOfYear,
            UserId = _factory.EmployeeUserId!.Value,
            ProjectId = project.Id
        };

        await context.Timesheets.AddAsync(timesheet);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _query.ExecuteAsync(timesheet.Id, CancellationToken.None);

        // Assert
        using var assertScope = _factory.Services.CreateScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<TimeTrackDbContext>();
        var entity = await assertContext.Timesheets
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == timesheet.Id);

        var expectedResult = new TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet
        {
            Id = entity!.Id,
            ReportForDate = entity.ReportForDate,
            ReportDayOfYear = entity.ReportDayOfYear,
            ReportMonthOfYear = entity.ReportMonthOfYear,
            ReportWeekOfYear = entity.ReportWeekOfYear,
            ReportYear = entity.ReportYear,
            ProjectId = entity.ProjectId,
            Project = entity.Project,
            TaskName = entity.TaskName,
            FactTrackedInMinutes = entity.FactTrackedInMinutes,
            BillableTrackedInMinutes = entity.BillableTrackedInMinutes,
            UserId = entity.UserId,
            User = entity.User,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedByUser = entity.CreatedByUser,
            CreatedById = entity.CreatedById,
        };

        result.Id.Should().Be(expectedResult.Id);
        result.ReportForDate.Should().Be(expectedResult.ReportForDate);
        result.ProjectId.Should().Be(expectedResult.ProjectId);
        result.FactTrackedInMinutes.Should().Be(expectedResult.FactTrackedInMinutes);
        result.BillableTrackedInMinutes.Should().Be(expectedResult.BillableTrackedInMinutes);
        result.UserId.Should().Be(expectedResult.UserId);
        result.CreatedById.Should().Be(expectedResult.CreatedById);
    }

    [Fact]
    public async Task GetByIdAsyncWhenTimesheetDoesNotExistReturnsNull()
    {
        // Act
        var result = await _query.ExecuteAsync(int.MaxValue, CancellationToken.None);

        // Assert
        using var assertScope = _factory.Services.CreateScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<TimeTrackDbContext>();
        var entity = await assertContext.Timesheets
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == int.MaxValue);

        result.Should().BeNull();
        result.Should().BeEquivalentTo(entity);
    }
}

