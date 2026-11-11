using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface INewTaskNameHasNoCollisionsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(UpdateTaskNameDto updateDto,
        List<TimesheetWithTrackedTimeCoreDto> timesheetsToUpdate,
        CancellationToken cancellationToken);
}

internal sealed class NewTaskNameHasNoCollisionsBusinessRule : INewTaskNameHasNoCollisionsBusinessRule
{
    private readonly ITimeTrackDbContext _dbContext;

    public NewTaskNameHasNoCollisionsBusinessRule(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(UpdateTaskNameDto updateDto,
        List<TimesheetWithTrackedTimeCoreDto> timesheetsToUpdate,
        CancellationToken cancellationToken)
    {
        var timesheetIds = timesheetsToUpdate.ConvertAll(timesheet => timesheet.Id);
        var reportsForDates = timesheetsToUpdate.ConvertAll(timesheet => timesheet.ReportForDate);

        var hasCollision = await _dbContext.Timesheets
            .Where(timesheet => timesheet.UserId == updateDto.UserId)
            .Where(timesheet => timesheet.ProjectId == updateDto.ProjectId)
            .Where(timesheet => timesheet.TaskName == updateDto.TaskName)
            .Where(timesheet => updateDto.StartDate <= timesheet.ReportForDate &&
                updateDto.EndDate >= timesheet.ReportForDate)
            .Where(timesheet => reportsForDates.Contains(timesheet.ReportForDate) ||
                timesheetIds.Contains(timesheet.Id))
            .AnyAsync(cancellationToken);

        return !hasCollision;
    }
}
