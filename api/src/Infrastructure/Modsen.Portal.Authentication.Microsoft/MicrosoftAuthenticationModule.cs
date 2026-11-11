using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Authentication;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Authentication.Microsoft;

public sealed class MicrosoftAuthenticationModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var getUserInfoUrl = Configuration.GetValue<string>("AppSettings:AuthSettings:MicrosoftUserInfoUrl");

        ArgumentException.ThrowIfNullOrWhiteSpace(getUserInfoUrl);

        services.AddScoped<IExternalAuthenticationService, MicrosoftAuthenticationService>();

        services.AddAuthentication(AuthenticationSchemeNames.AzureAd)
            .AddAzureAD(options =>
                Configuration.Bind(MicrosoftAuthenticationConstants.AzureAdSettings, options));

        services.Configure<OpenIdConnectOptions>(MicrosoftAuthenticationConstants.AzureOpenIdScheme, options =>
        {
            options.Events.OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.Prompt = "select_account";

                return Task.CompletedTask;
            };

            options.Events.OnRemoteFailure = context =>
            {
                var returnUrl = context.Properties?.Items["returnUrl"];
                if (returnUrl != null)
                {
                    context.Response.Redirect($"{returnUrl}?error=Remote failure");
                }

                context.HandleResponse();

                return Task.CompletedTask;
            };

            options.RequireHttpsMetadata = true;
        });

        services.AddHttpClient(MicrosoftAuthenticationConstants.GetUserInfoUrlHttpClientName, options =>
        {
            options.BaseAddress = new Uri(getUserInfoUrl);
            options.Timeout = TimeSpan.FromSeconds(60);
        });
    }
}
