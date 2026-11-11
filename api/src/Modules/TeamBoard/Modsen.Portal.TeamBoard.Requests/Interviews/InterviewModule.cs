using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;

namespace Modsen.Portal.TeamBoard.Requests.Interviews;

internal sealed class InterviewModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IInterviewUsersValidator, InterviewUsersValidator>();
        services.AddScoped<IInterviewSaleManagerValidator, InterviewSaleManagerValidator>();

        services.AddScoped<IInterviewLoader, InterviewLoader>();

        services.AddScoped<IInterviewsAuthenticationFilter, InterviewsAuthenticationFilter>();
    }
}
