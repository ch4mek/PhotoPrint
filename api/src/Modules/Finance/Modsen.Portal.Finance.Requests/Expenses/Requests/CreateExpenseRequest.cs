using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;

namespace Modsen.Portal.Finance.Requests.Expenses.Requests;

internal class CreateExpenseRequest : ICommandHandler<CreateExpenseRequest.Command, ExpenseCoreDto>
{
    private readonly IExpenseEncryptor _expenseEncryptor;
    private readonly IFinanceDbContext _dbContext;

    public CreateExpenseRequest(IExpenseEncryptor numberEncryptor, IFinanceDbContext dbContext)
    {
        _expenseEncryptor = numberEncryptor;
        _dbContext = dbContext;
    }

    public async Task<ExpenseCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.ExpenseCategories
                .FirstOrDefaultAsync(c => c.Id == request.ExpenseCategoryId && !c.IsDeleted, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ExpenseCategoryNotFound);

        var expenceExists = await _dbContext.Expenses.AnyAsync(e => e.Year == request.Year && e.MonthOfYear == request.MonthOfYear
                                                        && e.ExpenseCategoryId == request.ExpenseCategoryId, cancellationToken);
        if (expenceExists)
        {
            throw new InvalidDataAppException(PortalMessages.ExpenseAlreadyExists);
        }

        var newExpense = new ExpenseEntity
        {
            ExpenseCategoryId = request.ExpenseCategoryId,
            Year = request.Year,
            MonthOfYear = request.MonthOfYear,
            ExpenseAmount = _expenseEncryptor.Encrypt(request.ExpenseAmount)!,
            IsDeleted = false
        };

        await _dbContext.Expenses.AddAsync(newExpense, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ExpenseCoreDto()
        {
            Id = newExpense.Id,
            ExpenseCategoryId = newExpense.ExpenseCategoryId,
            ExpenseCategory = ExpenseCategoryCoreDto.Of(category),
            Year = newExpense.Year,
            MonthOfYear = newExpense.MonthOfYear,
            ExpenseAmount = request.ExpenseAmount
        };
    }

    internal sealed record Command : ICommand<ExpenseCoreDto>, ITransactionalRequest
    {
        public required int ExpenseCategoryId { get; set; }
        public required int Year { get; set; }
        public required int MonthOfYear { get; set; }
        public required decimal ExpenseAmount { get; set; }
    };

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ExpenseCategoryId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(x => x.Year)
                .NotNull()
                .GreaterThanOrEqualTo(2020);

            RuleFor(x => x.MonthOfYear)
                .NotNull()
                .InclusiveBetween(1, 12);

            RuleFor(x => x.ExpenseAmount)
                .NotNull()
                .InclusiveBetween(0m, 999999.99m)
                .Money();
        }
    }
}
