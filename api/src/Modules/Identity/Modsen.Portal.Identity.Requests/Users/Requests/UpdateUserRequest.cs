using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
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
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class UpdateUserRequest : ICommandHandler<UpdateUserRequest.Command, UserDto>
{
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly ICommunicationBus _communicationBus;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly ICanUserFieldsBeUpdatedBusinessRule _canUserFieldsBeUpdatedBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLoader _userLoader;
    private readonly IRolesCache _rolesCache;
    private readonly IUpdateUserValidator _updateUserValidator;
    private readonly IUserDepartmentHistoryComposer _userDepartmentHistoryComposer;

    public UpdateUserRequest(
        IHexColorGenerator hexColorGenerator,
        ICommunicationBus communicationBus,
        IAuthenticationTokenContext authenticationTokenContext,
        ICanUserFieldsBeUpdatedBusinessRule canUserFieldsBeUpdatedBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardContract teamBoardContract,
        IIdentityDbContext identityDbContext,
        IUserLoader userLoader,
        IRolesCache rolesCache,
        IUpdateUserValidator updateUserValidator,
        IUserDepartmentHistoryComposer userDepartmentHistoryComposer)
    {
        _hexColorGenerator = hexColorGenerator;
        _communicationBus = communicationBus;
        _authenticationTokenContext = authenticationTokenContext;
        _canUserFieldsBeUpdatedBusinessRule = canUserFieldsBeUpdatedBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardContract = teamBoardContract;
        _identityDbContext = identityDbContext;
        _userLoader = userLoader;
        _rolesCache = rolesCache;
        _updateUserValidator = updateUserValidator;
        _userDepartmentHistoryComposer = userDepartmentHistoryComposer;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id = {request.Id} does not exist");

        await ValidateRequest(user, request, cancellationToken);

        var updatedUser = await UpdateUserAsync(request, cancellationToken);

        return updatedUser;
    }

    private async Task ValidateRequest(UserDto user, Command request, CancellationToken cancellationToken)
    {
        await _updateUserValidator.Validate(user, request, cancellationToken);
        await ValidateCountryId(request.CountryId, cancellationToken);
        await ValidateCountryId(request.RegisteredLocationId, cancellationToken);

        var result = await _canUserFieldsBeUpdatedBusinessRule
                .IsCorrespondsToBusiness(new CanUserFieldsBeUpdatedBusinessRule.Request(user, request), cancellationToken);

        if (!result.IsSuccess)
        {
            throw new ActionNotAllowedException(result.Reason!);
        }
    }

    private async Task ValidateCountryId(int? countryId, CancellationToken cancellationToken)
    {
        var countries = await _teamBoardContract.GetCountries(cancellationToken);
        if (countries.All(x => x.Id != countryId))
        {
            throw new InvalidDataAppException($"Country with id: {countryId} is not found");
        }
    }

    private async Task<UserDto> UpdateUserAsync(Command request, CancellationToken cancellationToken)
    {
        var userEntity = await _identityDbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .Include(x => x.UserDepartmentHistory)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException($"User with id={request.Id} does not exists");

        userEntity.ResourceGroupId = request.ResourceGroupId;
        userEntity.FirstName = request.FirstName ?? userEntity.FirstName;
        userEntity.LastName = request.LastName ?? userEntity.LastName;
        userEntity.CountryId = request.CountryId;
        userEntity.RegisteredLocationId = request.RegisteredLocationId ?? userEntity.RegisteredLocationId;
        userEntity.Location = request.Location;
        userEntity.Color = request.Color;
        userEntity.EmailConfirmed = true;
        userEntity.HireDate = request.HireDate ?? userEntity.HireDate;
        userEntity.CurrentLocation = request.CurrentLocation;

        var email = request.Email ?? userEntity.Email;
        userEntity.Email = email;
        userEntity.NormalizedUserName = email.UpperInvariantNormalize();
        userEntity.NormalizedEmail = email.UpperInvariantNormalize();

        UpdateUserDepartment(request, userEntity);
        UpdateUserRuNames(request, userEntity);
        UpdateUserColor(request, userEntity);

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
            PositionOrderWeight = role.OrderWeight,
            Location = updatedUser.Location,
            ResourceGroupId = updatedUser.ResourceGroupId,
            RegisteredLocationId = updatedUser.RegisteredLocationId,
            HireDate = updatedUser.HireDate,
            CurrentLocation = updatedUser.CurrentLocation,
            AvatarColor = updatedUser.AvatarColor,            
            InitiatorId = _authenticatedUserContext.UserId
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        var shouldCancelUserSession = updatedUser.DepartmentId != request.DepartmentId ||
                                      updatedUser.Position != request.Position ||
                                      updatedUser.ResourceGroupId != request.ResourceGroupId;

        if (shouldCancelUserSession)
        {
            await _authenticationTokenContext.RevokeAllUserTokens(userEntity.Id, RevokeReasons.InfoUpdated, cancellationToken);
        }

        return updatedUser;
    }

    private void UpdateUserDepartment(Command request, User user)
    {
        _userDepartmentHistoryComposer.Compose(request.DepartmentId, user);

        user.DepartmentId = request.DepartmentId;

        if (request.Position.IsCeo() || request.Position.IsCto())
        {
            user.DepartmentId = null;
        }
    }

    private void UpdateUserColor(Command request, User user)
    {
        if (request.Color.IsNullOrEmpty())
        {
            user.Color = user.Color.IsNullOrEmpty()
                ? _hexColorGenerator.GenerateColor()
                : user.Color;
        }
    }

    private void UpdateUserRuNames(Command request, User user)
    {
        if (user.FirstNameRu == request.FirstNameRu && user.LastNameRu == request.LastNameRu)
        {
            return;
        }

        if (_authenticatedUserContext.UserId == request.Id)
        {
            throw new ActionNotAllowedException("You cannot edit your FirstNameRu (LastNameRu)");
        }

        user.FirstNameRu = request.FirstNameRu;
        user.LastNameRu = request.LastNameRu;
    }

    internal sealed record Command(
        int Id,
        string? FirstName,
        string? LastName,
        string? FirstNameRu,
        string? LastNameRu,
        string? Color,
        int? CountryId,
        int? RegisteredLocationId,
        string? CurrentLocation,
        string? Email,
        string Position,
        LocationType? Location,
        string? DepartmentId,
        int? ResourceGroupId,
        IReadOnlyCollection<string> CurrentUserRole,
        string? CurrentUserDepartmentId,
        DateOnly? HireDate) : ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
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

            RuleFor(x => x.Color)
                .HexColorFormatRule();

            When(x => x.Location is not null, () =>
                RuleFor(x => x.Location).IsInEnum()
                    .WithMessage("Enter correct location"));

            When(x => x.FirstName is not null, () => RuleFor(x => x.FirstName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule());

            When(x => x.LastName is not null, () => RuleFor(x => x.LastName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule());

            When(x => x.Email is not null && !x.Position.IsIntern(), () =>
                RuleFor(x => x.Email)
                    .EmailRuleWithDomain()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email!.Substring(0, x.Email.IndexOf('@')))
                            .EmailSubstringBeforeAtRule();
                    }));

            When(x => x.Email is not null && x.Position.IsIntern(), () =>
                RuleFor(x => x.Email)
                    .EmailRuleWithCustomLength(PortalSharedConstants.EmailType2MinLength,
                        PortalSharedConstants.EmailMaxLength)
                    .WithMessage("Invalid email address format"));

            RuleFor(x => x.CountryId).NotNull();

            RuleFor(x => x.RegisteredLocationId).NotNull();

            When(x => x.CurrentLocation is not null, () =>
                RuleFor(x => x.CurrentLocation)
                    .AlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());
        }
    }
}
