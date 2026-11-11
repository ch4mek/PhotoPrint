using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Core.Database.Interceptors;

public sealed class DatabaseAuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DatabaseAuditInterceptor(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        FillAuditColumns(eventData);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(eventData);
        FillAuditColumns(eventData);
        return base.SavingChanges(eventData, result);
    }

    private void FillAuditColumns(DbContextEventData eventData)
    {
        var nowUtc = DateTime.UtcNow;

        foreach (var entry in eventData.Context!.ChangeTracker
                     .Entries()
                     .Where(x => x.State is EntityState.Added or EntityState.Modified))
        {
            if (entry is { Entity: IPortalEntity portalEntity, State: EntityState.Added })
            {
                UpdateCreatedFields(portalEntity, nowUtc);
            }

            if (entry.Entity is IAuditableEntity entity)
            {
                if (entry.State == EntityState.Added)
                {
                    UpdateCreatedFields(entity, nowUtc);
                }

                if (entry.State == EntityState.Modified)
                {
                    UpdateUpdatedFields(entity, nowUtc);
                }
            }
        }
    }

    private void UpdateCreatedFields(IPortalEntity portalEntity, DateTime nowUtc)
    {
        portalEntity.CreatedAtUtc = nowUtc;
        if (portalEntity.CreatedById is <= 0 or null)
        {
            portalEntity.CreatedById = _authenticatedUserContext.UserId;
        }
    }

    private void UpdateUpdatedFields(IAuditableEntity auditableEntity, DateTime nowUtc)
    {
        auditableEntity.UpdatedAtUtc = nowUtc;
        if (auditableEntity.UpdatedById is <= 0 or null)
        {
            auditableEntity.UpdatedById = _authenticatedUserContext.UserId;
        }
    }
}
