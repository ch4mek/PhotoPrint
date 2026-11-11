using System.Globalization;
using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

internal sealed class DeleteTimesheetRequest
    : ICommandHandler<DeleteTimesheetRequest.Command, TimesheetDto>
{
    private readonly IGetProjectUserBillableStatusDbQuery _getProjectUserBillableStatusDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IDeleteTimesheetValidator _deleteTimesheetValidator;
    private readonly IGetTimesheetDbQuery _getTimesheetDbQuery;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public DeleteTimesheetRequest(IGetProjectUserBillableStatusDbQuery getProjectUserBillableStatusDbQuery,
        IAuthenticatedUserContext authenticatedUserContext,
        IDeleteTimesheetValidator deleteTimesheetValidator,
        IGetTimesheetDbQuery getTimesheetDbQuery,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _getProjectUserBillableStatusDbQuery = getProjectUserBillableStatusDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _deleteTimesheetValidator = deleteTimesheetValidator;
        _getTimesheetDbQuery = getTimesheetDbQuery;
        _timeTrackDbContext = timeTrackDbContext;

    }

    public async Task<TimesheetDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var timesheet = await _getTimesheetDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.TimesheetNotFound);

        await _deleteTimesheetValidator.ValidateAsync(timesheet, cancellationToken);

        var hasPermissionAssignment = _authenticatedUserContext.HasClaimWithValue(
            PortalClaimTypeNames.TrackBillableWorkingHoursOnProject,
            timesheet.ProjectId.ToString(CultureInfo.InvariantCulture));

        var isProjectBillable = await _getProjectUserBillableStatusDbQuery.ExecuteAsync(timesheet.ProjectId,
            timesheet.UserId,
            timesheet.ReportForDate,
            cancellationToken);

        var isTrackedBillableTimeOnNonBillableProjectWithoutPermission = !hasPermissionAssignment &&
            !isProjectBillable &&
            timesheet.BillableTrackedInMinutes != 0;

        var isNotManagerOnBillableProjectWithoutPermission = isProjectBillable && !hasPermissionAssignment &&
            (_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator());

        if (!_authenticatedUserContext.IsManager && isTrackedBillableTimeOnNonBillableProjectWithoutPermission)
        {
            timesheet.FactTrackedInMinutes = 0;
        }
        else
        {
            if (!request.DeleteBillable)
            {
                if (isNotManagerOnBillableProjectWithoutPermission)
                {
                    timesheet.BillableTrackedInMinutes = 0;
                }

                timesheet.FactTrackedInMinutes = 0;
            }

            if (request.DeleteBillable)
            {
                timesheet.BillableTrackedInMinutes = 0;
            }

            if (timesheet is { FactTrackedInMinutes: 0, BillableTrackedInMinutes: 0 })
            {
                _timeTrackDbContext.Timesheets.Remove(timesheet);
            }
        }

        await _timeTrackDbContext.SaveChanges(cancellationToken);

        return TimesheetDto.Of(timesheet);
    }

    internal sealed record Command(int Id, bool DeleteBillable) : ICommand<TimesheetDto>, ITransactionalRequest;

    internal sealed class DeleteTimesheetCommandValidator : AbstractValidator<Command>
    {
        public DeleteTimesheetCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
