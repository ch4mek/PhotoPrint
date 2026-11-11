using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Authentication;
using Modsen.Portal.Core.Authentication.External;

namespace Modsen.Portal.IntegrationTests;

internal class MicrosoftAuthenticationServiceMock : IExternalAuthenticationService
{
    public IActionResult GetAuthenticationActionResult(string returnUrl, string callbackUri)
    {
        var scheme = AuthenticationSchemeNames.AzureAd;

        return new ChallengeResult
        {
            AuthenticationSchemes = new List<string>
            {
                scheme
            },
            Properties = new AuthenticationProperties
            {
                RedirectUri = callbackUri,
                Items =
                {
                    [nameof(scheme)] = scheme,
                    [nameof(returnUrl)] = returnUrl
                }
            }
        };
    }

    public Task<UserInfoDto> GetUserInfoAsync(string token, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserInfoDto
        {
            Email = "test@modsen-software.com",
            GivenName = "Test",
            FamilyName = "Test"
        });
    }
}
