using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Persistence.Abstractions.Auditing;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Persistence.Interceptors;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    private static readonly string SYSTEM_USER = "SYSTEM";

    public AuditableEntitySaveChangesInterceptor(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry is { Entity: IAuditable audit })
            {
                if (entry.State == EntityState.Added)
                {
                    audit.CreatedBy = _currentUser.UserName ?? SYSTEM_USER;
                    audit.Created = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    audit.LastModifiedBy = _currentUser.UserName ?? SYSTEM_USER;
                    audit.LastModified = DateTime.UtcNow;
                }
            }

            if (entry is not { State: EntityState.Deleted, Entity: ISoftDelete delete }) continue;

            delete.IsDeleted = true;
            delete.DeletedBy = _currentUser.UserName;
            delete.Deleted = DateTime.UtcNow;
            entry.State = EntityState.Modified;
        }
    }
}
