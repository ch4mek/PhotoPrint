using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Finance.Requests.Core.CoreDto;

namespace Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;

internal class CreateExpenseCategoryRequest : ICommandHandler<CreateExpenseCategoryRequest.Command, ExpenseCategoryCoreDto>
{
    private readonly IFinanceDbContext _dbContext;

    public CreateExpenseCategoryRequest(IFinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseCategoryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isCategoryNameUsed = await _dbContext.ExpenseCategories
            .AnyAsync(ec => ec.Name == request.CategoryName && !ec.IsDeleted, cancellationToken);

        if (isCategoryNameUsed)
        {
            throw new InvalidDataAppException(PortalMessages.ExpenseCategoryExists);
        }

        var lastOrder = await _dbContext.ExpenseCategories
            .Select(x => (int?)x.OrderWeight)
            .MaxAsync(cancellationToken) ?? 0;

        var newCategory = new ExpenseCategoryEntity
        {
            Name = request.CategoryName,
            OrderWeight = lastOrder + 1,
            IsDeleted = false
        };

        await _dbContext.ExpenseCategories.AddAsync(newCategory, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = ExpenseCategoryCoreDto.Of(newCategory); 

        return result;
    }



    internal sealed record Command(string CategoryName) : ICommand<ExpenseCategoryCoreDto>, ITransactionalRequest;

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
