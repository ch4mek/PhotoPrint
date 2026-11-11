using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface IUpdateTaskNameValidator
{
    Task ValidateAsync(UpdateTaskNameDto updateDto, List<TimesheetWithTrackedTimeCoreDto> timesheetsToUpdate,
        CancellationToken cancellationToken);
}

internal sealed class UpdateTaskNameValidator : IUpdateTaskNameValidator
{
    private readonly ICanModifyTimesheetForDateBusinessRule _canModifyTimesheetForDateBusinessRule;
    private readonly ICanUserUpdateTimesheetBusinessRule _canUserUpdateTimesheetBusinessRule;
    private readonly INewTaskNameHasNoCollisionsBusinessRule _newTaskNameHasNoCollisionsBusinessRule;
    private readonly IGetProjectActiveStatusDbQuery _getProjectActiveStatusDbQuery;

    public UpdateTaskNameValidator(
        ICanModifyTimesheetForDateBusinessRule canModifyTimesheetForDateBusinessRule,
        ICanUserUpdateTimesheetBusinessRule canUserUpdateTimesheetBusinessRule,
        INewTaskNameHasNoCollisionsBusinessRule newTaskNameHasNoCollisionsBusinessRule,
        IGetProjectActiveStatusDbQuery getProjectActiveStatusDbQuery)
    {
        _canModifyTimesheetForDateBusinessRule = canModifyTimesheetForDateBusinessRule;
        _canUserUpdateTimesheetBusinessRule = canUserUpdateTimesheetBusinessRule;
        _newTaskNameHasNoCollisionsBusinessRule = newTaskNameHasNoCollisionsBusinessRule;
        _getProjectActiveStatusDbQuery = getProjectActiveStatusDbQuery;
    }

    public async Task ValidateAsync(UpdateTaskNameDto updateDto,
        List<TimesheetWithTrackedTimeCoreDto> timesheetsToUpdate,
        CancellationToken cancellationToken)
    {
        var projectActiveStatus = await _getProjectActiveStatusDbQuery.ExecuteAsync(
            updateDto.ProjectId,
            cancellationToken);

        if (!projectActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetAtInactiveProject);
        }

        var canUpdateTaskName = await _canUserUpdateTimesheetBusinessRule.IsCorrespondsToBusiness(updateDto.UserId,
            cancellationToken);
        if (!canUpdateTaskName)
        {
            throw new ActionNotAllowedException();
        }

        foreach (var timesheet in timesheetsToUpdate)
        {
            var canUpdateTimesheetForDate = await _canModifyTimesheetForDateBusinessRule.IsCorrespondsToBusiness(
                updateDto.UserId,
                timesheet.ReportForDate,
                cancellationToken);

            if (!canUpdateTimesheetForDate)
            {
                throw new ActionNotAllowedException(
                    PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
            }
        }

        var hasNoCollision = await _newTaskNameHasNoCollisionsBusinessRule.IsCorrespondsToBusiness(updateDto,
            timesheetsToUpdate,
            cancellationToken);

        if (!hasNoCollision)
        {
            throw new EntityConflictException(PortalMessages.TaskWithSameNameAlreadyExists);
        }
    }
}
