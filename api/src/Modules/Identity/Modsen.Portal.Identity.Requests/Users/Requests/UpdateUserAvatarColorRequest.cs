using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserAvatarColorRequest : ICommandHandler<UpdateUserAvatarColorRequest.Command, UserDto>
{
    private readonly ICommunicationBus _communicationBus;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public UpdateUserAvatarColorRequest(
        ICommunicationBus communicationBus,
        IAuthenticatedUserContext authenticatedUserContext,
        IUserLoader userLoader,
        IIdentityDbContext identityDbContext)
    {
        _communicationBus = communicationBus;
        _authenticatedUserContext = authenticatedUserContext;
        _userLoader = userLoader;
        _identityDbContext = identityDbContext;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _identityDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id ${request.UserId} does not exist");

        var userRoles = _authenticatedUserContext.GetRoles();

        if (userRoles.IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentDepartment);
        }

        if (userRoles.IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeUserWithDifferentGroup);
        }

        user.AvatarColor = request.Color;
        await _identityDbContext.SaveChanges(cancellationToken);

        var notification = new UserUpdatedEvent
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
            Color = user.Color,
            AvatarColor = user.AvatarColor,
            AvatarFileId = user.AvatarFileId,
            Position = user.UserRoles
                .Select(x => x.Role?.Name)
                .FirstOrDefault(x => !x.IsNullOrWhiteSpace()),
            PositionOrderWeight = user.UserRoles
                .Select(x => x.Role?.OrderWeight)
                .FirstOrDefault()!.Value,
            ResourceGroupId = user.ResourceGroupId,
            CountryId = user.CountryId,
            CurrentLocation = user.CurrentLocation,
            HireDate = user.HireDate,
            Location = user.Location,
            RegisteredLocationId = user.RegisteredLocationId,
            InitiatorId = _authenticatedUserContext.UserId
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return await _userLoader.LoadAsync(request.UserId, cancellationToken);
    }

    internal sealed record Command(int UserId, string? Color) :
        ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Color)
                .HexColorFormatRule();
        }
    }
}
