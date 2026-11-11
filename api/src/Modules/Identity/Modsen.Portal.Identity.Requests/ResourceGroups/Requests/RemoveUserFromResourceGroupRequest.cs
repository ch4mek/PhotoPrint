using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class RemoveUserFromResourceGroupRequest : ICommandHandler<RemoveUserFromResourceGroupRequest.Command, UserDto>
{
    private readonly ICanGroupMemberBeRemovedBusinessRule _canGroupMemberBeRemovedBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly IUserLoader _userLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public RemoveUserFromResourceGroupRequest(
        ICanGroupMemberBeRemovedBusinessRule canGroupMemberBeRemovedBusinessRule,
        IIdentityDbContext identityDbContext,
        ICommunicationBus communicationBus,
        IUserLoader userLoader,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canGroupMemberBeRemovedBusinessRule = canGroupMemberBeRemovedBusinessRule;
        _identityDbContext = identityDbContext;
        _communicationBus = communicationBus;
        _userLoader = userLoader;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var businessRuleResult = await _canGroupMemberBeRemovedBusinessRule.IsCorrespondsToBusiness(
            new CanGroupMemberBeRemovedBusinessRule.Request(request.UserId, request.GroupId),
            cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason!);
        }

        var user = await _identityDbContext.Users
            .Include(user => user.UserRoles)
            .ThenInclude(roles => roles.Role)
            .Where(user => user.Id == request.UserId && user.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        user.ResourceGroupId = null;

        await _identityDbContext.SaveChanges(cancellationToken);

        var domainUser = await _userLoader.LoadAsync(user.Id, cancellationToken);

        var message = new UserUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = domainUser.Id,
            LastName = domainUser.LastName ?? string.Empty,
            FirstName = domainUser.FirstName,
            LastNameRu = domainUser.LastNameRu,
            FirstNameRu = domainUser.FirstNameRu,
            PatronymicRu = domainUser.PatronymicRu,
            DepartmentId = domainUser.DepartmentId,
            Email = domainUser.Email,
            CountryId = domainUser.CountryId,
            Color = domainUser.Color,
            AvatarFileId = user.AvatarFileId,
            Position = domainUser.Position,
            PositionOrderWeight = domainUser.PositionOrderWeight!.Value,
            Location = domainUser.Location,
            ResourceGroupId = domainUser.ResourceGroupId,
            RegisteredLocationId = domainUser.RegisteredLocationId,
            AvatarColor = domainUser.AvatarColor,
            CurrentLocation = domainUser.CurrentLocation,
            HireDate = domainUser.HireDate,
            InitiatorId = _authenticatedUserContext.UserId,
        };

        await _communicationBus.PublishAsync(message, cancellationToken);

        return domainUser;
    }

    internal sealed record Command(int GroupId, int UserId) : ICommand<UserDto>, ITransactionalRequest;
}
