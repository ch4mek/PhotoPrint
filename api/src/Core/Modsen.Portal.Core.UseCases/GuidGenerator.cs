using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public sealed class GuidGenerator : IGuidGenerator
{
    public Guid Create()
    {
        return Guid.CreateVersion7();
    }
}
