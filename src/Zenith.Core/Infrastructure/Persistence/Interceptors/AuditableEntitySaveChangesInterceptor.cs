using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Zenith.Common.Date;
using Zenith.Core.Infrastructure.Identity;

namespace Zenith.Core.Infrastructure.Persistence.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserContext _currentUserService;
        private readonly IDateTime _dateTime;

        public AuditableEntitySaveChangesInterceptor(
           IServiceProvider serviceProvider)
        {
            _currentUserService = serviceProvider.GetRequiredService<ICurrentUserContext>();
            _dateTime = serviceProvider.GetRequiredService<IDateTime>();
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
            var currentUser = _currentUserService.GetCurrentUserContext();
            if (context == null) return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var baseType = entry.Entity.GetType().BaseType;
                if (baseType != null && baseType.Name == "BaseAuditableEntity")
                {

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedBy").CurrentValue = currentUser.Id;
                        entry.Property("Created").CurrentValue = _dateTime.Now;
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                    {
                        entry.Property("LastModifiedBy").CurrentValue = currentUser.Id;
                        entry.Property("LastModified").CurrentValue = _dateTime.Now;
                    }

                }
            }
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }

}

