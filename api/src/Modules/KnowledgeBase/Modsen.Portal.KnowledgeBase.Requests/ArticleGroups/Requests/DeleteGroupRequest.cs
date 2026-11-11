using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class DeleteGroupRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext,
    IAuthenticatedUserContext authenticatedUserContext) : ICommandHandler<DeleteGroupRequest.Command, int>
{
    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var group = await knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .Where(group => group.Id == request.Id)
            .Select(group => new
            {
                ParentGroupId = group.ParentGroup,
                ArticleCount = group.Articles.Count,
                ChildGroupsCount = group.ChildGroups.Count(childGroup => !childGroup.IsDeleted),
                Entity = group,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            throw new EntityNotFoundException($"There is no group with Id {request.Id}");
        }

        if (group.ParentGroupId != null && !authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageSubgroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage SubGroup");
        }

        if (group.ParentGroupId == null && !authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageGroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage Group");
        }

        if (group.ArticleCount > 0 || group.ChildGroupsCount > 0)
        {
            var message = group.ParentGroupId is null ?
                "Group contains subgroups or articles" :
                "Subgroup contains subgroups or articles";

            throw new EntityConflictException(message);
        }

        group.Entity.IsDeleted = true;

        await knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
