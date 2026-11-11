using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Payslips.Core.BusinessRules;

internal interface IIsUserHaveAccessToPayslipFileBusinessRule
    : IBusinessRule<IsUserHaveAccessToPayslipFileBusinessRuleModel>;

internal record IsUserHaveAccessToPayslipFileBusinessRuleModel(string PayslipFileId);

internal class IsUserHaveAccessToPayslipFileBusinessRule : IIsUserHaveAccessToPayslipFileBusinessRule
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public IsUserHaveAccessToPayslipFileBusinessRule(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }
    public async Task<bool> IsCorrespondsToBusiness(
        IsUserHaveAccessToPayslipFileBusinessRuleModel? model,
        CancellationToken cancellationToken)
    {
        var payslip = await _teamBoardDbContext.Payslips
            .AsNoTracking()
            .Include(payslip => payslip.User)
            .FirstOrDefaultAsync(
                payslip => payslip.FileId == model!.PayslipFileId,
                cancellationToken: cancellationToken);

        if (payslip is null)
        {
            throw new EntityNotFoundException("Payslip is not found");
        }

        return payslip.UserId == _authenticatedUserContext.UserId;
    }
}
