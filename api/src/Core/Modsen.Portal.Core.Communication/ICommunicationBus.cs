namespace Modsen.Portal.Core.Communication;

public interface ICommunicationBus
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken) where T : IIntegrationEvent;
    Task PublishAsync<T>(T[] integrationEvents, CancellationToken cancellationToken) where T : IIntegrationEvent;
}
