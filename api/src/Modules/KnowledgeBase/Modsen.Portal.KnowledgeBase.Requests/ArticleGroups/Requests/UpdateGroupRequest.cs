using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class UpdateGroupRequest : ICommandHandler<UpdateGroupRequest.Command, GroupDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly IGroupLoader _groupLoader;
    private readonly IIsGroupWithSameNameExistBusinessRule _isGroupWithSameNameExistBusinessRule;

    public UpdateGroupRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IGroupLoader groupLoader,
        IIsGroupWithSameNameExistBusinessRule isGroupWithSameNameExistBusinessRule)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _groupLoader = groupLoader;
        _isGroupWithSameNameExistBusinessRule = isGroupWithSameNameExistBusinessRule;
    }

    public async Task<GroupDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .FirstOrDefaultAsync(group => group.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no group with Id {request.Id}");

        _ = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .FirstOrDefaultAsync(group => group.Id == request.ParentGroupId, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no parent group with Id {request.ParentGroupId}");

        if (entity.ParentGroupId != null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageSubgroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage SubGroup");
        }

        if (entity.ParentGroupId == null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageGroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage Group");
        }

        var (isContain, message) = await _isGroupWithSameNameExistBusinessRule.IsCorrespondToRuleAsync(
            entity.ParentGroupId, request.Name, cancellationToken, request.Id);

        if (isContain)
        {
            throw new InvalidDataAppException(message!);
        }

        entity.ParentGroupId = request.ParentGroupId;
        entity.Name = request.Name;
        entity.Icon = request.Icon;
        entity.IsActive = request.IsActive;
        entity.IsAvailableOnMainScreen = request.IsAvailableOnMainScreen;
        entity.UpdatedAtUtc = _clockService.UtcNow;
        entity.UpdatedById = _authenticatedUserContext.UserId;

        var existingChildGroups = entity.ChildGroups.Select(g => g.Id).ToList();
        var newChildGroups = request.ChildGroups ?? [];
        var childGroupsToAdd = newChildGroups.Except(existingChildGroups).ToList();

        foreach (var groupId in childGroupsToAdd)
        {
            var childGroup = await _knowledgeBaseDbContext.Groups.FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
            if (childGroup is not null)
            {
                entity.ChildGroups.Add(childGroup);
            }
        }

        var childGroupsToDelete = existingChildGroups.Except(newChildGroups).ToList();
        entity.ChildGroups.RemoveAll(childGroup => childGroupsToDelete.Contains(childGroup.Id));

        var existingArticles = entity.Articles.Select(a => a.Id).ToList();
        var newArticles = request.Articles;
        var articlesToAdd = newArticles.Except(existingArticles).ToList();

        foreach (var a in articlesToAdd)
        {
            var article = await _knowledgeBaseDbContext.Articles.FirstOrDefaultAsync(x => x.Id == a, cancellationToken);
            if (article is not null)
            {
                entity.Articles.Add(article);
            }
        }

        var articlesToDelete = existingArticles.Except(newArticles).ToList();
        entity.Articles.RemoveAll(a => articlesToDelete.Contains(a.Id));

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        var groupDto = await _groupLoader.LoadAsync(entity.Id, cancellationToken);

        return groupDto;
    }

    internal sealed record Command(
        int Id,
        int ParentGroupId,
        IReadOnlyCollection<int>? ChildGroups,
        string Name,
        string? Icon,
        bool IsActive,
        bool IsAvailableOnMainScreen,
        IReadOnlyCollection<int> Articles)
        : ICommand<GroupDto>, ITransactionalRequest;

    internal sealed class UpdateGroupRequestValidator : AbstractValidator<Command>
    {
        public UpdateGroupRequestValidator()
        {
            RuleFor(command => command.Name)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(250);
        }
    }
}
