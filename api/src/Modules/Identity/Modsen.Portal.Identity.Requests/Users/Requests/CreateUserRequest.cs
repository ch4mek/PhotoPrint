using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
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
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class CreateUserRequest : ICommandHandler<CreateUserRequest.Command, UserDto>
{
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly ICommunicationBus _communicationBus;
    private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;
    private readonly ITeamBoardContract _teamBoardContract;

    private readonly ICanCurrentUserCreateUserWithRoleBusinessRule _canUserWithRoleBeCreatedBusinessRule;
    private readonly ICanRoleBeOccupiedBusinessRule _canRoleBeOccupiedBusinessRule;
    private readonly ICanRoleHaveDepartmentBusinessRule _canRoleHaveDepartmentBusinessRule;
    private readonly ICanUserWithResourceGroupBeCreatedBusinessRule _canUserWithResourceGroupBeCreatedBusinessRule;

    private readonly IIdentityDbContext _identityDbContext;
    private readonly IRolesCache _rolesCache;
    private readonly IUserLoader _userLoader;

    private readonly IUserDepartmentHistoryComposer _userDepartmentHistoryComposer;

    public CreateUserRequest(
        IHexColorGenerator hexColorGenerator,
        ICommunicationBus communicationBus,
        ICanCurrentUserCreateUserWithRoleBusinessRule canUserWithRoleBeCreatedBusinessRule,
        ICanRoleBeOccupiedBusinessRule canRoleBeOccupiedBusinessRule,
        ICanRoleHaveDepartmentBusinessRule canRoleHaveDepartmentBusinessRule,
        ICanUserWithResourceGroupBeCreatedBusinessRule canUserWithResourceGroupBeCreatedBusinessRule,
        IIdentityPortalSettingsAccessor identityPortalSettingsAccessor,
        ITeamBoardContract teamBoardContract,
        IIdentityDbContext identityDbContext,
        IRolesCache rolesCache,
        IUserLoader userLoader,
        IUserDepartmentHistoryComposer userDepartmentHistoryComposer)
    {
        _hexColorGenerator = hexColorGenerator;
        _communicationBus = communicationBus;
        _canUserWithRoleBeCreatedBusinessRule = canUserWithRoleBeCreatedBusinessRule;
        _canRoleBeOccupiedBusinessRule = canRoleBeOccupiedBusinessRule;
        _canRoleHaveDepartmentBusinessRule = canRoleHaveDepartmentBusinessRule;
        _canUserWithResourceGroupBeCreatedBusinessRule = canUserWithResourceGroupBeCreatedBusinessRule;
        _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
        _teamBoardContract = teamBoardContract;
        _identityDbContext = identityDbContext;
        _rolesCache = rolesCache;
        _userLoader = userLoader;
        _userDepartmentHistoryComposer = userDepartmentHistoryComposer;
    }

    public async Task<UserDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await ValidateDepartmentId(request.Position, request.DepartmentId, cancellationToken);
        await ValidateResourceGroupId(request.DepartmentId, request.Position, request.ResourceGroupId, cancellationToken);
        await ValidateRequestPosition(request, cancellationToken);
        await ValidateRequestEmail(request, cancellationToken);
        await CheckIfRoleIsOccupied(request, cancellationToken);
        await ValidateCountryId(request.CountryId, cancellationToken);
        await ValidateCountryId(request.RegisteredLocationId, cancellationToken);
        await CheckIfPersonalPhoneAndEmailsOccupiedAsync(request, cancellationToken);

        var createdUser = await CreateUser(request, cancellationToken);

        await PublishNotification(createdUser, request, cancellationToken);

        return createdUser;
    }

    private async Task ValidateCountryId(int? countryId, CancellationToken cancellationToken)
    {
        var countries = await _teamBoardContract.GetCountries(cancellationToken);
        if (countries.All(x => x.Id != countryId))
        {
            throw new InvalidDataAppException($"Country with id: {countryId} is not found");
        }
    }

    private async Task ValidateDepartmentId(
        string requestPosition,
        string? departmentId,
        CancellationToken cancellationToken)
    {
        if (departmentId != null &&
            !await _identityDbContext.Departments.AnyAsync(x => x.Id == departmentId, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.DepartmentNotFound);
        }

        var canUserWithDepartmentBeCreatedBusinessRuleModel = new CanRoleHaveDepartmentBusinessRule.Request(requestPosition, departmentId);
        var result = await _canRoleHaveDepartmentBusinessRule.IsCorrespondsToBusiness(canUserWithDepartmentBeCreatedBusinessRuleModel, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new InvalidDataAppException(result.Reason!);
        }
    }

    private async Task ValidateRequestPosition(Command request, CancellationToken cancellationToken)
    {
        var canUserWithRoleBeCreatedBusinessRuleModel = new CanCurrentUserCreateUserWithRoleBusinessRule.Request(request.Position, request.DepartmentId, request.ResourceGroupId);

        var result = await _canUserWithRoleBeCreatedBusinessRule.IsCorrespondsToBusiness(canUserWithRoleBeCreatedBusinessRuleModel, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new ActionNotAllowedException(result.Reason!);
        }
    }

    private async Task ValidateRequestEmail(Command request, CancellationToken cancellationToken)
    {
        var email = request.Position.IsIntern()
            ? request.PersonalEmail
            : request.Email;

        var emailToUpper = email.UpperInvariantNormalize();

        var isUserExistByEmail = await _identityDbContext.Users
            .AnyAsync(u => emailToUpper == u.NormalizedEmail, cancellationToken);

        if (isUserExistByEmail)
        {
            throw new EntityConflictException(string.Format(
                CultureInfo.InvariantCulture,
                PortalMessages.UserWithEmailAlreadyExists,
                request.Email
            ));
        }
    }

    private async Task ValidateResourceGroupId(string? departmentId, string position, int? resourceGroupId, CancellationToken cancellationToken)
    {
        var isEmployeeWithoutGroup = position.IsEmployee() && resourceGroupId == null;
        var isInternWithoutGroup = position.IsIntern() && resourceGroupId == null;
        var isGroupManagerWithoutGroup = position.IsGroupManager() && resourceGroupId == null;
        var isOtherRole = !position.IsEmployee() && !position.IsGroupManager();

        var isChiefOrHeadWithResourceGroup = (position.IsHeadOfDepartment() ||
                position.IsCeo() ||
                position.IsCto()) &&
            resourceGroupId != null;

        if (isChiefOrHeadWithResourceGroup)
        {
            throw new ActionNotAllowedException(PortalMessages.RoleCannotHaveGroup);
        }

        if (isEmployeeWithoutGroup || isGroupManagerWithoutGroup || isOtherRole || isInternWithoutGroup)
        {
            return;
        }

        var canUserWithResourceGroupBeCreatedModel = new CanUserWithResourceGroupBeCreatedBusinessRule.Request(
            departmentId, resourceGroupId!.Value, position);

        var result = await _canUserWithResourceGroupBeCreatedBusinessRule
            .IsCorrespondsToBusiness(canUserWithResourceGroupBeCreatedModel, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new ActionNotAllowedException(result.Reason!);
        }
    }

    private async Task CheckIfRoleIsOccupied(Command request, CancellationToken cancellationToken)
    {
        var canRoleBeOccupiedBusinessRuleModel = new CanRoleBeOccupiedBusinessRule.Request(request.Position, request.DepartmentId, request.ResourceGroupId);

        var result = await _canRoleBeOccupiedBusinessRule.IsCorrespondsToBusiness(canRoleBeOccupiedBusinessRuleModel, cancellationToken);

        if (!result.IsSuccess)
        {
            throw new ActionNotAllowedException(result.Reason!);
        }
    }

    private async Task CheckIfPersonalPhoneAndEmailsOccupiedAsync(Command request, CancellationToken cancellationToken)
    {
        var areUnique = await _teamBoardContract.ArePersonalPhoneAndEmailsUniqueAsync(request.PersonalEmail,
            request.PersonalEmailSecond, request.PersonalPhone, cancellationToken);

        if (!areUnique.IsSuccess)
        {
            throw new EntityConflictException(areUnique.Reason ?? string.Empty);
        }
    }

    private async Task<UserDto> CreateUser(Command request, CancellationToken cancellationToken)
    {
        var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);

        var email = request.Position.IsIntern() ? request.PersonalEmail : request.Email;

        var user = new User
        {
            Email = email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FirstNameRu = request.FirstNameRu,
            LastNameRu = request.LastNameRu,
            PatronymicRu = request.PatronymicRu,
            Color = request.Color.IsNullOrEmpty()
                ? _hexColorGenerator.GenerateColor()
                : request.Color,
            DepartmentId = request.DepartmentId,
            CountryId = request.CountryId ?? PortalSharedConstants.DefaultCountryId,
            TimeZone = settings.DefaultTimeZone,
            Location = request.Location,
            CurrentLocation = request.CurrentLocation,
            RegisteredLocationId = request.RegisteredLocationId,
            ResourceGroupId = request.ResourceGroupId,
            NormalizedUserName = email.UpperInvariantNormalize(),
            NormalizedEmail = email.UpperInvariantNormalize(),
            EmailConfirmed = true,
            HireDate = request.HireDate,
        };

        var roles = await _rolesCache.GetAllRoles(cancellationToken);

        var roleRecord = roles.FirstOrDefault(x => x.Name == request.Position)
            ?? throw new AppException($"Role with name {request.Position} does not exist");

        user.UserRoles.Add(new UserRole
        {
            RoleId = roleRecord.Id
        });

        user.SecurityStamp = Guid.CreateVersion7().ToString(PortalSharedConstants.DefaultGuidFormat);
        user.IsActive = true;

        var createdUser = _identityDbContext.Users.Add(user);
        await _identityDbContext.SaveChanges(cancellationToken);

        _userDepartmentHistoryComposer.Compose(request.DepartmentId, user);
        await _identityDbContext.SaveChanges(cancellationToken);

        return await _userLoader.LoadAsync(createdUser.Entity.Id, cancellationToken);
    }

    private Task PublishNotification(UserDto createdUser, Command request, CancellationToken cancellationToken)
    {
        var notification = new UserCreatedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            UserId = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName ?? string.Empty,
            FirstNameRu = createdUser.FirstNameRu,
            LastNameRu = createdUser.LastNameRu,
            PatronymicRu = createdUser.PatronymicRu,
            DepartmentId = createdUser.DepartmentId,
            Email = createdUser.Email,

            PersonalEmail = request.PersonalEmail,
            PersonalEmailSecond = request.PersonalEmailSecond,
            PersonalPhone = request.PersonalPhone,
            Telegram = request.Telegram,
            LinkedIn = request.LinkedIn,
            DateOfBirth = request.DateOfBirth,

            Color = createdUser.Color,
            CountryId = createdUser.CountryId,
            AvatarFileId = createdUser.AvatarFileId,
            Position = createdUser.Position,
            PositionOrderWeight = createdUser.PositionOrderWeight!.Value,
            Location = createdUser.Location,
            ResourceGroupId = createdUser.ResourceGroupId,
            RegisteredLocationId = createdUser.RegisteredLocationId,
            HireDate = createdUser.HireDate,
            CurrentLocation = createdUser.CurrentLocation,
            MilitaryServiceStatus = request.MilitaryServiceStatus,
            IsGraduateJobPlacement = request.IsGraduateJobPlacement,
            ContractDateFrom = request.ContractDateFrom,
            ContractDateTo = request.ContractDateTo,

            CreatedById = createdUser.CreatedById
        };

        return _communicationBus.PublishAsync(notification, cancellationToken);
    }

    internal sealed record Command(
        string FirstName,
        string LastName,
        string? FirstNameRu,
        string? LastNameRu,
        string? PatronymicRu,
        string? Color,
        int? CountryId,
        string? CurrentLocation,
        int RegisteredLocationId,
        string? Email,
        string? PersonalEmail,
        string? PersonalEmailSecond,
        string? PersonalPhone,
        string? Telegram,
        string? LinkedIn,
        string Position,
        LocationType Location,
        string? DepartmentId,
        int? ResourceGroupId,
        MilitaryServiceStatus? MilitaryServiceStatus,
        bool? IsGraduateJobPlacement,
        DateOnly? ContractDateFrom,
        DateOnly? ContractDateTo,
        DateOnly HireDate,
        DateOnly DateOfBirth) : ICommand<UserDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            When(x => x.Position.IsIntern(), () =>
            {
                RuleFor(x => x.Email)
                    .Empty()
                    .WithMessage("Corporate email address should be empty");

                RuleFor(x => x.PersonalEmail)
                    .NotEmpty()
                    .WithMessage("Personal email address is required");

                RuleFor(x => x)
                    .Must(x => (x.ContractDateFrom is null && x.ContractDateTo is null) ||
                               (x.ContractDateFrom is not null && x.ContractDateTo is not null))
                    .WithMessage("Both ContractDateFrom and ContractDateTo must be either null or have values.");
            });

            When(x => !x.Position.IsIntern(), () =>
            {
                RuleFor(x => x.Email)
                    .NotNull()
                    .WithMessage("Corporate email address is required");

                RuleFor(x => x.ContractDateFrom)
                    .NotNull()
                    .WithMessage("Contract date from is required");

                RuleFor(x => x.ContractDateTo)
                    .NotNull()
                    .WithMessage("Contract date to is required");
            });

            RuleFor(x => x.FirstName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.LastName)
                .AlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.FirstNameRu)
                .NotEmpty()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.LastNameRu)
                .NotEmpty()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            RuleFor(x => x.PatronymicRu)
                .NotEmpty()
                .CyrilicAlphabeticWithHyphensSpaces()
                .SpacesOrPunctuationSymbolsRule();

            When(x => !x.Email.IsNullOrEmpty(), () =>
                RuleFor(x => x.Email)
                    .EmailRuleWithDomain()
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email!.Substring(0, x.Email.IndexOf('@')))
                            .EmailSubstringBeforeAtRule();
                    }));

            When(x => !x.PersonalEmail.IsNullOrEmpty(), () =>
                RuleFor(x => x.PersonalEmail)
                    .EmailRuleWithCustomLength(PortalSharedConstants.EmailType2MinLength,
                        PortalSharedConstants.EmailMaxLength)
                    .WithMessage("Invalid email address format"));

            When(x => !x.PersonalEmailSecond.IsNullOrEmpty(), () =>
                RuleFor(x => x.PersonalEmailSecond)
                    .EmailRuleWithCustomLength(PortalSharedConstants.EmailType2MinLength,
                        PortalSharedConstants.EmailMaxLength)
                    .WithMessage("Invalid email address format"));

            When(x => !x.Telegram.IsNullOrEmpty(), () =>
                RuleFor(x => x.Telegram)
                    .ShortTextType()
                    .SpacesOrPunctuationSymbolsRule()
                    .WithMessage("Invalid telegram format"));

            When(x => !x.PersonalPhone.IsNullOrEmpty(), () =>
            {
                RuleFor(x => x.PersonalPhone)
                    .NotEmpty()
                    .PhoneNumberType()
                    .WithMessage("Invalid phone number format");
            });

            RuleFor(x => x.Color)
                .HexColorFormatRule();

            RuleFor(x => x.Location).IsInEnum()
                .WithMessage("Enter correct location");

            RuleFor(x => x.CountryId).NotNull();

            When(x => x.CurrentLocation is not null, () =>
                RuleFor(x => x.CurrentLocation)
                    .AlphabeticWithHyphensSpaces()
                    .SpacesOrPunctuationSymbolsRule());

            When(x => x.ContractDateTo is not null && x.ContractDateFrom is not null, () =>
                RuleFor(x => x.ContractDateTo)
                    .GreaterThan(x => x.ContractDateFrom));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.Now));

            RuleFor(x => x.LinkedIn)
                .Link()
                .LinkRule()
                .When(x => x.LinkedIn is not null);
        }
    }
}
