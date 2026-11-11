using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Authentication;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Account")]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthenticationTokenContext _authenticationTokenContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IExternalAuthenticationService _authenticationService;
    private readonly ISender _sender;
    private readonly IUrlEncoder _urlEncoder;
    private readonly IRequestExecutionContext _requestExecutionContext;

    public AccountController(
        IAuthenticationTokenContext authenticationTokenContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IExternalAuthenticationService authenticationService,
        IRequestExecutionContext requestExecutionContext,
        ISender sender,
        IUrlEncoder urlEncoder)
    {
        _authenticationTokenContext = authenticationTokenContext;
        _authenticatedUserContext = authenticatedUserContext;
        _authenticationService = authenticationService;
        _requestExecutionContext = requestExecutionContext;
        _sender = sender;
        _urlEncoder = urlEncoder;
    }

    /// <summary>
    /// Generates an authentication link with optional parameters for external login.
    /// Allowed Roles: Anonymous
    /// </summary>
    /// <remarks>
    ///This endpoint generates a link with parameters for external authentication and redirects the user to a 'externalLoginCallback' method where they can log in.
    /// </remarks>
    /// <param name="returnUrl">
    /// A string containing the URL to which the user will be redirected after successful(unsuccessful) authentication.
    /// </param>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("login")]
    public IActionResult ExternalLogin([FromQuery] string returnUrl)
    {
        return _authenticationService.GetAuthenticationActionResult(
            returnUrl,
            Url.Action(nameof(ExternalLoginCallbackAsync))!);
    }

    /// <summary>
    /// Executes Microsoft login authentication using the provided Microsoft token
    /// and generates a custom token for authorization.
    /// </summary>
    /// <remarks>
    /// This method facilitates authentication through Microsoft's services. It validates
    /// the provided Microsoft token and generates a custom token for further use in
    /// authorization processes.
    /// </remarks>
    /// <param name="loginModel">
    /// Object containing the necessary authentication information, including the Microsoft Token.
    /// </param>
    /// <returns>
    /// Custom token generated after successful authentication via Microsoft.
    /// </returns>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("microsoftLogin")]
    public async Task<IActionResult> MicrosoftLogin([FromBody] MicrosoftLoginDto loginModel)
    {
        ArgumentNullException.ThrowIfNull(loginModel);

        var userModel = await _authenticationService.GetUserInfoAsync(loginModel.Token, HttpContext.RequestAborted);
        var authenticateResult = new AuthenticationResult
        {
            Email = userModel.Email,
            GivenName = userModel.GivenName,
            Succeeded = true,
            Surname = userModel.FamilyName
        };
        var query = new ExternalLoginCallbackRequest.Command(authenticateResult);
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return !result.IsSucceeded
            ? Unauthorized(result.Error)
            : Ok(result.Token?.ToUrlEncodedString());
    }

    /// <summary>
    /// Локальный вход по email и паролю (демо-режим без Azure AD).
    /// Демо-пароль для всех сид-пользователей: demo1234.
    /// </summary>
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("localLogin")]
    public async Task<IActionResult> LocalLogin([FromBody] LocalLoginDto loginModel)
    {
        ArgumentNullException.ThrowIfNull(loginModel);

        var query = new LocalLoginRequest.Command(loginModel.Email, loginModel.Password);
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return !result.IsSucceeded
            ? Unauthorized(result.Error)
            : Ok(result.Token?.ToUrlEncodedString());
    }

    public sealed record LocalLoginDto(string Email, string Password);

    /// <summary>
    ///Initiates the AzureAD login process.
    /// Allowed Roles: Anonymous
    /// </summary>
    /// <remarks>
    ///This method is used to initiate the login process through Azure Active Directory (AzureAD).
    ///After a successful login, the user will be redirected back to the site with login information, and, to the specified URL.
    /// Url located in the properties like item.
    /// </remarks>
    [ProducesResponseType(StatusCodes.Status302Found)]
    [AllowAnonymous]
    [HttpGet("externalLoginCallback")]
    public async Task<IActionResult> ExternalLoginCallbackAsync()
    {
        var authenticateResult = await _requestExecutionContext.AuthenticateAsync(AuthenticationSchemeNames.AzureAd);
        var query = new ExternalLoginCallbackRequest.Command(authenticateResult);
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Redirect(GetRedirectUrl());

        string GetRedirectUrl()
        {
            return result.IsSucceeded
                ? new UriBuilder(authenticateResult.ReturnUrl!) { Query = result.Token?.ToUrlEncodedString() }.Uri
                    .ToString()
                : $"{authenticateResult.ReturnUrl}?error={_urlEncoder.Encode(result.Error!)}";
        }
    }

    /// <summary>
    /// Logout user
    /// Allowed Roles: Any
    /// </summary>
    /// <remarks>
    /// This method is used to terminate a user's session and log them out of the system.
    /// </remarks>
    [AllowAnonymous]
    [HttpGet("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> LogoutAsync()
    {
        if (_authenticatedUserContext.IsAuthenticated)
        {
            await HttpContext.SignOutAsync(AuthenticationSchemeNames.AzureAd);
            foreach (var bearerToken in _authenticatedUserContext.GetBearerTokens())
            {
                await _authenticationTokenContext.RevokeUserToken(_authenticatedUserContext.GetRequiredUserId(), bearerToken, RevokeReasons.Logout, HttpContext.RequestAborted);
            }
        }

        return Ok();
    }
}
