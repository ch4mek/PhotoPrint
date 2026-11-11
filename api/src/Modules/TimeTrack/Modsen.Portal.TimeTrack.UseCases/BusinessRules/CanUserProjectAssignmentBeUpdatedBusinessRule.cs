using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanUserProjectAssignmentBeUpdatedBusinessRule : IBusinessRuleWithFailReason<CanUserProjectAssignmentBeUpdatedModel>;

internal sealed record CanUserProjectAssignmentBeUpdatedModel(
    AssignLessFieldsModels OldAssign,
    AssignLessFieldsModels NewAssign,
    int UserId,
    int ProjectId,
    DateOnly UserOnProjectFrom,
    DateOnly? UserOnProjectTo);

internal sealed record AssignLessFieldsModels(bool IsTimeOnProjectBillable, decimal Fte, string SeniorityLevelId);

internal sealed class CanUserProjectAssignmentBeUpdatedBusinessRule(
    ITimeTrackDbContext timeTrackDbContext) : ICanUserProjectAssignmentBeUpdatedBusinessRule
{
    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(CanUserProjectAssignmentBeUpdatedModel? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var isTimesheetsExist = await timeTrackDbContext.Timesheets
            .Where(timesheet => timesheet.UserId == model.UserId)
            .Where(timesheet => timesheet.ProjectId == model.ProjectId)
            .Where(timesheet => timesheet.ReportForDate >= model.UserOnProjectFrom)
            .Where(timesheet => model.UserOnProjectTo == null || timesheet.ReportForDate <= model.UserOnProjectTo)
            .AnyAsync(cancellationToken);

        var isLessFieldsUpdated =
            model.OldAssign.IsTimeOnProjectBillable != model.NewAssign.IsTimeOnProjectBillable ||
            model.OldAssign.Fte != model.NewAssign.Fte ||
            model.OldAssign.SeniorityLevelId != model.NewAssign.SeniorityLevelId;

        if (isTimesheetsExist && isLessFieldsUpdated)
        {
            return BusinessRuleResult.Fail("Assign with tracked time is not full editable");
        }

        return BusinessRuleResult.Success();
    }
}
