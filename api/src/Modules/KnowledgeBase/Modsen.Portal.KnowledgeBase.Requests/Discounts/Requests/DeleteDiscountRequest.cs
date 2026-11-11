using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class DeleteDiscountRequest : ICommandHandler<DeleteDiscountRequest.Command, int>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly ICommunicationBus _communicationBus;

    public DeleteDiscountRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        ICommunicationBus communicationBus)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _communicationBus = communicationBus;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var discount = await _knowledgeBaseDbContext.Discounts
            .Where(discount => discount.Id == request.Id)
            .Include(discount => discount.DiscountCities)
            .Include(discount => discount.DiscountTags)
            .Include(discount => discount.DiscountReferences)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"There is no discount with Id {request.Id}");

        if (discount.DiscountTags.Count > 0)
        {
            _knowledgeBaseDbContext.DiscountTags.RemoveRange(discount.DiscountTags);
        }

        if (discount.DiscountCities.Count > 0)
        {
            _knowledgeBaseDbContext.DiscountCities.RemoveRange(discount.DiscountCities);
        }

        if (discount.DiscountReferences.Count > 0)
        {
            _knowledgeBaseDbContext.DiscountsReferences.RemoveRange(discount.DiscountReferences);
        }

        if (discount.ImageFileId is not null)
        {
            var deleteFileNotification = new DeleteFileEvent(discount.ImageFileId, Guid.CreateVersion7().ToString());

            await _communicationBus.PublishAsync(deleteFileNotification, cancellationToken);
        }

        _knowledgeBaseDbContext.Discounts.Remove(discount);

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
