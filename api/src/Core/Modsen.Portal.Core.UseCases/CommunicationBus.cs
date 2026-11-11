using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Outbox.Storage;

namespace Modsen.Portal.Core.UseCases;

public class CommunicationBus : ICommunicationBus
{
    private readonly IOutboxEventStorageResolver _outboxEventStorageResolver;

    public CommunicationBus(IOutboxEventStorageResolver outboxEventStorageResolver)
    {
        _outboxEventStorageResolver = outboxEventStorageResolver;
    }

    public Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken)
        where T : IIntegrationEvent
    {
        return GetRepository<T>().Add(integrationEvent, cancellationToken);
    }

    public Task PublishAsync<T>(T[] integrationEvents, CancellationToken cancellationToken)
        where T : IIntegrationEvent
    {
        return GetRepository<T>().Add(integrationEvents, cancellationToken);
    }

    private IOutboxEventRepository GetRepository<T>() where T : IIntegrationEvent
    {
        return _outboxEventStorageResolver.Resolve<T>() ?? throw new AppException($"Could not resolve repository for outbox event: {typeof(T).Name}");
    }
}
