using CrmPlatform.Application.Tenancy;

namespace CrmPlatform.IntegrationTests.Tenancy;

internal sealed class TestCurrentTenant(Guid? tenantId) : ICurrentTenant
{
    public Guid? Id { get; } = tenantId;

    public Guid RequireId() => Id.RequireTenantId();
}
