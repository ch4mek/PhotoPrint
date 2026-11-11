using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.ApiClient.Mock;

public sealed class MockPeopleForceApiClientModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.RemoveAll<IPeopleForceApiClient>();
        services.AddSingleton<IPeopleForceApiClient, MockPeopleForceApiClient>();
    }
}
