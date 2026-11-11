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
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.ResourceGroups.Core.BusinessRules;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class CreateResourceGroupRequest : ICommandHandler<CreateResourceGroupRequest.Command, int>
{
    private readonly ICommunicationBus _communicationBus;
    private readonly ICanUserUpdateGroupBusinessRule _canUserUpdateGroupBusinessRule;
    private readonly IIsNewGroupManagerValidBusinessRule _groupManagerValidBusinessRule;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CreateResourceGroupRequest(
        ICommunicationBus communicationBus,
        ICanUserUpdateGroupBusinessRule canUserUpdateGroupBusinessRule,
        IIsNewGroupManagerValidBusinessRule groupManagerValidBusinessRule,
        IIdentityDbContext identityDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _communicationBus = communicationBus;
        _canUserUpdateGroupBusinessRule = canUserUpdateGroupBusinessRule;
        _groupManagerValidBusinessRule = groupManagerValidBusinessRule;
        _identityDbContext = identityDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        var createdGroupId = await CreateResourceGroupAsync(request.GroupManagerId, request.GroupName, request.DepartmentId, cancellationToken);

        await PublishEvents(request, createdGroupId, cancellationToken);

        return createdGroupId;
    }

    private async Task<int> CreateResourceGroupAsync(int? groupManagerId, string groupName, string departmentId, CancellationToken cancellationToken)
    {
        var groupManager = await _identityDbContext.Users
            .Where(user => user.Id == groupManagerId && user.IsActive)
            .Where(user => user.UserRoles.Any(role => role.Role!.Name == PortalRoleNames.GroupManager))
            .Include(user => user.ResourceGroup)
            .FirstOrDefaultAsync(cancellationToken);

        var newGroup = new ResourceGroup
        {
            DepartmentId = departmentId,
            Name = groupName,
            IsActive = true,
        };

        if (groupManager is not null)
        {
            groupManager.ResourceGroup = newGroup;
        }

        var entry = _identityDbContext.ResourceGroups.Add(newGroup);

        await _identityDbContext.SaveChanges(cancellationToken);

        return entry.Entity.Id;
    }

    private async Task ValidateRequest(Command request, CancellationToken cancellationToken)
    {
        var isUserCanCreateGroup = await _canUserUpdateGroupBusinessRule.IsCorrespondsToBusiness(
            new CanUserUpdateGroupBusinessRule.Request(request.DepartmentId), cancellationToken);

        if (!isUserCanCreateGroup.IsSuccess)
        {
            throw new ActionNotAllowedException(isUserCanCreateGroup.Reason!);
        }

        if (request.GroupManagerId != null)
        {
            var isGmValid = await _groupManagerValidBusinessRule.IsCorrespondsToBusiness(
                new IsNewGroupManagerValidBusinessRule.Request(request.GroupManagerId.Value, request.DepartmentId), cancellationToken);

            if (!isGmValid.IsSuccess)
            {
                throw new InvalidDataAppException(isGmValid.Reason!);
            }
        }

        var isNameExist = await _identityDbContext.ResourceGroups
            .AnyAsync(resourceGroup => resourceGroup.Name == request.GroupName, cancellationToken);

        if (isNameExist)
        {
            throw new InvalidDataAppException(PortalMessages.GroupWithTheSameNameExists);
        }
    }

    private async Task PublishEvents(Command request, int createdGroupId,
        CancellationToken cancellationToken)
    {
        var groupCreatedEvent = await _identityDbContext.ResourceGroups
            .Where(x => x.Id == createdGroupId)
            .Select(group => new ResourceGroupCreatedEvent
            {
                EventId = Guid.CreateVersion7().ToString(),
                ResourceGroupId = group.Id,
                ResourceGroupName = group.Name,
                IsActive = group.IsActive,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (groupCreatedEvent != null)
        {
            await _communicationBus.PublishAsync(groupCreatedEvent, cancellationToken);
        }

        if (request.GroupManagerId != null)
        {
            var updateOldGmNotification = await _identityDbContext.Users
                .Include(user => user.UserRoles)
                .ThenInclude(roles => roles.Role)
                .Where(user => user.IsActive && user.Id == request.GroupManagerId.Value)
                .Select(user => new UserUpdatedEvent
                {
                    EventId = Guid.CreateVersion7().ToString(),
                    UserId = user.Id,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    LastNameRu = user.LastNameRu,
                    FirstNameRu = user.FirstNameRu,
                    PatronymicRu = user.PatronymicRu,
                    DepartmentId = user.DepartmentId,
                    Email = user.Email!,
                    CountryId = user.CountryId,
                    Color = user.Color,
                    AvatarFileId = user.AvatarFileId,
                    Position = user.UserRoles.Select(x => x.Role!.Name!).First(),
                    PositionOrderWeight = user.UserRoles.First().Role!.OrderWeight,
                    Location = user.Location,
                    ResourceGroupId = createdGroupId,
                    RegisteredLocationId = user.RegisteredLocationId,
                    AvatarColor = user.Color,
                    HireDate = user.HireDate,
                    CurrentLocation = user.CurrentLocation,
                    InitiatorId = _authenticatedUserContext.UserId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (updateOldGmNotification != null)
            {
                await _communicationBus.PublishAsync(updateOldGmNotification, cancellationToken);
            }
        }
    }

    internal sealed record Command(int? GroupManagerId, string DepartmentId, string GroupName)
        : ICommand<int>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.GroupName)
                .ShortTextType();
        }
    }
}
