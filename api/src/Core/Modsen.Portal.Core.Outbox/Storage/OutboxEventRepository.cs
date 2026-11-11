using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.Outbox.Storage;

public interface IOutboxEventRepository
{
    string StorageScope { get; }

    Task Add<T>(T integrationEvent, CancellationToken cancellationToken) where T : IIntegrationEvent;
    Task Add<T>(T[] integrationEvents, CancellationToken cancellationToken) where T : IIntegrationEvent;

    Task<int> GetCountOfUndeliveredEvents(CancellationToken cancellationToken);
    Task<Dictionary<string, int>> GetDetailsOfUndeliveredEvents(CancellationToken cancellationToken);
}

public interface IOutboxEventRepository<out TContext> : IOutboxEventRepository where TContext : DbContext
{
    TContext StoringContext { get; }
    Task<OutboxEvent?> GetNextUnsentEvent(CancellationToken cancellationToken);
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

internal sealed class OutboxEventRepository<TContext> : IOutboxEventRepository<TContext> where TContext : DbContext
{
    private readonly IClockService _clockService;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICorrelationContext _correlationContext;

    public OutboxEventRepository(
        TContext dbContext,
        IClockService clockService,
        IAuthenticatedUserContext authenticatedUserContext,
        ICorrelationContext correlationContext)
    {
        StoringContext = dbContext;
        _clockService = clockService;
        _authenticatedUserContext = authenticatedUserContext;
        _correlationContext = correlationContext;
    }

    public TContext StoringContext { get; private set; }

    public Task<OutboxEvent?> GetNextUnsentEvent(CancellationToken cancellationToken)
    {
        return StoringContext.Set<OutboxEvent>()
            .Where(x => !x.WasSent)
            .OrderBy(e => e.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> GetCountOfUndeliveredEvents(CancellationToken cancellationToken)
    {
        return StoringContext.Set<OutboxEvent>()
            .Where(x => !x.WasSent)
            .CountAsync(cancellationToken);
    }

    public Task<Dictionary<string, int>> GetDetailsOfUndeliveredEvents(CancellationToken cancellationToken)
    {
        return StoringContext.Set<OutboxEvent>()
            .Where(x => !x.WasSent)
            .Where(x => x.EventType != null)
            .GroupBy(e => e.EventType)
            .Select(g => new
            {
                EventType = g.Key,
                Count = g.Count()
            })
            .ToDictionaryAsync(x => x.EventType!, x => x.Count, cancellationToken);
    }

    public string StorageScope => typeof(TContext).Name;

    public Task Add<T>(T integrationEvent, CancellationToken cancellationToken) where T : IIntegrationEvent
    {
        StoringContext.Set<OutboxEvent>().Add(MapToOutboxEvent(integrationEvent));
        return StoringContext.SaveChangesAsync(cancellationToken);
    }

    public Task Add<T>(T[] integrationEvents, CancellationToken cancellationToken) where T : IIntegrationEvent
    {
        StoringContext.Set<OutboxEvent>().AddRange(integrationEvents.Select(MapToOutboxEvent));
        return StoringContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken)
    {
        return StoringContext.SaveChangesAsync(cancellationToken);
    }

    private OutboxEvent MapToOutboxEvent<T>(T @event) where T : IIntegrationEvent
    {
        var jsonData = JsonSerializer.Serialize(@event);
        return new OutboxEvent
        {
            Id = Guid.CreateVersion7(),
            CreatedAtUtc = _clockService.UtcNow,
            CorrelationId = _correlationContext.CorrelationId,
            CreatedById = _authenticatedUserContext.UserId,
            WasSent = false,
            JsonData = jsonData,
            EventType = typeof(T).Name
        };
    }
}
