namespace Modsen.Portal.Core.Services;

public interface IDataProtectionFactory
{
    IDataProtectionService Create(string scope);
}
