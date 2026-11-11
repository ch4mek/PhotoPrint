using Modsen.Portal.Core.Authentication.ApiKey;

namespace Modsen.Portal.IntegrationTests.Mocks;

public class ApiKeyValidatorMock : IApiKeyValidator
{
    public bool IsValidProvidedKey()
    {
        return true;
    }
}
