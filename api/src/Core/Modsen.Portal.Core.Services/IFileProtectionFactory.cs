namespace Modsen.Portal.Core.Services;

public interface IFileProtectionFactory
{
    IFileProtectionService Create(string scope);
}
