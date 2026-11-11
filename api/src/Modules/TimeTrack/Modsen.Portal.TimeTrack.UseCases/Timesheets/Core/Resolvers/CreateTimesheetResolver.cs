using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;

internal interface ICreateTimesheetResolver
{
    Task<TimesheetCoreDto> Resolve(CreateTimesheetDto request, CancellationToken cancellationToken);
}

internal class CreateTimesheetResolver : ICreateTimesheetResolver
{
    private readonly IGetProjectUserBillableStatusDbQuery _getProjectUserBillableStatusDbQuery;
    private readonly ICreateTimesheetValidator _userCanCreateTimesheetValidator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimesheetDbQuery _getTimesheetDbQuery;
    private readonly IIdentityContract _identityContract;
    private readonly ITimeTrackDbContext _dbContext;
    private readonly IClockService _clockService;

    public CreateTimesheetResolver(
        ICreateTimesheetValidator userCanCreateTimesheetValidator,
        IIdentityContract identityContract,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetProjectUserBillableStatusDbQuery getProjectUserBillableStatusDbQuery,
        IGetTimesheetDbQuery getTimesheetDbQuery,
        ITimeTrackDbContext dbContext,
        IClockService clockService)
    {
        _userCanCreateTimesheetValidator = userCanCreateTimesheetValidator;
        _identityContract = identityContract;
        _authenticatedUserContext = authenticatedUserContext;
        _getProjectUserBillableStatusDbQuery = getProjectUserBillableStatusDbQuery;
        _getTimesheetDbQuery = getTimesheetDbQuery;
        _dbContext = dbContext;
        _clockService = clockService;
    }

    public async Task<TimesheetCoreDto> Resolve(CreateTimesheetDto request, CancellationToken cancellationToken)
    {
        var timesheetIdWithTheSameTaskName = await _dbContext.Timesheets
            .Where(timesheet => timesheet.UserId == request.UserId &&
                timesheet.ProjectId == request.ProjectId &&
                timesheet.ReportYear == request.ReportForDate.Year &&
                timesheet.ReportDayOfYear == request.ReportForDate.DayOfYear &&
                timesheet.TaskName == request.TaskName)
            .Select(timesheet => new { timesheet.Id } )
            .FirstOrDefaultAsync(cancellationToken);

        var assignment = await _identityContract.GetPermissionAssignment(request.UserId, request.ProjectId.ToString(CultureInfo.InvariantCulture),
            PermissionResourceType.Project, PermissionType.TrackBillableWorkingHours, cancellationToken);

        var isUserAbleToTrackBillableHours = assignment is not null;

        var isTimesheetWithoutFactHoursExisting = await _dbContext.Timesheets
            .Where(timesheet => timesheet.UserId == request.UserId &&
                timesheet.ProjectId == request.ProjectId &&
                timesheet.ReportYear == request.ReportForDate.Year &&
                timesheet.ReportDayOfYear == request.ReportForDate.DayOfYear &&
                timesheet.TaskName == request.TaskName &&
                timesheet.FactTrackedInMinutes == 0)
            .Select(timesheet => timesheet.Id)
            .AnyAsync(cancellationToken);

        await _userCanCreateTimesheetValidator.ValidateAsync(request, cancellationToken);

        if (!isUserAbleToTrackBillableHours &&
            !isTimesheetWithoutFactHoursExisting &&
            timesheetIdWithTheSameTaskName is not null || isUserAbleToTrackBillableHours &&
            timesheetIdWithTheSameTaskName is not null)
        {
            throw new EntityConflictException(PortalMessages.TaskWithSameNameAlreadyExists);
        }

        TimesheetCoreDto result;

        var isProjectStatusBillable = await _getProjectUserBillableStatusDbQuery.ExecuteAsync(request.ProjectId,
            request.UserId,
            request.ReportForDate,
            cancellationToken);

        if (!_authenticatedUserContext.IsManager && assignment is null && isProjectStatusBillable)
        {
            request.BillableTrackedInMinutes = request.FactTrackedInMinutes;
        }
        else
        {
            request.BillableTrackedInMinutes ??= 0;
        }

        if (!isTimesheetWithoutFactHoursExisting)
        {
            result = await CreateTimesheetAsync(request.UserId, request, cancellationToken);
        }
        else
        {
            var timesheet = await _getTimesheetDbQuery.ExecuteAsync(timesheetIdWithTheSameTaskName!.Id,
                cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.TimesheetNotFound);

            timesheet.FactTrackedInMinutes = request.FactTrackedInMinutes;

            _dbContext.Timesheets.Update(timesheet);
            await _dbContext.SaveChanges(cancellationToken);

            result = new TimesheetCoreDto
            {
                Id = timesheet.Id,
                CreatedById = timesheet.UserId,
                TaskName = timesheet.TaskName,
                FactTrackedInMinutes = request.FactTrackedInMinutes,
                BillableTrackedInMinutes = 0,
                ReportForDate = timesheet.ReportForDate,
                UserId = timesheet.UserId,
                ProjectId = timesheet.ProjectId
            };
        }

        return result;
    }

    private async Task<TimesheetCoreDto> CreateTimesheetAsync(
        int userId,
        CreateTimesheetDto request,
        CancellationToken cancellationToken)
    {
        var timesheet = request.ToDatabaseEntity(userId, _authenticatedUserContext.UserId, _clockService.UtcNow);

        var createdTimesheet = await _dbContext.Timesheets.AddAsync(timesheet, cancellationToken);
        await _dbContext.SaveChanges(cancellationToken);

        return new TimesheetCoreDto
        {
            Id = createdTimesheet.Entity.Id,
            CreatedById = createdTimesheet.Entity.CreatedById,
            CreatedAtUtc = createdTimesheet.Entity.CreatedAtUtc,
            ProjectId = createdTimesheet.Entity.ProjectId,
            ReportForDate = createdTimesheet.Entity.ReportForDate,
            TaskName = request.TaskName,
            FactTrackedInMinutes = request.FactTrackedInMinutes,
            BillableTrackedInMinutes = request.BillableTrackedInMinutes ?? 0,
            UserId = userId
        };
    }
}
