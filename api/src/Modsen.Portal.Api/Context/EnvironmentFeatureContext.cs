using Microsoft.FeatureManagement;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Api.Context;

public sealed class EnvironmentFeatureContext : IEnvironmentFeatureContext
{
    private readonly IFeatureManager _featureManager;
    private readonly string _featureCheckProjectAssignDates = "FeatureCheckProjectAssignDates";

    public EnvironmentFeatureContext(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    public async Task<bool> IsCheckAssignDatesFeatureEnabledAsync()
    {
        return await _featureManager.IsEnabledAsync(_featureCheckProjectAssignDates);
    }
}
