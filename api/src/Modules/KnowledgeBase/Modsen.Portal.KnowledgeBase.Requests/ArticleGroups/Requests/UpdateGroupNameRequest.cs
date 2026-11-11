using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class UpdateGroupNameRequest : ICommandHandler<UpdateGroupNameRequest.Command, string>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIsGroupWithSameNameExistBusinessRule _isGroupWithSameNameExistBusinessRule;

    public UpdateGroupNameRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIsGroupWithSameNameExistBusinessRule isGroupWithSameNameExistBusinessRule)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _isGroupWithSameNameExistBusinessRule = isGroupWithSameNameExistBusinessRule;
    }

    public async Task<string> Handle(Command request, CancellationToken cancellationToken)
    {
        var groupEntity = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .FirstOrDefaultAsync(group => group.Id == request.Id, cancellationToken);

        if (groupEntity is null)
        {
            throw new EntityNotFoundException("Group is not exist");
        }

        if (groupEntity.ParentGroupId != null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageSubgroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage SubGroup");
        }

        if (groupEntity.ParentGroupId == null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageGroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage Group");
        }

        var (isContain, message) = await _isGroupWithSameNameExistBusinessRule.IsCorrespondToRuleAsync(
            groupEntity.ParentGroupId, request.Name, cancellationToken, request.Id);

        if (isContain)
        {
            throw new InvalidDataAppException(message!);
        }

        groupEntity.Name = request.Name;

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return request.Name;
    }

    internal sealed record Command(int Id, string Name) : ICommand<string>, ITransactionalRequest;

    internal sealed class UpdateGroupNameRequestValidator : AbstractValidator<Command>
    {
        public UpdateGroupNameRequestValidator()
        {
            RuleFor(command => command.Name)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(250);
        }
    }
}
