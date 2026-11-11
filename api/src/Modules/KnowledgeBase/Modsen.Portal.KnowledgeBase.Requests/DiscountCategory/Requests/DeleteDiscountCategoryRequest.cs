using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class DeleteDiscountCategoryRequest : ICommandHandler<DeleteDiscountCategoryRequest.Command, int>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public DeleteDiscountCategoryRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _knowledgeBaseDbContext.DiscountCategories
            .Include(x => x.DiscountCategoryTags)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no discount category with Id {request.Id}");

        if (await _knowledgeBaseDbContext.Discounts.AnyAsync(x => x.DiscountCategoryId == request.Id, cancellationToken))
        {
            throw new EntityConflictException("You cannot delete discount category that contains cards");
        }

        _knowledgeBaseDbContext.DiscountCategoryTags.RemoveRange(entity.DiscountCategoryTags);

        _knowledgeBaseDbContext.DiscountCategories.Remove(entity);

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
