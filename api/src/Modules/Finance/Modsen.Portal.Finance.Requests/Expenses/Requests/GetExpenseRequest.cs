using FluentValidation;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Builders;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.Expenses.Requests;

internal class GetExpenseRequest : IQueryHandler<GetExpenseRequest.Query, ExpensesTabPageCoreDto>
{
    private readonly IExpenseEncryptor _expenseEncryptor;
    private readonly IExpensesQueryBuilder _expensesQueryBuilder;

    public GetExpenseRequest(IExpenseEncryptor expenseEncryptor, IExpensesQueryBuilder expensesQueryBuilder)
    {
        _expenseEncryptor = expenseEncryptor;
        _expensesQueryBuilder = expensesQueryBuilder;
    }

    public async Task<ExpensesTabPageCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var expensesResult = await _expensesQueryBuilder
            .ForYear(request.Year)
            .WithFilter(request.Filter)
            .WithOrdering(request.Ordering)
            .WithPagination(request.PageNumber, request.PageSize)
            .BuildAsync(cancellationToken);

        var categoriesExpenses = new List<CategoryExpensesCoreDto>();
        var sumsByMonth = new decimal[12];

        foreach (var category in expensesResult.Categories)
        {
            if (category.Expenses.Count == 0)
            {
                categoriesExpenses.Add(new CategoryExpensesCoreDto
                {
                    Category = ExpenseCategoryCoreDto.Of(category),
                    ExpensesByMonth = new List<ExpenseShortCoreDto?>()
                });

                continue;
            }

            var currentCategoryExpensesByMonth = new List<ExpenseShortCoreDto>();
            foreach (var e in category.Expenses.OrderBy(x => x.MonthOfYear))
            {
                var expenseAmount = _expenseEncryptor.Decrypt(e.ExpenseAmount);
                sumsByMonth[e.MonthOfYear - 1] += expenseAmount;

                currentCategoryExpensesByMonth.Add(new ExpenseShortCoreDto
                {
                    Id = e.Id,
                    MonthOfYear = e.MonthOfYear,
                    ExpenseAmount = expenseAmount
                });
            }

            categoriesExpenses.Add(new CategoryExpensesCoreDto
            {
                Category = ExpenseCategoryCoreDto.Of(category),
                ExpensesByMonth = currentCategoryExpensesByMonth!
            });
        }

        var result = new ExpensesTabPageCoreDto
        {
            CategoriesExpenses = PageableListOfItemsExtensions
                .ToPageable(categoriesExpenses)
                .WithPaging(request.PageNumber, request.PageSize)
                .WithTotal(expensesResult.TotalCount),
            ExpensesSumsByMonth = sumsByMonth,
        };

        return result;
    }

    internal sealed record Query : IQuery<ExpensesTabPageCoreDto>
    {
        public int Year { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public ExpenseFilterCoreDto Filter { get; set; } = new();
        public ExpenseOrderingCoreDto Ordering { get; set; } = new();
    };

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Year)
                .NotNull()
                .GreaterThanOrEqualTo(2020);
        }
    }
}
