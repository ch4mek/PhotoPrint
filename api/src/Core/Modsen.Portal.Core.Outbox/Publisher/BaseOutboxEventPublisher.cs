using System.Text.Json;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Outbox.Publisher;

public abstract class BaseOutboxEventPublisher
{
    private static readonly Dictionary<string, Type> s_eventTypes = GetEventTypes();

    protected static IIntegrationEvent? DeserializeOutboxEvent(OutboxEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var eventType = !integrationEvent.EventType.IsNullOrEmpty() &&
            s_eventTypes.TryGetValue(integrationEvent.EventType, out var type)
                ? type
                : null;

        var deserializedEvent = eventType != null
            ? JsonSerializer.Deserialize(integrationEvent.JsonData!, eventType) as IIntegrationEvent
            : null;

        return deserializedEvent;
    }

    private static Dictionary<string, Type> GetEventTypes()
    {
        var integrationEventType = typeof(IIntegrationEvent);

        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes().Where(x => x.IsAssignableTo(integrationEventType)))
            .Where(x => !x.Name.IsNullOrEmpty())
            .ToDictionary(x => x.Name, x => x);
    }
}
