using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.Resolvers;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class UpdateResourceGroupRequest : ICommandHandler<UpdateResourceGroupRequest.Command, UpdateGroupDto>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICanUserUpdateGroupBusinessRule _canUpdateGroupBusinessRule;
    private readonly IIsNewGroupManagerValidBusinessRule _isGmValid;
    private readonly ICommunicationBus _communicationBus;
    private readonly IUpdateGroupManagerResolver _updateGroupManagerResolver;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UpdateResourceGroupRequest(
        IIdentityDbContext identityDbContext,
        ICanUserUpdateGroupBusinessRule canUserUpdateGroupBusinessRule,
        IIsNewGroupManagerValidBusinessRule isGmValid,
        ICommunicationBus communicationBus,
        IUpdateGroupManagerResolver updateGroupManagerResolver,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _identityDbContext = identityDbContext;
        _canUpdateGroupBusinessRule = canUserUpdateGroupBusinessRule;
        _isGmValid = isGmValid;
        _communicationBus = communicationBus;
        _updateGroupManagerResolver = updateGroupManagerResolver;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UpdateGroupDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        var group = await _identityDbContext.ResourceGroups
            .FirstAsync(group => group.Id == request.Id && group.IsActive, cancellationToken);

        group.Name = request.Name;

        var updateGmDbResponse = await _updateGroupManagerResolver.ResolveAsync(
            new UpdateGroupManagerResolver.Request(request.Id, request.GroupManagerId), cancellationToken);

        await _identityDbContext.SaveChanges(cancellationToken);

        await PublishEvents(updateGmDbResponse, cancellationToken);

        return new UpdateGroupDto(request.Id, request.GroupManagerId, group.Name);
    }

    private async Task ValidateRequest(Command request, CancellationToken cancellationToken)
    {
        var groupDepartmentId = await _identityDbContext.ResourceGroups
            .Where(group => group.Id == request.Id)
            .Select(group => group.DepartmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (groupDepartmentId is null)
        {
            throw new EntityNotFoundException(PortalMessages.GroupNotFound);
        }

        if (groupDepartmentId != request.DepartmentId)
        {
            throw new InvalidDataAppException("You can not change the department of group");
        }

        var isUserCanUpdateGroup = await _canUpdateGroupBusinessRule.IsCorrespondsToBusiness(
            new CanUserUpdateGroupBusinessRule.Request(request.DepartmentId), cancellationToken);

        if (!isUserCanUpdateGroup.IsSuccess)
        {
            throw new ActionNotAllowedException(isUserCanUpdateGroup.Reason!);
        }

        var groupManagerId = await _identityDbContext.Users
            .Where(user => user.ResourceGroupId == request.Id && user.IsActive)
            .Where(user => user.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
            .Select(user => user.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var isNewGroupManagerSet = groupManagerId != request.GroupManagerId && request.GroupManagerId != null;

        if (isNewGroupManagerSet)
        {
            var isGmValid = await _isGmValid.IsCorrespondsToBusiness(
                new IsNewGroupManagerValidBusinessRule.Request(request.GroupManagerId!.Value, request.DepartmentId), cancellationToken);
            if (!isGmValid.IsSuccess)
            {
                throw new InvalidDataAppException(isGmValid.Reason!);
            }
        }

        var isNameExist = await _identityDbContext.ResourceGroups
            .AnyAsync(group => group.IsActive && group.Name == request.Name && group.Id != request.Id, cancellationToken);

        if (isNameExist)
        {
            throw new InvalidDataAppException(PortalMessages.GroupWithTheSameNameExists);
        }
    }

    private async Task PublishEvents(UpdateGroupManagerResolver.Response updateResponse, CancellationToken cancellationToken)
    {
        var isGroupManagerChanged = updateResponse.OldGroupManagerId != updateResponse.NewGroupManagerId;

        if (isGroupManagerChanged)
        {
            var events = new List<UserUpdatedEvent>();

            if (updateResponse.OldGroupManagerId.HasValue)
            {
                var oldGmUpdateEvent = await GetActiveUserAsEventById(updateResponse.OldGroupManagerId!.Value,
                    resourceGroupId: null, cancellationToken);

                events.Add(oldGmUpdateEvent);
            }

            if (updateResponse.NewGroupManagerId.HasValue)
            {
                var newGmUpdateEvent = await GetActiveUserAsEventById(updateResponse.NewGroupManagerId!.Value,
                    updateResponse.UpdatedGroupId, cancellationToken);

                events.Add(newGmUpdateEvent);
            }

            await _communicationBus.PublishAsync(events.ToArray(), cancellationToken);
        }

        var groupUpdatedEvent = await _identityDbContext.ResourceGroups
            .Where(x => x.Id == updateResponse.UpdatedGroupId)
            .Select(group => new ResourceGroupUpdatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                ResourceGroupId = group.Id,
                ResourceGroupName = group.Name,
                IsActive = group.IsActive,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (groupUpdatedEvent != null)
        {
            await _communicationBus.PublishAsync(groupUpdatedEvent, cancellationToken);
        }
    }

    private async Task<UserUpdatedEvent> GetActiveUserAsEventById(
        int userId,
        int? resourceGroupId,
        CancellationToken cancellationToken)
    {
        return await _identityDbContext.Users
            .Where(user => user.IsActive && user.Id == userId)
            .Include(user => user.UserRoles)
            .ThenInclude(roles => roles.Role)
            .Select(user => new UserUpdatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LastNameRu = user.LastNameRu,
                FirstNameRu = user.FirstNameRu,
                PatronymicRu = user.PatronymicRu,
                Email = user.Email!,
                Color = user.Color,
                DepartmentId = user.DepartmentId,
                Location = user.Location,
                AvatarFileId = user.AvatarFileId,
                CountryId = user.CountryId,
                Position = user.UserRoles.Select(x => x.Role!.Name!).First(),
                PositionOrderWeight = user.UserRoles.First().Role!.OrderWeight,
                AvatarColor = user.AvatarColor,
                CurrentLocation = user.CurrentLocation,
                HireDate = user.HireDate,
                RegisteredLocationId = user.RegisteredLocationId,
                ResourceGroupId = resourceGroupId,
                InitiatorId = _authenticatedUserContext.UserId,
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);
    }

    internal sealed record Command(int Id, string Name, int? GroupManagerId, string DepartmentId)
        : ICommand<UpdateGroupDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .ShortTextType();
        }
    }
}
