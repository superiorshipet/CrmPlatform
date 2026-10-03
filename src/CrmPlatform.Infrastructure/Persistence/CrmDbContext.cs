using CrmPlatform.Application.Tenancy;
using CrmPlatform.Domain.Common;
using CrmPlatform.Domain.Organization;
using CrmPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatform.Infrastructure.Persistence;

public sealed class CrmDbContext(
    DbContextOptions<CrmDbContext> options,
    ICurrentTenant currentTenant) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Department> Departments => Set<Department>();

    private Guid? CurrentTenantId => currentTenant.Id;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);

        modelBuilder.Entity<Department>()
            .HasQueryFilter(department =>
                CurrentTenantId.HasValue && department.TenantId == CurrentTenantId.Value);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        EnforceTenantIsolation();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        EnforceTenantIsolation();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void EnforceTenantIsolation()
    {
        var tenantEntries = ChangeTracker
            .Entries()
            .Where(entry =>
                entry.Entity is TenantEntity &&
                entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToArray();

        if (tenantEntries.Length == 0)
        {
            return;
        }

        var currentTenantId = currentTenant.RequireId();

        foreach (var entry in tenantEntries)
        {
            var entity = (TenantEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                if (entity.TenantId != Guid.Empty && entity.TenantId != currentTenantId)
                {
                    throw new TenantIsolationViolationException(currentTenantId, entity.TenantId);
                }

                entity.AssignToTenant(currentTenantId);
                continue;
            }

            if (entity.TenantId != currentTenantId)
            {
                throw new TenantIsolationViolationException(currentTenantId, entity.TenantId);
            }

            var tenantProperty = entry.Property(nameof(TenantEntity.TenantId));
            if (tenantProperty.IsModified && tenantProperty.OriginalValue is Guid originalTenantId &&
                originalTenantId != currentTenantId)
            {
                throw new TenantIsolationViolationException(currentTenantId, originalTenantId);
            }
        }
    }
}
