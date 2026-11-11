namespace Modsen.Portal.Core.Authentication.SecurityHash;

public interface ISecurityHashValidator
{
    bool IsValidProvidedHash();
}
