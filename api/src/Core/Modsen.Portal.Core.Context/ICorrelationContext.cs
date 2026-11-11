namespace Modsen.Portal.Core.Context;

public interface ICorrelationContext
{
    Guid CorrelationId { get; }
    bool IsCorrelationIdEmpty { get; }

    void UseCorrelationId(Guid correlationId);
}
