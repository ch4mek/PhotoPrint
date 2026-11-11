using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanUserProjectBeDecreasedBusinessRule 
    : IBusinessRuleWithFailReason<CanUserProjectBeDecreasedModel>;
    
internal record CanUserProjectBeDecreasedModel(int ProjectId, int UserId, DateOnly UserOnProjectFromBefore,
    DateOnly? UserOnProjectToBefore, DateOnly UserOnProjectFrom, DateOnly? UserOnProjectTo);
    
internal class CanUserProjectBeDecreasedBusinessRule : ICanUserProjectBeDecreasedBusinessRule
{
    private readonly IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery;
    
    public CanUserProjectBeDecreasedBusinessRule(
        IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery getTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery)
    {
        _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery = getTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(CanUserProjectBeDecreasedModel? model, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var dbRequest = new GetTimesheetByProjectIdAndUserIdAndDateIntervalDbRequest(model.ProjectId, model.UserId,
            model.UserOnProjectFromBefore, model.UserOnProjectToBefore);
        var timesheet = await _getTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery.ExecuteAsync(dbRequest,
            cancellationToken);

        return timesheet.Any(x => model.UserOnProjectFromBefore <= x.ReportForDate && x.ReportForDate < model.UserOnProjectFrom ||
            model.UserOnProjectTo < x.ReportForDate && x.ReportForDate <= model.UserOnProjectToBefore ||
            model.UserOnProjectTo < x.ReportForDate && !model.UserOnProjectToBefore.HasValue)
            ? BusinessRuleResult.Fail(PortalMessages.TimesheetIsFilledForPeriod)
            : BusinessRuleResult.Success();
    }
}
