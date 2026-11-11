using System.Runtime.CompilerServices;
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
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.Validators;

[assembly: InternalsVisibleTo("Modsen.Portal.ArchitectureTests")]
namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserGeneralInfoRequest : ICommandHandler<UpdateUserGeneralInfoRequest.InternalCommand, UserDto>
{
    private readonly ICommunicationBus _communicationBus;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly ICanUserBeUpdatedBusinessRule _canUserBeUpdatedBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLoader _userLoader;
    private readonly IUpdateUserValidator _updateUserValidator;
    private readonly IRolesCache _rolesCache;
    private readonly IUserDepartmentHistoryComposer _userDepartmentHistoryComposer;

    public UpdateUserGeneralInfoRequest(
        ICommunicationBus communicationBus,
        IAuthenticationTokenContext authenticationTokenContext,
        ICanUserBeUpdatedBusinessRule canUserBeUpdatedBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader,
        IUpdateUserValidator updateUserValidator,
        IRolesCache rolesCache,
        IUserDepartmentHistoryComposer userDepartmentHistoryComposer)
    {
        _communicationBus = communicationBus;
        _authenticationTokenContext = authenticationTokenContext;
        _canUserBeUpdatedBusinessRule = canUserBeUpdatedBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
        _updateUserValidator = updateUserValidator;
        _rolesCache = rolesCache;
        _userDepartmentHistoryComposer = userDepartmentHistoryComposer;
    }

    public async Task<UserDto> Handle(InternalCommand request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id = {request.Id} does not exist");

        await ValidateRequest(user, request, cancellationToken);

        var updatedUser = await UpdateUserAsync(request, cancellationToken);

        return updatedUser;
    }

    private async Task ValidateRequest(UserDto user, InternalCommand request, CancellationToken cancellationToken)
    {
        await _updateUserValidator.Validate(user, request, cancellationToken);

        var businessRuleRequest = new CanUserBeUpdatedBusinessRule.Request(
            user,
            request.DepartmentId,
            request.ResourceGroupId,
            request.Email);

        var result = await _canUserBeUpdatedBusinessRule
                .IsCorrespondsToBusiness(businessRuleRequest, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new ActionNotAllowedException(result.Reason!);
        }
    }

    private async Task<UserDto> UpdateUserAsync(InternalCommand request, CancellationToken cancellationToken)
    {
        var userEntity = await _identityDbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Include(x => x.UserDepartmentHistory)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id={request.Id} does not exists");

        userEntity.FirstName = request.FirstName ?? userEntity.FirstName;
        userEntity.LastName = request.LastName ?? userEntity.LastName;
        userEntity.EmailConfirmed = true;
        userEntity.ResourceGroupId = request.ResourceGroupId;

        var email = request.Email ?? userEntity.Email;
        userEntity.Email = email;
        userEntity.NormalizedUserName = email.UpperInvariantNormalize();
        userEntity.NormalizedEmail = email.UpperInvariantNormalize();

        await UpdateUserRoleAsync(request, userEntity, cancellationToken);
        await UpdateUserDepartmentAsync(request, userEntity, cancellationToken);
        UpdateUserRuNames(request, userEntity);

        await _identityDbContext.SaveChanges(cancellationToken);
        var updatedUser = await _userLoader.LoadAsync(request.Id, cancellationToken);

        var notification = new UserUpdatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = updatedUser.Id,
            FirstName = updatedUser.FirstName,
            LastName = updatedUser.LastName ?? string.Empty,
            FirstNameRu = updatedUser.FirstNameRu,
            LastNameRu = updatedUser.LastNameRu,
            PatronymicRu = updatedUser.PatronymicRu,
            DepartmentId = updatedUser.DepartmentId,
            Email = updatedUser.Email,
            CountryId = updatedUser.CountryId,
            Color = updatedUser.Color,
            AvatarFileId = updatedUser.AvatarFileId,
            Position = updatedUser.Position,
            PositionOrderWeight = updatedUser.PositionOrderWeight ?? 0,
            Location = updatedUser.Location,
            ResourceGroupId = updatedUser.ResourceGroupId,
            RegisteredLocationId = updatedUser.RegisteredLocationId,
            HireDate = updatedUser.HireDate,
            CurrentLocation = updatedUser.CurrentLocation,
            AvatarColor = updatedUser.AvatarColor,
            InitiatorId = _authenticatedUserContext.UserId,
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        var shouldCancelUserSession = updatedUser.DepartmentId != request.DepartmentId ||
                                      updatedUser.ResourceGroupId != request.ResourceGroupId;

        if (shouldCancelUserSession)
        {
            await _authenticationTokenContext.RevokeAllUserTokens(userEntity.Id, RevokeReasons.InfoUpdated, cancellationToken);
        }

        return updatedUser;
    }

    private async Task UpdateUserRoleAsync(InternalCommand request, User userEntity, CancellationToken cancellationToken)
    {
        var allRoleModels = await _rolesCache.GetAllRoles(cancellationToken);
        var role = allRoleModels.FirstOrDefault(role => role.Name == request.Position)
            ?? throw new AppException($"Role with name {request.Position} does not exist");

        if (userEntity.UserRoles.First().Role!.Name != request.Position)
        {
            userEntity.UserRoles.Clear();
            userEntity.UserRoles.Add(new UserRole
            {
                UserId = userEntity.Id,
                RoleId = role.Id
            });
        }
    }

    private async Task UpdateUserDepartmentAsync(InternalCommand request, User user, CancellationToken cancellationToken)
    {
        _userDepartmentHistoryComposer.Compose(request.DepartmentId, user);
        user.DepartmentId = request.DepartmentId;
        var roleIds = user.UserRoles.Select(x => x.RoleId);
        var roles = await _rolesCache.GetAllRoles(cancellationToken);
        var rolesStr = roles.Where(role => roleIds.Contains(role.Id)).Select(role => role.Name)
            .Where(r => r is not null).ToList();
        if (rolesStr.IsCeo() || rolesStr.IsCto())
        {
            user.DepartmentId = null;
        }
    }

    private void UpdateUserRuNames(InternalCommand request, User user)
    {
        if (user.FirstNameRu != request.FirstNameRu ||
            user.LastNameRu != request.LastNameRu ||
            user.PatronymicRu != request.PatronymicRu)
        {
            if (_authenticatedUserContext.UserId == request.Id)
            {
                throw new ActionNotAllowedException("You cannot edit your FirstNameRu (LastNameRu, PatronymicRu)");
            }

            user.FirstNameRu = request.FirstNameRu;
            user.LastNameRu = request.LastNameRu;
            user.PatronymicRu = request.PatronymicRu;
        }
    }

    internal sealed record InternalCommand(
        int Id,
        string? FirstName,
        string? LastName,
        string? FirstNameRu,
        string? LastNameRu,
        string? PatronymicRu,
        string? Email,
        string? DepartmentId,
        int? ResourceGroupId,
        string Position)
    : ICommand<UserDto>, IIdentityRequest;


    internal sealed class Validator : AbstractValidator<InternalCommand>
    {
        public Validator()
        {
            When(x => x.FirstNameRu is not null, () =>
                RuleFor(x => x.FirstNameRu)
                    .CyrilicAlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.LastNameRu is not null, () =>
                RuleFor(x => x.LastNameRu)
                    .CyrilicAlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.PatronymicRu is not null, () =>
                RuleFor(x => x.PatronymicRu)
                    .CyrilicAlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.FirstName is not null, () => RuleFor(x => x.FirstName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule());

            When(x => x.LastName is not null, () => RuleFor(x => x.LastName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule());

            When(x => x.Email is not null, () =>
                RuleFor(x => x.Email)
                    .EmailRuleWithDomain()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email!.Substring(0, x.Email.IndexOf('@')))
                            .EmailSubstringBeforeAtRule();
                    }));

            RuleFor(x => x.Position)
                .Must(position => position.IsEmployee() || position.IsHeadOfDepartment()
                    || position.IsGroupManager() || position.IsSystemAdministrator()
                    || position.IsIntern())
                .WithMessage("Invalid position value");
        }
    }
}
