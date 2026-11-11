using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;

internal class DeleteExpenseCategoryRequest : ICommandHandler<DeleteExpenseCategoryRequest.Command, ExpenseCategoryCoreDto>
{
    private readonly IFinanceDbContext _dbContext;

    public DeleteExpenseCategoryRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseCategoryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.ExpenseCategories
            .Include(x => x.Expenses)
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ExpenseCategoryNotFound);

        var expenses = existing.Expenses;
        foreach (var expense in expenses)
        {
            expense.IsDeleted = true;
        }

        existing.IsDeleted = true;

        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = ExpenseCategoryCoreDto.Of(existing);

        return result;
    }

    internal sealed record Command(int CategoryId) : ICommand<ExpenseCategoryCoreDto>, ITransactionalRequest;
}
