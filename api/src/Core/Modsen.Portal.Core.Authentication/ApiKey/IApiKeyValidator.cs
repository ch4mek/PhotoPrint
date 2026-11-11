namespace Modsen.Portal.Core.Authentication.ApiKey;

public interface IApiKeyValidator
{
    bool IsValidProvidedKey();
}
