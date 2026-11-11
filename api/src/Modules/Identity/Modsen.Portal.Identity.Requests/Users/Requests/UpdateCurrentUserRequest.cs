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
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Storage.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateCurrentUserRequest : ICommandHandler<UpdateCurrentUserRequest.Command, UserDto>{

    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly ICommunicationBus _communicationBus;
    private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;

    private readonly ICanRoleBeOccupiedBusinessRule _canRoleBeOccupiedBusinessRule;
    private readonly ICanRoleHaveDepartmentBusinessRule _canRoleHaveDepartmentBusinessRule;

    private readonly IStorageContract _storageContract;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLoader _userLoader;
    private readonly IRolesCache _rolesCache;

    private readonly IUserDepartmentHistoryComposer _userDepartmentHistoryComposer;

    public UpdateCurrentUserRequest(
        IAuthenticatedUserContext authenticatedUser,
        ICommunicationBus communicationBus,
        IStorageContract storageContract,
        IAuthenticationTokenContext authenticationTokenContext,
        ICanRoleBeOccupiedBusinessRule canRoleBeOccupiedBusinessRule,
        ICanRoleHaveDepartmentBusinessRule canRoleHaveDepartmentBusinessRule,
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader,
        IRolesCache rolesCache,
        IIdentityPortalSettingsAccessor identityPortalSettingsAccessor,
        IUserDepartmentHistoryComposer userDepartmentHistoryComposer)
    {
        _authenticatedUser = authenticatedUser;
        _communicationBus = communicationBus;
        _storageContract = storageContract;
        _authenticationTokenContext = authenticationTokenContext;
        _canRoleBeOccupiedBusinessRule = canRoleBeOccupiedBusinessRule;
        _canRoleHaveDepartmentBusinessRule = canRoleHaveDepartmentBusinessRule;
        _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
        _rolesCache = rolesCache;
        _userDepartmentHistoryComposer = userDepartmentHistoryComposer;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _identityDbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Include(x => x.UserDepartmentHistory)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == _authenticatedUser.GetRequiredUserId(), cancellationToken)
            ?? throw new EntityNotFoundException($"User with id {_authenticatedUser.GetRequiredUserId()} does not exist");

        var departmentId = user.DepartmentId;
        var userPosition = user.UserRoles
            .Select(x => x.Role?.Name)
            .FirstOrDefault(x => !x.IsNullOrWhiteSpace());

        await ValidateRequest(request, departmentId, userPosition, cancellationToken);

        if (request.AvatarFileId is null && user.AvatarFileId is not null)
        {
            var deleteFileNotification = new DeleteFileEvent(user.AvatarFileId, Guid.CreateVersion7().ToString());

            await _communicationBus.PublishAsync(deleteFileNotification, cancellationToken);
        }

        user.AvatarColor = request.AvatarColor;
        user.AvatarFileId = request.AvatarFileId;
        user.DepartmentId = request.DepartmentId;
        user.Location = request.Location;
        user.TimeZone = request.TimeZone;

        if (request.AvatarColor.IsNullOrEmpty() && user.AvatarColor.IsNullOrEmpty())
        {
            var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);
            user.AvatarColor = settings.DefaultColorHex;
        }

        if (user.UserRoles.First().Role!.Name != userPosition)
        {
            var roles = await _rolesCache.GetAllRoles(cancellationToken);
            var role = roles.FirstOrDefault(role => role.Name == userPosition)
                ?? throw new AppException($"Role with name {userPosition} does not exist");

            user.UserRoles.Clear();
            user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        }

        if (departmentId != user.DepartmentId)
        {
            await _authenticationTokenContext.RevokeAllUserTokens(user.Id, reason: RevokeReasons.DepartmentChanged, cancellationToken);
        }

        _userDepartmentHistoryComposer.Compose(request.DepartmentId, user);
        await _identityDbContext.SaveChanges(cancellationToken);

        var updatedUser = await _userLoader.LoadAsync(user.Id, cancellationToken);

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
            Color = updatedUser.Color,
            AvatarColor = updatedUser.AvatarColor,
            AvatarFileId = updatedUser.AvatarFileId,
            Position = userPosition,
            PositionOrderWeight = updatedUser.PositionOrderWeight!.Value,
            ResourceGroupId = updatedUser.ResourceGroupId,
            Location = updatedUser.Location,
            RegisteredLocationId = updatedUser.RegisteredLocationId,
            CountryId = updatedUser.CountryId,
            CurrentLocation = updatedUser.CurrentLocation,
            HireDate = updatedUser.HireDate,
            InitiatorId = updatedUser.Id,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return updatedUser;
    }

    private async Task ValidateDepartmentId(string? position, string? departmentId, CancellationToken cancellationToken)
    {
        var model = new CanRoleHaveDepartmentBusinessRule.Request(position, departmentId);
        var result = await _canRoleHaveDepartmentBusinessRule.IsCorrespondsToBusiness(model, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new InvalidDataAppException(result.Reason!);
        }
    }

    private async Task ValidateAvatarFileId(Command request, CancellationToken cancellationToken)
    {
        if (request.AvatarFileId != null &&
            !await _storageContract.IsFileExists(request.AvatarFileId, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.FileIsNotFound);
        }
    }

    private async Task ValidateRequest(Command request, string? departmentId, string? userPosition, CancellationToken cancellationToken)
    {
        await ValidateDepartmentId(userPosition, request.DepartmentId, cancellationToken);

        if (userPosition.IsHeadOfDepartment()
            && departmentId == null
            && request.DepartmentId != null)
        {
            var canRoleBeOccupiedBusinessRuleModel = new CanRoleBeOccupiedBusinessRule.Request(userPosition, request.DepartmentId, null);

            var canRoleBeOccupied = await _canRoleBeOccupiedBusinessRule.IsCorrespondsToBusiness(canRoleBeOccupiedBusinessRuleModel, cancellationToken);

            if (!canRoleBeOccupied.IsSuccess)
            {
                throw new ActionNotAllowedException(canRoleBeOccupied.Reason!);
            }
        }

        if (departmentId != null
            && departmentId != request.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotChangeYourDepartment);
        }

        await ValidateAvatarFileId(request, cancellationToken);
    }

    internal sealed record Command(
        string TimeZone,
        string? AvatarColor,
        string? AvatarFileId,
        string? DepartmentId,
        LocationType Location) : ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.TimeZone)
                .TimeZoneRule();

            RuleFor(x => x.AvatarColor)
                .HexColorFormatRule();

            RuleFor(x => x.Location).IsInEnum()
                .WithMessage("Enter correct location");
        }
    }
}
