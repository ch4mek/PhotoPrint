using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;

internal interface IIsSalaryFromDateValidBusinessRule
{
    Task<bool> IsInvalid(int? salaryId, int userId, DateOnly date, CancellationToken cancellationToken);
}

internal class IsSalaryFromDateValidBusinessRule : IIsSalaryFromDateValidBusinessRule
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public IsSalaryFromDateValidBusinessRule(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public Task<bool> IsInvalid(int? salaryId, int userId, DateOnly date, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.UserSalaries
            .Where(salary => salary.UserId == userId)
            .Where(salary => salary.Id != salaryId)
            .AnyAsync(salary => salary.SalaryFrom == date,
                cancellationToken);
    }
}
