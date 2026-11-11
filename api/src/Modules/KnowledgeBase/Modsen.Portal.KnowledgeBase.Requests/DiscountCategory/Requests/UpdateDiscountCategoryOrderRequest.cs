using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class UpdateDiscountCategoryOrderRequest : ICommandHandler<UpdateDiscountCategoryOrderRequest.Command, Unit>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public UpdateDiscountCategoryOrderRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var categoriesToUpdate = await _knowledgeBaseDbContext.DiscountCategories
            .Where(x => request.Orders.Select(f => f.DiscountCategoryId).Contains(x.Id))
            .ToListAsync(cancellationToken);

        _ = categoriesToUpdate.Join(
            inner: request.Orders,
            outerKeySelector: categoryToUpdate => categoryToUpdate.Id,
            innerKeySelector: newOrder => newOrder.DiscountCategoryId,
            resultSelector: (categoryToUpdate, newOrder) =>
            {
                categoryToUpdate.OrderWeight = newOrder.OrderWeight;

                return categoryToUpdate;
            }
            ).ToList();

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    internal sealed record Command : ICommand<Unit>, ITransactionalRequest
    {
        public IReadOnlyCollection<DiscountCategoryOrderDto> Orders { get; set; } = [];
    }
}
