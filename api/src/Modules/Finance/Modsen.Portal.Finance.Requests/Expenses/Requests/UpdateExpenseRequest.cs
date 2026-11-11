using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.Expenses.Requests;

internal class UpdateExpenseRequest : ICommandHandler<UpdateExpenseRequest.Command, ExpenseCoreDto>
{
    private readonly IExpenseEncryptor _expenseEncryptor;
    private readonly IFinanceDbContext _dbContext;

    public UpdateExpenseRequest(IExpenseEncryptor numberEncryptor, IFinanceDbContext dbContext)
    {
        _expenseEncryptor = numberEncryptor;
        _dbContext = dbContext;
    }

    public async Task<ExpenseCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var expense = await _dbContext.Expenses
                .Include(e => e.ExpenseCategory)
                .FirstOrDefaultAsync(e => e.Id == request.ExpenseId && !e.IsDeleted, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ExpenseNotFound);

        expense.ExpenseAmount = _expenseEncryptor.Encrypt(request.ExpenseAmount)!;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ExpenseCoreDto()
        {
            Id = expense.Id,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            ExpenseCategory = ExpenseCategoryCoreDto.Of(expense.ExpenseCategory!),
            Year = expense.Year,
            MonthOfYear = expense.MonthOfYear,
            ExpenseAmount = request.ExpenseAmount
        };
    }

    internal sealed record Command(int ExpenseId, decimal ExpenseAmount) : ICommand<ExpenseCoreDto>, ITransactionalRequest; 

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ExpenseAmount)
                .NotNull()
                .InclusiveBetween(0m, 999999.99m)
                .Money();
        }
    }
}
