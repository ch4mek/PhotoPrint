using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;

internal interface ITimesheetComposer
{
    Task<TimesheetCoreDto> Compose(CreateOrUpdateTrackedTimeDto request, CancellationToken cancellationToken);
}

internal class TimesheetComposer : ITimesheetComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICreateTimesheetResolver _createTimesheetResolver;
    private readonly IUpdateTimesheetResolver _updateTimesheetResolver;
    private readonly ITimeTrackDbContext _dbContext;

    public TimesheetComposer(
        IAuthenticatedUserContext authenticatedUserContext,
        ICreateTimesheetResolver createTimesheetResolver,
        IUpdateTimesheetResolver updateTimesheetResolver,
        ITimeTrackDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _createTimesheetResolver = createTimesheetResolver;
        _updateTimesheetResolver = updateTimesheetResolver;
        _dbContext = dbContext;
    }

    public async Task<TimesheetCoreDto> Compose(CreateOrUpdateTrackedTimeDto request, CancellationToken cancellationToken)
    {
        int userId;

        if (request.AllowAutomatic)
        {
            userId = request.UserId!.Value;
        }
        else
        {
            userId = _authenticatedUserContext.IsManager && request.UserId is not null
                ? request.UserId.Value
                : _authenticatedUserContext.GetRequiredUserId();
        }

         var timesheet = await _dbContext.Timesheets
             .AsNoTracking()
             .Where(x => x.UserId == userId
                 && x.ProjectId == request.ProjectId
                 && x.ReportYear == request.ReportForDate.Year
                 && x.ReportDayOfYear == request.ReportForDate.DayOfYear
                 && x.TaskName == request.TaskName)
             .Select(x => new TimesheetCoreDto
             {
                 Id = x.Id,
                 ProjectId = x.ProjectId,
                 ReportForDate = x.ReportForDate,
                 BillableTrackedInMinutes = x.BillableTrackedInMinutes,
                 FactTrackedInMinutes = x.FactTrackedInMinutes
             })
             .FirstOrDefaultAsync(cancellationToken);

        int? billableTrackedInMinutes = request is UpsertBillableDto billable
            ? billable.BillableTrackedInMinutes
            : null;
        var factTrackedInMinutes = request is UpsertFactDto fact
            ? fact.FactTrackedInMinutes
            : 0;

        if (timesheet is null)
        {
            return await _createTimesheetResolver.Resolve(new CreateTimesheetDto
            {
                BillableTrackedInMinutes = billableTrackedInMinutes,
                FactTrackedInMinutes = factTrackedInMinutes,
                ProjectId = request.ProjectId,
                ReportForDate = request.ReportForDate,
                TaskName = request.TaskName,
                UserId = userId,
                AllowAutomatic = request.AllowAutomatic,
            }, cancellationToken);
        }

        return await _updateTimesheetResolver.Resolve(new UpdateTimesheetDto
        {
            BillableTrackedInMinutes = request is UpsertBillableDto
                ? billableTrackedInMinutes
                : timesheet.BillableTrackedInMinutes,
            FactTrackedInMinutes = request is UpsertFactDto
                ? factTrackedInMinutes
                : timesheet.FactTrackedInMinutes,
            BillableTrackedTimeUpdated = billableTrackedInMinutes.HasValue,
            Id = timesheet.Id,
            ProjectId = request.ProjectId
        }, cancellationToken);
    }
}
