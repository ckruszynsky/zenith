﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenith.Common.Date;
using Zenith.Common.Identity;

namespace Zenith.Core.Infrastructure.Persistence.Interceptors
{
    public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public AuditableEntitySaveChangesInterceptor(
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
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
                var baseType = entry.Entity.GetType().BaseType;
                if (baseType != null && baseType.Name == "BaseAuditableEntity")
                {

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreatedBy").CurrentValue = _currentUserService.UserId;
                        entry.Property("Created").CurrentValue = _dateTime.Now;
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                    {
                        entry.Property("LastModifiedBy").CurrentValue = _currentUserService.UserId;
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
