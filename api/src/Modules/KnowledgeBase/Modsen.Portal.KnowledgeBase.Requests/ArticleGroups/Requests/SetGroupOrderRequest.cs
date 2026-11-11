using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class SetGroupOrderRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    : ICommandHandler<SetGroupOrderRequest.Command, Unit>
{
    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        foreach (var group in request.GroupOrders)
        {
            await UpdateGroupAsync(group, cancellationToken);
        }

        await knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task UpdateGroupAsync(GroupOrderDto group, CancellationToken cancellationToken)
    {
        var groupEntity = await knowledgeBaseDbContext.Groups
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(groupEntity => groupEntity.Id == group.Id, cancellationToken);

        if (groupEntity is null)
        {
            throw new EntityNotFoundException("Group not found");
        }

        groupEntity.ParentGroupId = group.ParentGroupId;
        groupEntity.OrderWeight = group.OrderWeight;

        foreach (var articleOrder in group.ArticleOrders)
        {
            var articleEntity = await knowledgeBaseDbContext.Articles
                .FirstOrDefaultAsync(article => article.Id == articleOrder.Id, cancellationToken);

            if (articleEntity is null)
            {
                throw new InvalidDataAppException("Article is not exist");
            }

            articleEntity.GroupId = groupEntity.Id;
            articleEntity.OrderWeight = articleOrder.OrderWeight;
        }

        foreach (var childGroup in group.ChildGroups)
        {
            await UpdateGroupAsync(childGroup, cancellationToken);
        }
    }

    internal sealed record Command(IReadOnlyCollection<GroupOrderDto> GroupOrders) : ICommand<Unit>, ITransactionalRequest;
}
