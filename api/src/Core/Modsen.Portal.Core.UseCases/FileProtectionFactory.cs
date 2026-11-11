using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public class FileProtectionFactory : IFileProtectionFactory
{
    private readonly IConfiguration _configuration;

    public FileProtectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IFileProtectionService Create(string scope)
    {
        var key = _configuration.GetValue<string>($"Encryption:{scope}:Key");       // 32

        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return new FileProtectionService(key);
    }
}
