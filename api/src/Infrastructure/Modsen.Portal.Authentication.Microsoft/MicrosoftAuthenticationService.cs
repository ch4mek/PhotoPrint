using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Authentication.Microsoft.ApiModels;
using Modsen.Portal.Core.Authentication;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Authentication.Microsoft;

public sealed class MicrosoftAuthenticationService : IExternalAuthenticationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MicrosoftAuthenticationService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

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

    public async Task<UserInfoDto> GetUserInfoAsync(string token, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(MicrosoftAuthenticationConstants.GetUserInfoUrlHttpClientName);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetAsync(string.Empty, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalUnauthorizedException();
        }

        var result = await response.Content.ReadFromJsonAsync<MeModel>(cancellationToken: cancellationToken);
        if (result != null)
        {
            return new UserInfoDto
            {
                FamilyName = result.Surname,
                GivenName = result.GivenName,
                Email = !result.Email.IsNullOrWhiteSpace()
                    ? result.Email
                    : result.PrincipalName
            };
        }

        throw new AppException("Received empty response from Microsoft API (Get User Info) or unexpected message format");
    }
}
