using System.Globalization;
using System.Security.Claims;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.IdentityPortalSettings;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class ExternalLoginCallbackRequest : ICommandHandler<ExternalLoginCallbackRequest.Command, ExternalLoginDto>
{
    private readonly ITokenService _tokenService;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly IIsApplicationRolesCorrectBusinessRule _isApplicationRolesCorrectBusinessRule;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IIdentityPortalSettingsAccessor _identityPortalSettingsAccessor;

    private readonly string[] _nonAuthorizedEmails =
    [
        "modsen.portal.test.ceo@modsen-software.com",
        "modsen.portal.test.cto@modsen-software.com",
        "kazteleport.demo.ceo@modsen-software.com",
        "kazteleport.demo.cto@modsen-software.com",
        "modsen.portal.test.sysadmin@modsen-software.com",
        "modsen.portal.test.employee@modsen-software.com",
        "modsen.portal.test.headofdepartment@modsen-software.com",
        "modsen.portal.test.intern@modsen.by"
    ];

    public ExternalLoginCallbackRequest(
        ITokenService tokenService,
        IHostEnvironmentContext hostEnvironmentContext,
        IIsApplicationRolesCorrectBusinessRule isApplicationRolesCorrectBusinessRule,
        IIdentityPortalSettingsAccessor identityPortalSettingsAccessor,
        IAuthenticationTokenContext authenticationTokenContext,
        IIdentityDbContext identityDbContext)
    {
        _tokenService = tokenService;
        _hostEnvironmentContext = hostEnvironmentContext;
        _isApplicationRolesCorrectBusinessRule = isApplicationRolesCorrectBusinessRule;
        _authenticationTokenContext = authenticationTokenContext;
        _identityDbContext = identityDbContext;
        _identityPortalSettingsAccessor = identityPortalSettingsAccessor;
    }

    public async Task<ExternalLoginDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!request.AuthenticationResult.Succeeded)
        {
            return ExternalLoginDto.Failed(PortalMessages.ExternalAuthenticationError);
        }

        var settings = await _identityPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentException.ThrowIfNullOrEmpty(settings.EmailDomain);
        ArgumentException.ThrowIfNullOrEmpty(settings.DefaultTimeZone);

        if (_hostEnvironmentContext.IsProductionEnvironment() &&
            _nonAuthorizedEmails.Contains(request.AuthenticationResult.Email))
        {
            return ExternalLoginDto.Failed(PortalMessages.AccountIsNotAuthorized);
        }

        var email = request.AuthenticationResult.Email;
        ArgumentException.ThrowIfNullOrEmpty(email);

        var user = await LoadUserWithClaims(email, cancellationToken);
        if (user == null)
        {
            return ExternalLoginDto.Failed(PortalMessages.UserNotFound);
        }

        if (!user.UserRoles.Any(x => x.Role!.Name == PortalRoleNames.Intern) &&
            !(email?.EndsWith(settings.EmailDomain, StringComparison.InvariantCulture) ?? false))
        {
            return ExternalLoginDto.Failed(PortalMessages.UserIsNotInDomain);
        }

        if (!_hostEnvironmentContext.IsDevelopmentEnvironment() &&
            !_hostEnvironmentContext.IsQaEnvironment() &&
            !await _isApplicationRolesCorrectBusinessRule.IsCorrespondsToBusiness(null, cancellationToken))
        {
            var usersIds = await _identityDbContext.Users
                .Select(f => f.Id)
                .ToListAsync(cancellationToken);

            foreach (var userId in usersIds)
            {
                await _authenticationTokenContext.RevokeAllUserTokens(userId, reason: RevokeReasons.EmergencyShutdown, cancellationToken);
            }

            return ExternalLoginDto.Failed(PortalMessages.InternalAuthenticationError);
        }

        if (!user.IsActive)
        {
            return ExternalLoginDto.Failed(PortalMessages.UserIsInactive);
        }

        var bearerToken = GenerateTokenAsync(user);
        await _authenticationTokenContext.RegisterUserToken(user.Id, bearerToken.Token, bearerToken.ValidTillUtc, cancellationToken);

        return ExternalLoginDto.Success(bearerToken);
    }

    private Task<User?> LoadUserWithClaims(string email, CancellationToken token)
    {
        return _identityDbContext.Users
            .AsSplitQuery()
            .Include(x => x.Department)
            .ThenInclude(x => x!.Claims)
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .ThenInclude(x => x!.Claims)
            .Include(x => x.Claims)
            .FirstOrDefaultAsync(user => user.Email == email || user.NormalizedEmail == email.UpperInvariantNormalize(), token);
    }

    private TokenModel GenerateTokenAsync(User user)
    {
        var claims = user.Claims
            .Select(userClaim => userClaim.ToClaim())
            .Union(user.UserRoles
                .SelectMany(userRole => userRole.Role!.Claims)
                .Select(x => x.ToClaim())
            )
            .Union(user.Department?.Claims
                .Select(x => x.ToClaim()) ?? []
            )
            .ToList();
        var roles = user.UserRoles.Select(x => x.Role!.Name!).ToList();

        if (user.UserRoles.Any(role => role.Role!.IsCompanyManager))
        {
            claims.Add(new Claim(PortalClaimTypeNames.IsManager, "1"));
        }

        var trackBillableWorkingHoursOnProjectClaims = _identityDbContext.PermissionAssignments
            .Where(x => x.UserId == user.Id &&
                x.ResourceType == nameof(PermissionResourceType.Project) &&
                x.Permission!.Name == nameof(PermissionType.TrackBillableWorkingHours))
            .Select(x => new Claim(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, x.ResourceId!))
            .ToList();

        if (trackBillableWorkingHoursOnProjectClaims.Count > 0)
        {
            claims.AddRange(trackBillableWorkingHoursOnProjectClaims);
        }

        var userDto = new UserTokenDto(
            user.Id.ToString(CultureInfo.InvariantCulture),
            user.Email!,
            user.FirstName,
            user.LastName,
            user.ResourceGroupId)
        {
            DepartmentId = user.DepartmentId,
            Roles = roles,
            Claims = claims
        };

        return _tokenService.GenerateToken(userDto);
    }

    internal sealed record Command(AuthenticationResult AuthenticationResult)
        : ICommand<ExternalLoginDto>, ITransactionalRequest, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.AuthenticationResult.Email)
                .NotNull()
                .NotEmpty()
                .Must(email => email?.StartsWith("live#", StringComparison.InvariantCulture) != true)
                .WithMessage("Email should not start with 'live#'");
        }
    }
}
