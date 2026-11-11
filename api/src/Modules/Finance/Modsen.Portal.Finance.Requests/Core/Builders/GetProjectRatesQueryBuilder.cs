using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Finance.Requests.Core.Builders;

internal interface IGetProjectRatesQueryBuilder
{
    IGetProjectRatesQueryBuilder ForYearAndMonth(int year, int month);
    IGetProjectRatesQueryBuilder WithPagination(int skip, int take);
    IGetProjectRatesQueryBuilder WithFilter(RatesManagementBaseFilterCoreDto filter);
    IGetProjectRatesQueryBuilder WithOrdering(FinanceOrderingCoreDto ordering);
    Task<GetProjectRatesQueryBuilderDto> BuildRates(CancellationToken token, bool isForManagement = false);
}

internal class GetProjectRatesQueryBuilder : IGetProjectRatesQueryBuilder
{
    private readonly IFinanceDbContext _dbContext;
    private readonly IFinanceClientsAndUsersQueryBuilder _managedClientsQueryBuilder;
    private readonly IIdentityContract _identityContract;

    private int _year;
    private int _month;
    private int? _skip;
    private int? _take;
    private DateOnly _firstDayInMonth;
    private DateOnly _lastDayInMonth;

    private RatesManagementBaseFilterCoreDto? _filter;
    private FinanceOrderingCoreDto? _ordering;

    public GetProjectRatesQueryBuilder(
        IFinanceDbContext dbContext,
        IFinanceClientsAndUsersQueryBuilder managedClientsQueryBuilder,
        IIdentityContract identityContract)
    {
        _dbContext = dbContext;
        _managedClientsQueryBuilder = managedClientsQueryBuilder;
        _identityContract = identityContract;
    }

    public IGetProjectRatesQueryBuilder ForYearAndMonth(int year, int month)
    {
        _year = year;
        _month = month;
        _firstDayInMonth = new DateOnly(_year, _month, 1);
        _lastDayInMonth = new DateOnly(_year, _month, DateTime.DaysInMonth(_year, _month));

        return this;
    }

    public IGetProjectRatesQueryBuilder WithPagination(int skip, int take)
    {
        _skip = skip;
        _take = take;

        return this;
    }

    public IGetProjectRatesQueryBuilder WithFilter(RatesManagementBaseFilterCoreDto filter)
    {
        _filter = filter;
        return this;
    }

    public IGetProjectRatesQueryBuilder WithOrdering(FinanceOrderingCoreDto ordering)
    {
        _ordering = ordering;
        return this;
    }

    public async Task<GetProjectRatesQueryBuilderDto> BuildRates(CancellationToken token, bool isForManagement = false)
    {
        var query = _dbContext.FinanceProjectUsers.AsQueryable();

        (query, var managedProjectUsersQuery) = await _managedClientsQueryBuilder
            .BuildAvailableFinanceProjectUsersAsync(query, _year, _month, token, !isForManagement);

        query = await ApplyFiltersAsync(query, token);
        var sql = query.ToQueryString();
        var pageUsersIds = await GetPageUsersIdsAsync(query, token);
        var pageProjectUserAssigns = query.Where(x => pageUsersIds.Items.Contains(x.UserId))
            .Include(x => x.User)
            .Include(x => x.Project)
            .ThenInclude(x => x!.Client);

        var rates = GetTargetRates(pageProjectUserAssigns);

        var resultProjectUsers = await pageProjectUserAssigns.GroupBy(x => x.UserId).ToListAsync(token);
        var resultProjectUsersIds = resultProjectUsers.SelectMany(x => x).Select(x => x.Id);

        var resultManagedProjectUsersIds = !isForManagement
            ? await managedProjectUsersQuery
                .Where(x => resultProjectUsersIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToHashSetAsync(token)
            : [];

        var result = new GetProjectRatesQueryBuilderDto
        {
            UsersIds = pageUsersIds.Items,
            ManagedProjectUsersIds = resultManagedProjectUsersIds,
            TotalUsersCount = pageUsersIds.TotalItemsCount,
            ProjectUsers = resultProjectUsers,
            Rates = await rates.GroupBy(x => x.UserId).ToListAsync(token)
        };

        return result;
    }

    private async Task<IQueryable<FinanceProjectUserEntity>> ApplyFiltersAsync(
        IQueryable<FinanceProjectUserEntity> query,
        CancellationToken token)
    {
        query = FilterByUserId(query);
        query = FilterByUserName(query);
        query = await FilterByDepartmentIdsAsync(query, token);

        query = FilterByProjectName(query);
        query = FilterByClientName(query);
        query = FilterByClientLegalEntity(query);

        return query;
    }

    private IQueryable<FinanceProjectUserEntity> FilterByUserId(IQueryable<FinanceProjectUserEntity> query)
    {
        return _filter?.UserId is not null
            ? query.Where(x => x.UserId == _filter.UserId)
            : query;
    }

    private IQueryable<FinanceProjectUserEntity> FilterByUserName(IQueryable<FinanceProjectUserEntity> query)
    {
        if (!(_filter?.UserFullName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.UserFullName.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(
                x.User!.LastNameRu == null && x.User!.FirstNameRu == null
                    ? x.User!.LastName + " " + x.User!.FirstName
                    : x.User!.LastNameRu != null && x.User!.FirstNameRu != null
                        ? x.User!.LastNameRu + " " + x.User!.FirstNameRu + " (" + x.User!.LastName + " " + x.User!.FirstName + ")"
                        : (x.User!.LastNameRu ?? x.User!.FirstNameRu) + " (" + x.User!.LastName + " " + x.User!.FirstName + ")",
                keyword));
        }

        return query;
    }

    private async Task<IQueryable<FinanceProjectUserEntity>> FilterByDepartmentIdsAsync(IQueryable<FinanceProjectUserEntity> query,
        CancellationToken token)
    {
        if (_filter?.DepartmentIds == null || _filter.DepartmentIds.Count == 0)
        {
            return query;
        }

        var userIds = await _identityContract.UsersWorkInDepartment(year: _year, month: _month,
            departmentIds: _filter.DepartmentIds, cancellationToken: token);

        return query.Where(x => userIds.Contains(x.UserId));
    }

    private IQueryable<FinanceProjectUserEntity> FilterByProjectName(IQueryable<FinanceProjectUserEntity> query)
    {
        if (!(_filter?.ProjectName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.ProjectName.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.Project!.ProjectName, keyword));
        }

        return query;
    }

