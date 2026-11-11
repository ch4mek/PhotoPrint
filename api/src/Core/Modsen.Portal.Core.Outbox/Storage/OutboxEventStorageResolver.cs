using Modsen.Portal.Core.Communication;

namespace Modsen.Portal.Core.Outbox.Storage;

public interface IOutboxEventStorageResolver
{
    IOutboxEventRepository? Resolve<TEvent>() where TEvent : IIntegrationEvent;
}

internal sealed class OutboxEventStorageResolver : IOutboxEventStorageResolver
{
    private readonly IServiceProvider _serviceProvider;

    public OutboxEventStorageResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOutboxEventRepository? Resolve<TEvent>() where TEvent : IIntegrationEvent
    {
        var storageType = OutboxEventStorageConfiguration.GetStorageForEvent<TEvent>();
        if (storageType != null)
        {
            return _serviceProvider.GetService(storageType) as IOutboxEventRepository;
        }

        return null;
    }
}
