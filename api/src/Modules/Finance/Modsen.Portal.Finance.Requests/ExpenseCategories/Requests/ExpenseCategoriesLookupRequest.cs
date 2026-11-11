using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;

internal class ExpenseCategoriesLookupRequest : IQueryHandler<ExpenseCategoriesLookupRequest.Query, List<string>>
{
    private readonly IFinanceDbContext _dbContext;

    public ExpenseCategoriesLookupRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> Handle(Query request, CancellationToken cancellationToken)
    {
        var categories = await _dbContext.ExpenseCategories
            .Where(x => !x.IsDeleted)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken);

        return categories;
    }

    internal sealed record Query : IQuery<List<string>>;
}
