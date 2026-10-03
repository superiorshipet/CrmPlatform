namespace CrmPlatform.Application.Tenancy;

public sealed class TenantIsolationViolationException(Guid expectedTenantId, Guid entityTenantId)
    : InvalidOperationException(
        $"Tenant isolation rejected access to tenant '{entityTenantId}' from tenant '{expectedTenantId}'.");
