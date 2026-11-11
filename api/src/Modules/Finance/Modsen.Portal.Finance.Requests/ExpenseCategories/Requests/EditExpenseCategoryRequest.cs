using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;

internal class EditExpenseCategoryRequest : ICommandHandler<EditExpenseCategoryRequest.Command, ExpenseCategoryCoreDto>
{
    private readonly IFinanceDbContext _dbContext;

    public EditExpenseCategoryRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseCategoryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.ExpenseCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ExpenseCategoryNotFound);

        if (existing.Name != request.CategoryName)
        {
            var isCategoryNameUsed = await _dbContext.ExpenseCategories
                .AnyAsync(ec => ec.Name == request.CategoryName && !ec.IsDeleted, cancellationToken);

            if (isCategoryNameUsed)
            {
                throw new InvalidDataAppException(PortalMessages.ExpenseCategoryExists);
            }

            existing.Name = request.CategoryName;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        var result = ExpenseCategoryCoreDto.Of(existing);

        return result;
    }

    internal sealed record Command(int CategoryId, string CategoryName) : ICommand<ExpenseCategoryCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.CategoryName)
                .NotNull()
                .NotEmpty()
                .ShortTextType()
                .NotOnlyPunctuationMarks();
        }
    }
}
