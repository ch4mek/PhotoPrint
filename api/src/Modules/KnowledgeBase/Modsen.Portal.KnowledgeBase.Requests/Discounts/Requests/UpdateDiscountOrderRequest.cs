using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class UpdateDiscountOrderRequest : ICommandHandler<UpdateDiscountOrderRequest.Command, Unit>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public UpdateDiscountOrderRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var discountsToUpdate = await _knowledgeBaseDbContext.Discounts
            .Where(x => request.Orders.Select(f => f.DiscountId).Contains(x.Id))
            .ToListAsync(cancellationToken);

        _ = discountsToUpdate.Join(
            inner: request.Orders,
            outerKeySelector: discountToUpdate => discountToUpdate.Id,
            innerKeySelector: newOrder => newOrder.DiscountId,
            resultSelector: (discountToUpdate, newOrder) =>
            {
                discountToUpdate.OrderWeight = newOrder.OrderWeight;

                return discountToUpdate;
            }
            ).ToList();

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    internal sealed record Command : ICommand<Unit>, ITransactionalRequest
    {
        public IReadOnlyCollection<DiscountOrderDto> Orders { get; set; } = [];
    }
}
