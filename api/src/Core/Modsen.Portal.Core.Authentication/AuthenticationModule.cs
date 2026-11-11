using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Modsen.Portal.Core.Authentication.AgentKey;
using Modsen.Portal.Core.Authentication.ApiKey;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Authentication.PayslipsKey;
using Modsen.Portal.Core.Authentication.RecruitmentKey;
using Modsen.Portal.Core.Authentication.RegexApiKey;
using Modsen.Portal.Core.Authentication.SecurityHash;
using Modsen.Portal.Core.Authentication.TokenContext;
using Modsen.Portal.Core.Authentication.UserEventsKey;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.Authentication;

public sealed class AuthenticationModule(RegistrationModuleAggregate rootModule) : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var section = Configuration.GetSection("AppSettings").GetSection("AuthSettings");

        ArgumentNullException.ThrowIfNull(section);

        var authenticationOptions = new JwtAuthenticationOptions
        {
            Audience = section.GetValue<string>("Audience")!,
            Issuer = section.GetValue<string>("Issuer")!,
            SecurityKey = section.GetValue<string>("SecurityKey")!,
            TokenLifetimeInMinutes = section.GetValue<int>("TokenLifetime")
        };

        ArgumentException.ThrowIfNullOrEmpty(authenticationOptions.SecurityKey);
        ArgumentException.ThrowIfNullOrEmpty(authenticationOptions.Audience);
        ArgumentException.ThrowIfNullOrEmpty(authenticationOptions.Issuer);

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationTokenContext, AuthenticationTokenContext>();
        services.AddSingleton<IPublicTokenMaskProvider, PublicTokenMaskProvider>();
        services.AddSingleton<IIpAddressRegionProvider, IpAddressRegionProvider>();
        services.AddScoped<IRequestIpAddressProvider, RequestIpAddressProvider>();
        services.AddScoped<IAuthenticatedTokenStore, AuthenticatedTokenStore>();
        services.AddScoped(_ => authenticationOptions);

        services.AddScoped<ApiKeyHeaderAttribute>();
        services.AddScoped<IApiKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var xApiKey = configuration.GetValue<string>("AppSettings:AuthSettings:XApiKey");

            return new ApiKeyValidator(xApiKey, httpContextAccessor);
        });

        services.AddScoped<RegexApiKeyHeaderAttribute>();
        services.AddScoped<IRegexApiKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var regexApiKey = configuration.GetValue<string>("AppSettings:AuthSettings:XRegexApiKey");

            return new RegexApiKeyValidator(regexApiKey, httpContextAccessor);
        });

        services.AddScoped<RecruitmentKeyHeaderAttribute>();
        services.AddScoped<IRecruitmentKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var key = configuration.GetValue<string>("AppSettings:AuthSettings:XRecruitmentKey");

            return new RecruitmentKeyValidator(key, httpContextAccessor);
        });

        services.AddScoped<PayslipsApiKeyHeaderAttribute>();
        services.AddScoped<IPayslipsKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var key = configuration.GetValue<string>("AppSettings:AuthSettings:XPayslipsApiKey");

            return new PayslipsKeyValidator(key, httpContextAccessor);
        });

        services.AddScoped<UserEventsApiKeyHeaderAttribute>();
        services.AddScoped<IUserEventsKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var key = configuration.GetValue<string>("AppSettings:AuthSettings:XUserEventsApiKey");

            return new UserEventsKeyValidator(key, httpContextAccessor);
        });

        services.AddScoped<AgentApiKeyHeaderAttribute>();
        services.AddScoped<IAgentKeyValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var key = configuration.GetValue<string>("AppSettings:AuthSettings:XAgentApiKey");

            return new AgentKeyValidator(key, httpContextAccessor);
        });

        services.AddScoped<SecurityHashHeaderAttribute>();
        services.AddScoped<ISecurityHashValidator>(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var securityHashString = configuration.GetValue<string>("AppSettings:SecurityHashString");

            var hashService = serviceProvider.GetRequiredService<IHashService>();
            var clockService = serviceProvider.GetRequiredService<IClockService>();

            return new SecurityHashValidator(securityHashString, httpContextAccessor, hashService, clockService);
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authenticationOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authenticationOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authenticationOptions.SecurityKey)),
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/notifications", StringComparison.InvariantCulture))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = _ => Task.CompletedTask
                };
            })
            .AddMicrosoftIdentityWebApi(Configuration, jwtBearerScheme: OpenIdConnectDefaults.AuthenticationScheme);

        services.Configure<MicrosoftIdentityOptions>(options =>
        {
            options.Events.OnAccessDenied = _ => Task.CompletedTask;
            options.SaveTokens = false;
            options.UseTokenLifetime = false;
            options.RequireHttpsMetadata = true;
            options.NonceCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.SignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.SignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(ClaimTypes.Role)
                .Build();

            foreach (var rolePolicy in PortalPolicyNames.RolePolicies())
            {
                options.AddPolicy(rolePolicy.ClaimName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(ClaimTypes.Role, rolePolicy.Values);
                });
            }

            foreach (var policy in rootModule.Modules.SelectMany(x => x.GetPolicies()))
            {
                options.AddPolicy(policy.Name, policy.Policy);
            }
        });
    }
}
