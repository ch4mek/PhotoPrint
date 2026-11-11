using Microsoft.AspNetCore.Mvc;

namespace Modsen.Portal.Core.Authentication.External;

public interface IExternalAuthenticationService
{
    IActionResult GetAuthenticationActionResult(string returnUrl, string callbackUri);

    Task<UserInfoDto> GetUserInfoAsync(string token, CancellationToken cancellationToken);
}
