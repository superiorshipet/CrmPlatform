namespace CrmPlatform.Domain.Common;

public abstract class TenantEntity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public Guid TenantId { get; private set; }

    public void AssignToTenant(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("A tenant identifier is required.", nameof(tenantId));
        }

        if (TenantId != Guid.Empty && TenantId != tenantId)
        {
            throw new InvalidOperationException("A tenant-owned entity cannot change tenants.");
        }

        TenantId = tenantId;
    }
}
