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

internal sealed class AddUserToResourceGroupRequest : ICommandHandler<AddUserToResourceGroupRequest.Command, UserDto>
{
    private readonly ICanGroupMemberBeAddedBusinessRule _canGroupMemberBeAddedBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly IUserLoader _userLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public AddUserToResourceGroupRequest(
        ICanGroupMemberBeAddedBusinessRule canGroupMemberBeAddedBusinessRule,
        IIdentityDbContext identityDbContext,
        ICommunicationBus communicationBus,
        IUserLoader userLoader,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _canGroupMemberBeAddedBusinessRule = canGroupMemberBeAddedBusinessRule;
        _identityDbContext = identityDbContext;
        _communicationBus = communicationBus;
        _userLoader = userLoader;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var businessRuleResult = await _canGroupMemberBeAddedBusinessRule.IsCorrespondsToBusiness(
            new CanGroupMemberBeAddedBusinessRule.Request(request.UserId, request.GroupId),
            cancellationToken);

        if (!businessRuleResult.IsSuccess)
        {
            throw new ActionNotAllowedException(businessRuleResult.Reason!);
        }

        var userEntity = await _identityDbContext.Users
            .Where(user => user.Id == request.UserId && user.IsActive)
            .Include(user => user.UserRoles)
            .ThenInclude(roles => roles.Role)
            .FirstOrDefaultAsync(cancellationToken);

        if (userEntity == null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        userEntity.ResourceGroupId = request.GroupId;

        await _identityDbContext.SaveChanges(cancellationToken);

        var user = await _userLoader.LoadAsync(userEntity.Id, cancellationToken);

        var message = new UserUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            LastName = user.LastName ?? string.Empty,
            FirstName = user.FirstName,
            LastNameRu = user.LastNameRu,
            FirstNameRu = user.FirstNameRu,
            PatronymicRu = user.PatronymicRu,
            DepartmentId = user.DepartmentId,
            Email = user.Email,
            CountryId = user.CountryId,
            Color = user.Color,
            AvatarFileId = user.AvatarFileId,
            Position = user.Position,
            PositionOrderWeight = user.PositionOrderWeight!.Value,
            Location = user.Location,
            ResourceGroupId = user.ResourceGroupId,
            RegisteredLocationId = user.RegisteredLocationId,
            AvatarColor = user.AvatarColor,
            CurrentLocation = user.CurrentLocation,
            HireDate = user.HireDate,
            InitiatorId = _authenticatedUserContext.UserId
        };

        await _communicationBus.PublishAsync(message, cancellationToken);

        return user;
    }

    internal sealed record Command(int GroupId, int UserId) : ICommand<UserDto>, IIdentityRequest, ITransactionalRequest;
}
