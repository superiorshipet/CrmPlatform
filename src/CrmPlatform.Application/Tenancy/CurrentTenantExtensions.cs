namespace CrmPlatform.Application.Tenancy;

public static class CurrentTenantExtensions
{
    public static Guid RequireTenantId(this Guid? tenantId) =>
        tenantId is { } value && value != Guid.Empty
            ? value
            : throw new CurrentTenantUnavailableException();
}
