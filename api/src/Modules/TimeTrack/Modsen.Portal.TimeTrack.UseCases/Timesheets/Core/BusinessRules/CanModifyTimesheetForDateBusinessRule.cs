using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface ICanModifyTimesheetForDateBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, DateOnly reportForDate, CancellationToken cancellationToken);
}

internal class CanModifyTimesheetForDateBusinessRule : ICanModifyTimesheetForDateBusinessRule
{
    private readonly IGetTimesheetSubmitStatusDbQuery _getSubmitStatusDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanModifyTimesheetForDateBusinessRule(IAuthenticatedUserContext authenticatedUserContext,
        IGetTimesheetSubmitStatusDbQuery getSubmitStatusDbQuery)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getSubmitStatusDbQuery = getSubmitStatusDbQuery;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, DateOnly reportForDate,
        CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.IsManager)
        {
            return true;
        }

        var submitStatus = await _getSubmitStatusDbQuery.ExecuteAsync(userId, reportForDate, cancellationToken);

        if (submitStatus is TimesheetSubmitStatusType.UnderReview or TimesheetSubmitStatusType.Approved)
        {
            return false;
        }

        return true;
    }
}
