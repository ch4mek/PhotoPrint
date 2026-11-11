using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Finance.Requests.Core.Builders;

internal interface IFinanceClientsAndUsersQueryBuilder
{
    Task<IQueryable<FinanceProjectUserEntity>> BuildAvailabletoViewFinanceProjectUsersAsync(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token);
    Task<IQueryable<int>> BuildAvailableFinanceUserIdsAsync(CancellationToken token, int year = default, int month = default);
    Task<(IQueryable<FinanceProjectUserEntity> View, IQueryable<FinanceProjectUserEntity> Manage)> BuildAvailableFinanceProjectUsersAsync(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token, bool composeManage = true);
}

internal class FinanceClientsAndUsersQueryBuilder : IFinanceClientsAndUsersQueryBuilder
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;
    private readonly IClockService _clockService;

    public FinanceClientsAndUsersQueryBuilder(
        IFinanceDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract,
        IClockService clockService)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
        _clockService = clockService;
    }

    public async Task<IQueryable<FinanceProjectUserEntity>> BuildAvailabletoViewFinanceProjectUsersAsync(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token)
    {
        query = await ApplyGeneralFilters(query, year, month, token);

        if (_authenticatedUserContext.HasClaim(FinanceClaimTypeNames.ViewAllRates.Name))
        {
            return query;
        }
        else
        {
            var today = _clockService.TodayUtc();

            return await GetRestrictedQuery(
                query,
                year == default ? today.Year : year,
                month == default ? today.Month : month,
                token);
        }
    }

    public async Task<IQueryable<int>> BuildAvailableFinanceUserIdsAsync(CancellationToken token, int year = default, int month = default)
    {
        var allProjectUsers = _dbContext.FinanceProjectUsers.AsNoTracking().AsQueryable();
        var managedProjectUsers = await BuildAvailabletoViewFinanceProjectUsersAsync(allProjectUsers, year, month, token);
        return managedProjectUsers.Select(x => x.UserId).Distinct();
    }

    public async Task<(IQueryable<FinanceProjectUserEntity> View, IQueryable<FinanceProjectUserEntity> Manage)> BuildAvailableFinanceProjectUsersAsync(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token, bool composeManage = true)
    {
        query = await ApplyGeneralFilters(query, year, month, token);

        var canViewAll = _authenticatedUserContext.HasClaim(FinanceClaimTypeNames.ViewAllRates.Name);
        var canManageAll = _authenticatedUserContext.HasClaim(FinanceClaimTypeNames.ManageAllRates.Name);

        if (canViewAll && canManageAll)
        {
            return (query, query);
        }

        var view = canViewAll
            ? query
            : await GetRestrictedQuery(query, year, month, token);

        var today = _clockService.TodayUtc();

        var manage = canManageAll || !composeManage
            ? query
            : await GetRestrictedQuery(query, today.Year, today.Month, token);

        return (view, manage);
    }

    private async Task<IQueryable<FinanceProjectUserEntity>> GetRestrictedQuery(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token)
    {
        var userAndSubordinatesIds = await GetUserAndSubordinatesIdsAsync(year, month, token);

        var managedClientsIds = _dbContext.FinanceClients
            .Where(x => x.SalesManagerId != null && userAndSubordinatesIds.Contains(x.SalesManagerId.Value))
            .Select(x => x.Id);

        return query.Where(x => managedClientsIds.Contains(x.Project!.ClientId) || userAndSubordinatesIds.Contains(x.UserId));
    }

    private async Task<List<int>> GetUserAndSubordinatesIdsAsync(int year, int month, CancellationToken token)
    {
        var userIds = new List<int>();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            var subordinatesIds = await _identityContract.UsersWorkInDepartment(
                year: year,
                month: month,
                departmentIds: [_authenticatedUserContext.DepartmentId!],
                cancellationToken: token);
            userIds.AddRange(subordinatesIds);
        }
        else if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            var groupMembers = await _dbContext.FinanceUsers
                .Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                .Select(x => x.Id)
                .ToListAsync(token);

            userIds.AddRange(groupMembers);
        }
        else
        {
            userIds.Add(_authenticatedUserContext.GetRequiredUserId());
        }

        return userIds;
    }

    private async Task<IQueryable<FinanceProjectUserEntity>> ApplyGeneralFilters(
        IQueryable<FinanceProjectUserEntity> query, int year, int month, CancellationToken token)
    {
        query = ApplyProjectAssignRequiriments(query);
        query = await ExcludeCeoAndCtoAsync(query, token);
        query = FilterMonth(query, year, month);

        return query;
    }

    private static IQueryable<FinanceProjectUserEntity> ApplyProjectAssignRequiriments(IQueryable<FinanceProjectUserEntity> query)
    {
        return query.Where(x => !x.Project!.IsPublic && x.IsTimeOnProjectBillable);
    }

    private async Task<IQueryable<FinanceProjectUserEntity>> ExcludeCeoAndCtoAsync(
        IQueryable<FinanceProjectUserEntity> query, CancellationToken token)
    {
        var ids = await _identityContract.GetCeoCtoIds(token);

        return query.Where(x => !ids.Contains(x.UserId));
    }

    private static IQueryable<FinanceProjectUserEntity> FilterMonth(
        IQueryable<FinanceProjectUserEntity> query, int year, int month)
    {
        if (year == default || month == default)
        {
            return query;
        }

        var firstDayInMonth = new DateOnly(year, month, 1);
        var lastDayInMonth = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

        return query.Where(x => x.UserOnProjectFrom <= lastDayInMonth
               && (x.UserOnProjectTo == null || x.UserOnProjectTo >= firstDayInMonth));
    }
}
