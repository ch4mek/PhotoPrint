using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.Expenses.Core.Builders;

internal interface IExpensesQueryBuilder
{
    IExpensesQueryBuilder ForYear(int year);
    IExpensesQueryBuilder WithFilter(ExpenseFilterCoreDto filter);
    IExpensesQueryBuilder WithOrdering(ExpenseOrderingCoreDto ordering);
    IExpensesQueryBuilder WithPagination(int pageNumber, int pageSize);
    Task<ExpensesQueryBuilderDto> BuildAsync(CancellationToken token);
}

internal class ExpensesQueryBuilder : IExpensesQueryBuilder
{
    private readonly IFinanceDbContext _dbContext;

    private int _year;
    private int _skip;
    private int _take;
    private ExpenseFilterCoreDto? _filter;
    private ExpenseOrderingCoreDto? _ordering;

    public ExpensesQueryBuilder(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IExpensesQueryBuilder ForYear(int year)
    {
        _year = year;

        return this;
    }

    public IExpensesQueryBuilder WithFilter(ExpenseFilterCoreDto filter)
    {
        _filter = filter;
        return this;
    }

    public IExpensesQueryBuilder WithOrdering(ExpenseOrderingCoreDto ordering)
    {
        _ordering = ordering;
        return this;
    }

    public IExpensesQueryBuilder WithPagination(int pageNumber, int pageSize)
    {
        _skip = pageNumber * pageSize;
        _take = pageSize;

        return this;
    }

    public async Task<ExpensesQueryBuilderDto> BuildAsync(CancellationToken token)
    {
        var categoriesQuery = _dbContext.ExpenseCategories
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        if (!(_filter?.CategoryName).IsNullOrEmpty())
        {
            var keyword = $"%{_filter.CategoryName.Trim()}%";
            categoriesQuery = categoriesQuery.Where(x => EF.Functions.ILike(x.Name, keyword));
        }

        var pagedCategories = await GetCategoriesIdsAsync(categoriesQuery, token);

        var categoriesWithExpenses = await pagedCategories.Categories.Include(c => c.Expenses.Where(e => e.Year == _year && !e.IsDeleted))
            .ToListAsync(token);

        var result = new ExpensesQueryBuilderDto
        {
            Categories = categoriesWithExpenses,
            TotalCount = pagedCategories.TotalCount,
        };

        return result;
    }

    private async Task<(IQueryable<ExpenseCategoryEntity> Categories, int TotalCount)> GetCategoriesIdsAsync(
    IQueryable<ExpenseCategoryEntity> categories, CancellationToken token)
    {
        var orderedCategories = _ordering is null
            ? categories
            : _ordering.Field switch
            {
                ExpenseOrderingField.OrderWeight => categories.OrderByDirection(x => x.OrderWeight, _ordering.IsDescending),

                ExpenseOrderingField.CategoryName => categories.OrderByDirection(x => x.Name, _ordering.IsDescending),

                _ => categories
            };

        var categoriesCount = await orderedCategories.CountAsync(token);

        var paginated = orderedCategories.Skip(_skip).Take(_take);

        return (paginated, categoriesCount);
    }
}
