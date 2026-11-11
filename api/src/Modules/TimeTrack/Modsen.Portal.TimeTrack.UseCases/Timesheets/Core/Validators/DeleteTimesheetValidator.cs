using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface IDeleteTimesheetValidator
{
    Task ValidateAsync(Timesheet timesheet, CancellationToken cancellationToken);
}

internal sealed class DeleteTimesheetValidator : IDeleteTimesheetValidator
{
    private readonly ICanModifyTimesheetForDateBusinessRule _canModifyTimesheetForDateBusinessRule;
    private readonly ICanUserUpdateTimesheetBusinessRule _canUserUpdateTimesheetBusinessRule;
    private readonly IGetProjectActiveStatusDbQuery _getProjectActiveStatusDbQuery;

    public DeleteTimesheetValidator(
        ICanModifyTimesheetForDateBusinessRule canModifyTimesheetForDateBusinessRule,
        ICanUserUpdateTimesheetBusinessRule canUserUpdateTimesheetBusinessRule,
        IGetProjectActiveStatusDbQuery getProjectActiveStatusDbQuery)
    {
        _canModifyTimesheetForDateBusinessRule = canModifyTimesheetForDateBusinessRule;
        _canUserUpdateTimesheetBusinessRule = canUserUpdateTimesheetBusinessRule;
        _getProjectActiveStatusDbQuery = getProjectActiveStatusDbQuery;
    }

    public async Task ValidateAsync(Timesheet timesheet, CancellationToken cancellationToken)
    {
        var projectActiveStatus = await _getProjectActiveStatusDbQuery.ExecuteAsync(
            timesheet.ProjectId,
            cancellationToken);

        if (!projectActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetAtInactiveProject);
        }

        var canUserDeleteTimesheet = await _canUserUpdateTimesheetBusinessRule
            .IsCorrespondsToBusiness(timesheet.UserId, cancellationToken);
        if (!canUserDeleteTimesheet)
        {
            throw new ActionNotAllowedException();
        }

        var canDeleteTimesheetForDate = await _canModifyTimesheetForDateBusinessRule.IsCorrespondsToBusiness(
            timesheet.UserId,
            timesheet.ReportForDate,
            cancellationToken);
        if (!canDeleteTimesheetForDate)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
        }
    }
}
