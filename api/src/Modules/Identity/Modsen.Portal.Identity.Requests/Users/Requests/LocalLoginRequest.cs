using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
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

namespace Modsen.Portal.Identity.Requests.Users.Requests;

/// <summary>
/// Локальный вход по email+паролю (демо-режим хакатона, замена Azure AD).
/// Пароли выставляются сидами (UsersSeed, демо-пароль demo1234).
/// </summary>
internal sealed class LocalLoginRequest : ICommandHandler<LocalLoginRequest.Command, ExternalLoginDto>
{
    private readonly ITokenService _tokenService;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IIdentityDbContext _identityDbContext;

    public LocalLoginRequest(
        ITokenService tokenService,
        IAuthenticationTokenContext authenticationTokenContext,
        IIdentityDbContext identityDbContext)
    {
        _tokenService = tokenService;
        _authenticationTokenContext = authenticationTokenContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<ExternalLoginDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var user = await LoadUserWithClaims(email, cancellationToken);

        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return ExternalLoginDto.Failed("Пользователь не найден или пароль не задан");
        }

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
        {
            return ExternalLoginDto.Failed("Неверный пароль");
        }

        if (!user.IsActive)
        {
            return ExternalLoginDto.Failed(PortalMessages.UserIsInactive);
        }

        var bearerToken = GenerateToken(user);
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

    private TokenModel GenerateToken(User user)
    {
        var claims = user.Claims
            .Select(userClaim => userClaim.ToClaim())
            .Union(user.UserRoles
                .SelectMany(userRole => userRole.Role!.Claims)
                .Select(x => x.ToClaim()))
            .Union(user.Department?.Claims.Select(x => x.ToClaim()) ?? [])
            .ToList();
        var roles = user.UserRoles.Select(x => x.Role!.Name!).ToList();

        if (user.UserRoles.Any(role => role.Role!.IsCompanyManager))
        {
            claims.Add(new Claim(PortalClaimTypeNames.IsManager, "1"));
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

    internal sealed record Command(string Email, string Password)
        : ICommand<ExternalLoginDto>, ITransactionalRequest, IIdentityRequest;
}
