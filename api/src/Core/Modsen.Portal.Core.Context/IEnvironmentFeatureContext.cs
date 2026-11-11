namespace Modsen.Portal.Core.Context;

public interface IEnvironmentFeatureContext
{
    Task<bool> IsCheckAssignDatesFeatureEnabledAsync();
}
