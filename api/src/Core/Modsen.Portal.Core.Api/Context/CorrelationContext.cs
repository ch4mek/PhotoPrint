using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;

namespace Modsen.Portal.Core.Api.Context;

public sealed class CorrelationContext : ICorrelationContext
{
    private Guid? _correlationId;

    public Guid CorrelationId => _correlationId ??= Guid.CreateVersion7();
    public bool IsCorrelationIdEmpty => !_correlationId.HasValue || _correlationId == Guid.Empty;

    public void UseCorrelationId(Guid correlationId)
    {
        if (correlationId == Guid.Empty)
        {
            throw new ArgumentException("Provided Correlation ID is empty.");
        }

        if (!IsCorrelationIdEmpty)
        {
            throw new AppException($"Correlation ID is not empty. Use {nameof(IsCorrelationIdEmpty)} property to check it.");
        }

        _correlationId = correlationId;
    }
}
