using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserHireDateRequest : ICommandHandler<UpdateUserHireDateRequest.Command, DateOnly>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ICommunicationBus _communicationBus;
    private readonly IUserLoader _userLoader;

    public UpdateUserHireDateRequest(
        IIdentityDbContext identityDbContext,
        ICommunicationBus communicationBus,
        IAuthenticatedUserContext authenticatedUserContext,
        IUserLoader userLoader)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _communicationBus = communicationBus;
        _userLoader = userLoader;
    }

    public async Task<DateOnly> Handle(Command request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRequiredUserId() == request.Id)
        {
            throw new ActionNotAllowedException("You are not allowed to edit your own hire date");
        }

        var user = await _identityDbContext.Users
            .Include(x=> x.UserRoles)
            .ThenInclude(x=> x.Role)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id = {request.Id} does not exist");

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
                user.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another department");
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
                user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException("You are not allowed to update user from another resource group");
            }
        }

        user.HireDate = request.HireDate;
        await _identityDbContext.SaveChanges(cancellationToken);

        var updatedUser = await _userLoader.LoadAsync(request.Id, cancellationToken);
        var notification = new UserUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = updatedUser.Id,
            LastName = updatedUser.LastName ?? string.Empty,
            FirstName = updatedUser.FirstName,
            LastNameRu = updatedUser.LastNameRu,
            FirstNameRu = updatedUser.FirstNameRu,
            PatronymicRu = updatedUser.PatronymicRu,
            DepartmentId = updatedUser.DepartmentId,
            Email = updatedUser.Email,
            CountryId = updatedUser.CountryId,
            Color = updatedUser.Color,
            AvatarFileId = updatedUser.AvatarFileId,
            Position = updatedUser.Position,
            PositionOrderWeight = user.UserRoles.First().Role!.OrderWeight,
            Location = updatedUser.Location,
            ResourceGroupId = updatedUser.ResourceGroupId,
            RegisteredLocationId = updatedUser.RegisteredLocationId,
            HireDate = updatedUser.HireDate,
            AvatarColor = updatedUser.AvatarColor,
            CurrentLocation = updatedUser.CurrentLocation,
            InitiatorId = _authenticatedUserContext.UserId,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);


        return updatedUser.HireDate!.Value;
    }

    internal sealed record Command(
        int Id,
        DateOnly HireDate) : ICommand<DateOnly>, ITransactionalRequest, IIdentityRequest;
}
