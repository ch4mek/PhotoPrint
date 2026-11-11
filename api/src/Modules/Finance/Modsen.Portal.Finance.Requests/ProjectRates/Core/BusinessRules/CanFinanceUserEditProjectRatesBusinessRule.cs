using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.BusinessRules;

internal interface ICanFinanceUserEditProjectRatesBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, int projectId, DateOnly targetDate, CancellationToken cancellationToken);
    Task<Dictionary<ProjectRateBusinessKey, bool>> IsCorrespondsToBusinessBatch(
            IEnumerable<ProjectRateBusinessKey> rates, CancellationToken cancellationToken);
}

internal class CanFinanceUserEditProjectRatesBusinessRule : ICanFinanceUserEditProjectRatesBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;
    private readonly IFinanceDbContext _dbContext;

    public CanFinanceUserEditProjectRatesBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract,
        IFinanceDbContext dbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
        _dbContext = dbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, int projectId, DateOnly targetDate, CancellationToken cancellationToken)
    {
        var key = new ProjectRateBusinessKey(userId, projectId, targetDate);
        var result = await IsCorrespondsToBusinessBatch([new ProjectRateBusinessKey(userId, projectId, targetDate)], cancellationToken);
        return result[key];
    }

    public async Task<Dictionary<ProjectRateBusinessKey, bool>> IsCorrespondsToBusinessBatch(
        IEnumerable<ProjectRateBusinessKey> rates, CancellationToken cancellationToken)
    {
        var result = new Dictionary<ProjectRateBusinessKey, bool>();
        if (_authenticatedUserContext.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name))
        {
            return rates.ToDictionary(x => x, _ => true);
        }

        if (!_authenticatedUserContext.HasClaim(FinanceClaimTypeNames.ManageRates.Name))
        {
            return rates.ToDictionary(x => x, _ => false);
        }

        var clientSalesManagerIds = await _dbContext.FinanceProjects
            .Where(p => rates.Select(x => x.ProjectId).Contains(p.Id))
            .Include(p => p.Client)
            .ToDictionaryAsync(p => p.Id, p => p.Client!.SalesManagerId, cancellationToken);

        var dateSubordinates = new Dictionary<DateOnly, List<int>>();

        var initiatorId = _authenticatedUserContext.GetRequiredUserId();

        foreach (var rate in rates)
        {
            var rateSalesManagerId = clientSalesManagerIds[rate.ProjectId];

            if (rateSalesManagerId == initiatorId)
            {
                result[rate] = true;
                continue;
            }

            if (!dateSubordinates.TryGetValue(rate.RateFrom, out _))
            {
                dateSubordinates[rate.RateFrom] = rate.RateFrom == DateOnly.MinValue
                    ? await _identityContract.GetUserAndSubordinatesIdsAsync(default, default, cancellationToken, includeSelf: false)
                    : await _identityContract.GetUserAndSubordinatesIdsAsync(rate.RateFrom.Year, rate.RateFrom.Month, cancellationToken, includeSelf: false);
            }

            var subordinates = dateSubordinates[rate.RateFrom];

            if (subordinates.Contains(rate.UserId)
                || (rateSalesManagerId is not null && subordinates.Contains(rateSalesManagerId.Value)))
            {
                result[rate] = true;
                continue;
            }

            result[rate] = false;
        }

        return result;
    }
}
