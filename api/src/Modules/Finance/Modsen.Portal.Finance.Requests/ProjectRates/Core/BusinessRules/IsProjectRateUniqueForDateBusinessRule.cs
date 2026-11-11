using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;

internal interface IIsProjectRateUniqueForDateBusinessRule
{
    Task<bool> IsInvalid(int? rateId, int userId, int projectId, DateOnly date, CancellationToken cancellationToken);
}

internal class IsProjectRateUniqueForDateBusinessRule : IIsProjectRateUniqueForDateBusinessRule
{
    private readonly IFinanceDbContext _dbContext;

    public IsProjectRateUniqueForDateBusinessRule(IFinanceDbContext teamBoardDbContext)
    {
        _dbContext = teamBoardDbContext;
    }

    public Task<bool> IsInvalid(int? rateId, int userId, int projectId, DateOnly date, CancellationToken cancellationToken)
    {
        // user already has another project rate with the same RateFrom date
        return _dbContext.ProjectRates
            .Where(rate => rate.UserId == userId && rate.ProjectId == projectId)
            .Where(rate => rate.Id != rateId)
            .AnyAsync(rate => rate.RateFrom == date,
                cancellationToken);
    }
}
