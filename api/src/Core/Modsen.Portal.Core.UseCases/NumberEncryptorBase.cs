using System.Globalization;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public interface INumberEncryptor
{
    public string? Encrypt(decimal? number);
    public decimal Decrypt(string? encryptedText);
}

public abstract class NumberEncryptorBase : INumberEncryptor
{
    private readonly IDataProtectionService? _dataProtector;

    protected NumberEncryptorBase(IDataProtectionFactory factory, string scope)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _dataProtector = factory.Create(scope);
    }

    public string? Encrypt(decimal? number)
    {
        ArgumentNullException.ThrowIfNull(_dataProtector);

        if (number is null)
        {
            return null;
        }

        var salaryAsString = number.Value.ToString("0.####", CultureInfo.InvariantCulture);
        return _dataProtector.Encrypt(salaryAsString);
    }

    public decimal Decrypt(string? encryptedText)
    {
        ArgumentNullException.ThrowIfNull(_dataProtector);

        if (encryptedText is null)
        {
            return 0m;
        }

        return decimal.TryParse(_dataProtector.Decrypt(encryptedText), CultureInfo.InvariantCulture, out var value)
            ? value
            : 0m;
    }
}
