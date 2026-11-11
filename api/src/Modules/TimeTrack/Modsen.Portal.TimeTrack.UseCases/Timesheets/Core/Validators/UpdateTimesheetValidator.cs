using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface IUpdateTimesheetValidator
{
    Task ValidateAsync(int userId, int projectId, int? trackedBillableMinutes, CancellationToken cancellationToken);
}

internal sealed class UpdateTimesheetValidator : IUpdateTimesheetValidator
{
    private readonly ICanUserUpdateTimesheetBusinessRule _canUserUpdateTimesheetBusinessRule;
    private readonly ICanTrackBillableHoursBusinessRule _canTrackBillableHoursBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetProjectActiveStatusDbQuery _getProjectActiveStatusDbQuery;

    public UpdateTimesheetValidator(
        ICanUserUpdateTimesheetBusinessRule canUserUpdateTimesheetBusinessRule,
        ICanTrackBillableHoursBusinessRule canTrackBillableHoursBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetProjectActiveStatusDbQuery getProjectActiveStatusDbQuery)
    {
        _canUserUpdateTimesheetBusinessRule = canUserUpdateTimesheetBusinessRule;
        _canTrackBillableHoursBusinessRule = canTrackBillableHoursBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _getProjectActiveStatusDbQuery = getProjectActiveStatusDbQuery;
    }

    public async Task ValidateAsync(int userId, int projectId, int? trackedBillableMinutes,
        CancellationToken cancellationToken)
    {
        var projectActiveStatus = await _getProjectActiveStatusDbQuery.ExecuteAsync(
            projectId,
            cancellationToken);

        if (!projectActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetAtInactiveProject);
        }

        if (!_authenticatedUserContext.IsManager)
        {
            var canTrackBillable = await _canTrackBillableHoursBusinessRule.IsCorrespondsToBusiness(userId,
                projectId,
                trackedBillableMinutes,
                cancellationToken);

            if (!canTrackBillable)
            {
                throw new ActionNotAllowedException(PortalMessages.CannotTrackBillableHoursWithoutPermission);
            }
        }

        var canUpdate = await _canUserUpdateTimesheetBusinessRule.IsCorrespondsToBusiness(userId, cancellationToken);
        if (!canUpdate)
        {
            throw new ActionNotAllowedException();
        }
    }
}
