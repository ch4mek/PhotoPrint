namespace Modsen.Portal.Core.Context;

public interface IHostEnvironmentContext
{
    bool IsProductionEnvironment();

    bool IsDevelopmentOrQaEnvironment();

    bool IsStagingEnvironment();

    bool IsDevelopmentEnvironment();

    bool IsQaEnvironment();

    bool IsFunctionalTestEnvironment();

    bool IsIntegrationTestEnvironment();
}
