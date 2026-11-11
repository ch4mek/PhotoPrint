using System.Globalization;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;

internal interface IUpdateTimesheetResolver
{
    Task<TimesheetCoreDto> Resolve(UpdateTimesheetDto request, CancellationToken cancellationToken);
}

internal class UpdateTimesheetResolver : IUpdateTimesheetResolver
{
    private readonly IGetProjectBillableStatusIfExistsDbQuery _getProjectBillableStatusIfExistsDbQuery;
    private readonly ICanModifyTimesheetForDateBusinessRule _canModifyTimesheetForDateBusinessRule;
    private readonly IGetProjectUserBillableStatusDbQuery _getProjectUserBillableStatusDbQuery;
    private readonly IGetProjectActiveStatusDbQuery _getProjectActiveStatusDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUpdateTimesheetValidator _updateTimesheetValidator;
    private readonly IGetTimesheetDbQuery _getTimesheetDbQuery;
    private readonly ITimeTrackDbContext _dbContext;


    public UpdateTimesheetResolver(IUpdateTimesheetValidator updateTimesheetValidator,
        IGetProjectUserBillableStatusDbQuery getProjectUserBillableStatusDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        ICanModifyTimesheetForDateBusinessRule canModifyTimesheetForDateBusinessRule,
        IGetProjectBillableStatusIfExistsDbQuery getProjectBillableStatusIfExistsDbQuery,
        IGetTimesheetDbQuery getTimesheetDbQuery,
        ITimeTrackDbContext dbContext,
        IGetProjectActiveStatusDbQuery getProjectActiveStatusDbQuery)
    {
        _updateTimesheetValidator = updateTimesheetValidator;
        _getProjectUserBillableStatusDbQuery = getProjectUserBillableStatusDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _canModifyTimesheetForDateBusinessRule = canModifyTimesheetForDateBusinessRule;
        _getProjectBillableStatusIfExistsDbQuery = getProjectBillableStatusIfExistsDbQuery;
        _getTimesheetDbQuery = getTimesheetDbQuery;
        _dbContext = dbContext;
        _getProjectActiveStatusDbQuery = getProjectActiveStatusDbQuery;
    }

    public async Task<TimesheetCoreDto> Resolve(UpdateTimesheetDto request, CancellationToken cancellationToken)
    {
        var timesheetId = request.Id;
        var timesheet = await GetTimesheet(timesheetId, cancellationToken);

        var projectBillableStatus = await _getProjectBillableStatusIfExistsDbQuery.ExecuteAsync(
                timesheet.ProjectId,
            cancellationToken);

        var isProjectBillable = await _getProjectUserBillableStatusDbQuery.ExecuteAsync(timesheet.ProjectId,
            timesheet.UserId,
            timesheet.ReportForDate,
            cancellationToken);

        if (!projectBillableStatus.HasValue)
        {
            throw new EntityNotFoundException($"Project with ID {timesheet.ProjectId} does not exists");
        }

        var projectActiveStatus = await _getProjectActiveStatusDbQuery.ExecuteAsync(
            request.ProjectId,
            cancellationToken);

        if (!projectActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetAtInactiveProject);
        }

        var canUpdateTimesheetForDate = await _canModifyTimesheetForDateBusinessRule.IsCorrespondsToBusiness(
            timesheet.UserId,
            timesheet.ReportForDate,
            cancellationToken);

        if (!canUpdateTimesheetForDate)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
        }

        var hasPermissionAssignment = _authenticatedUserContext
            .HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, timesheet.ProjectId.ToString(CultureInfo.InvariantCulture));
        if (request.BillableTrackedTimeUpdated || _authenticatedUserContext.IsManager)
        {
            await _updateTimesheetValidator.ValidateAsync(timesheet.UserId, timesheet.ProjectId,
                request.BillableTrackedInMinutes,
                cancellationToken);
        }

        if (!_authenticatedUserContext.IsManager && !hasPermissionAssignment && isProjectBillable)
        {
            var model = request with { BillableTrackedInMinutes = request.FactTrackedInMinutes };
            request = model;
        }
        else if (!_authenticatedUserContext.IsManager && !hasPermissionAssignment && !isProjectBillable)
        {
            var model = request with { BillableTrackedInMinutes = timesheet.BillableTrackedInMinutes };
            request = model;
        }
        else if (request.BillableTrackedInMinutes is null)
        {
            var model = request with { BillableTrackedInMinutes = 0 };
            request = model;
        }

        await UpdateTimeAsync(timesheet, request, cancellationToken);

        var updatedTimesheet = await GetUpdatedTimesheetDto(timesheetId, cancellationToken);

        return new TimesheetCoreDto
        {
            TaskName = updatedTimesheet.TaskName,
            BillableTrackedInMinutes = updatedTimesheet.BillableTrackedInMinutes,
            FactTrackedInMinutes = updatedTimesheet.FactTrackedInMinutes,
            Id = updatedTimesheet.Id,
            ProjectId = updatedTimesheet.ProjectId,
            ReportForDate = updatedTimesheet.ReportForDate,
            UserId = updatedTimesheet.UserId,
        };
    }

    private async Task<TimesheetDto> GetUpdatedTimesheetDto(int timesheetId, CancellationToken cancellationToken)
    {
        var timesheet = await GetTimesheet(timesheetId, cancellationToken);

        return TimesheetDto.Of(timesheet);
    }

    private async Task<Timesheet> GetTimesheet(int timesheetId, CancellationToken cancellationToken)
    {
        var timesheet = await _getTimesheetDbQuery.ExecuteAsync(timesheetId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.TimesheetNotFound);

        return timesheet;
    }

    private Task<int> UpdateTimeAsync(Timesheet timesheet, UpdateTimesheetDto request, CancellationToken cancellationToken)
    {
        timesheet.FactTrackedInMinutes = request.FactTrackedInMinutes;
        timesheet.BillableTrackedInMinutes = request.BillableTrackedInMinutes!.Value;

        _dbContext.Timesheets.Update(timesheet);

        return _dbContext.SaveChanges(cancellationToken);
    }
}