    private IQueryable<FinanceProjectUserEntity> FilterByClientName(IQueryable<FinanceProjectUserEntity> query)
    {
        if (!(_filter?.ClientName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.ClientName.Trim()}%";
            query = query.Where(x => EF.Functions.ILike(x.Project!.Client!.Name, keyword));
        }

        return query;
    }

    private IQueryable<FinanceProjectUserEntity> FilterByClientLegalEntity(IQueryable<FinanceProjectUserEntity> query)
    {
        if (!(_filter?.ClientLegalEntity).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.ClientLegalEntity.Trim()}%";
            query = query.Where(x => x.Project!.Client!.LegalEntity != null
                                && EF.Functions.ILike(x.Project!.Client.LegalEntity, keyword));
        }

        return query;
    }

    private async Task<(List<int> Items, int TotalItemsCount)> GetPageUsersIdsAsync(
        IQueryable<FinanceProjectUserEntity> query,
        CancellationToken token)
    {
        var orderedUsers = ApplyUsersOrdering(query);
        var paginatedUsers = (_skip is not null && _take is not null)
            ? orderedUsers.Skip(_skip.Value).Take(_take.Value)
            : orderedUsers;

        var usersCount = await orderedUsers.CountAsync(token);
        var usersIdsList = await paginatedUsers.Select(x => x.Id).ToListAsync(token);

        return (usersIdsList, usersCount);
    }

    private IQueryable<FinanceUserEntity> ApplyUsersOrdering(IQueryable<FinanceProjectUserEntity> query)
    {
        var users = query.Select(x => x.User!).Distinct();

        if (_ordering == null)
        {
            return users;
        }

        users = _ordering.Field switch
        {
            FinanceOrderingField.UserId =>
                _ordering.IsDescending
                    ? users.OrderByDescending(x => x.Id)
                    : users.OrderBy(x => x.Id),

            FinanceOrderingField.UserFullName => OrderUsersByName(users),

            FinanceOrderingField.DepartmentId =>
                _ordering.IsDescending
                ? users.OrderByDescending(x => x.DepartmentId)
                : users.OrderBy(x => x.DepartmentId),

            _ => users
        };

        return users;
    }

    private IQueryable<ProjectRateEntity> GetTargetRates(IQueryable<FinanceProjectUserEntity> pageProjectUserAssigns)
    {
        var assignedsUserIds = pageProjectUserAssigns.Select(x => x.UserId);
        var assignedProjectIds = pageProjectUserAssigns.Select(x => x.ProjectId);
        var rates = _dbContext.ProjectRates
            .Include(x => x.Currency)
            .Where(x => assignedsUserIds.Contains(x.UserId) && assignedProjectIds.Contains(x.ProjectId)
                && x.RateFrom <= _lastDayInMonth
                && (x.RateTo == null || x.RateTo >= _firstDayInMonth));

        return rates;
    }

    private IQueryable<FinanceUserEntity> OrderUsersByName(
    IQueryable<FinanceUserEntity> users)
    {
        Expression<Func<FinanceUserEntity, string>> nameToOrder = x =>
            string.IsNullOrEmpty(x.LastNameRu) && string.IsNullOrEmpty(x.FirstNameRu)
                ? (x.LastName + " " + x.FirstName).Trim()
                : !string.IsNullOrEmpty(x.LastNameRu) && !string.IsNullOrEmpty(x.FirstNameRu)
                    ? x.LastNameRu + " " + x.FirstNameRu
                    : (x.LastNameRu ?? x.FirstNameRu)!;

        var usersOrderedByNameLanguage = users.OrderBy(x => string.IsNullOrEmpty(x.LastNameRu) && string.IsNullOrEmpty(x.FirstNameRu));

        return _ordering!.IsDescending
            ? usersOrderedByNameLanguage.ThenByDescending(nameToOrder)
            : usersOrderedByNameLanguage.ThenBy(nameToOrder);
    }
}

internal class GetProjectRatesQueryBuilderDto
{
    public required List<int> UsersIds { get; set; }
    public required HashSet<int> ManagedProjectUsersIds { get; set; }
    public int TotalUsersCount { get; set; }
    public required List<IGrouping<int, FinanceProjectUserEntity>> ProjectUsers { get; set; }
    public required List<IGrouping<int, ProjectRateEntity>> Rates { get; set; }
}
