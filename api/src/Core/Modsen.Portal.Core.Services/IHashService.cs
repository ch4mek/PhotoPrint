namespace Modsen.Portal.Core.Services;

public interface IHashService
{
    string GetSha256Hash(string input);
}
