using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

internal sealed class DataProtectionFactory : IDataProtectionFactory
{
    private readonly IConfiguration _configuration;

    public DataProtectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDataProtectionService Create(string scope)
    {
        var key = _configuration.GetValue<string>($"Encryption:{scope}:Key");           // 32
        var vector = _configuration.GetValue<string>($"Encryption:{scope}:Vector");     // 16

        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(vector);

        return new DataProtectionService(key, vector);
    }
}
