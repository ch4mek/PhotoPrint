using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;
using Modsen.Portal.KnowledgeBase.Requests.Core.Calculators;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class CreateGroupRequest : ICommandHandler<CreateGroupRequest.Command, GroupDto>
{
    private readonly IKnowledgeBaseDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly IGroupLoader _groupLoader;
    private readonly IMaxOrderWeightCalculator _maxOrderWeightCalculator;
    private readonly IIsGroupWithSameNameExistBusinessRule _isGroupWithSameNameExistBusinessRule;

    public CreateGroupRequest(
        IKnowledgeBaseDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IGroupLoader groupLoader,
        IMaxOrderWeightCalculator maxOrderWeightCalculator,
        IIsGroupWithSameNameExistBusinessRule isGroupWithSameNameExistBusinessRule)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _groupLoader = groupLoader;
        _maxOrderWeightCalculator = maxOrderWeightCalculator;
        _isGroupWithSameNameExistBusinessRule = isGroupWithSameNameExistBusinessRule;
    }

    public async Task<GroupDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (request.ParentGroupId != null)
        {
            _ = await _groupLoader.LoadAsync(request.ParentGroupId.Value, cancellationToken);
        }

        if (request.ParentGroupId != null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageSubgroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage SubGroup");
        }

        if (request.ParentGroupId == null && !_authenticatedUserContext.HasClaim(WikiClaimTypeNames.ManageGroup.Name))
        {
            throw new ActionNotAllowedException("You have no permissions to manage Group");
        }

        var (isContain, message) = await _isGroupWithSameNameExistBusinessRule.IsCorrespondToRuleAsync(
            request.ParentGroupId, request.Name, cancellationToken);

        if (isContain)
        {
            throw new InvalidDataAppException(message!);
        }

        var groupEntity = new GroupEntity
        {
            ParentGroupId = request.ParentGroupId,
            Name = request.Name,
            Icon = request.Icon,
            IsActive = request.IsActive,
            IsAvailableOnMainScreen = request.IsAvailableOnMainScreen,
            OrderWeight =  await _maxOrderWeightCalculator.CalculateAsync(request.ParentGroupId, cancellationToken),
            CreatedById = _authenticatedUserContext.UserId,
            CreatedAtUtc = _clockService.UtcNow,
        };

        _dbContext.Groups.Add(groupEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var groupDto = await _groupLoader.LoadAsync(groupEntity.Id, cancellationToken);

        return groupDto;
    }

    internal sealed record Command(
        int? ParentGroupId,
        string Name,
        string? Icon,
        bool IsActive,
        bool IsAvailableOnMainScreen)
        : ICommand<GroupDto>, ITransactionalRequest;

    internal sealed class CreateGroupRequestValidator : AbstractValidator<Command>
    {
        public CreateGroupRequestValidator()
        {
            RuleFor(command => command.Name)
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(250);
        }
    }
}
