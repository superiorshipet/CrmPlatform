using CrmPlatform.Domain.Tenants;

namespace CrmPlatform.UnitTests;

public sealed class TenantTests
{
    [Fact]
    public void Tenant_normalizes_slug_and_starts_active()
    {
        var tenant = new Tenant("Acme Company", " Acme-EG ");

        Assert.Equal("acme-eg", tenant.Slug);
        Assert.Equal(TenantStatus.Active, tenant.Status);
    }

    [Fact]
    public void Tenant_rejects_empty_name()
    {
        Assert.Throws<ArgumentException>(() => new Tenant(" ", "acme"));
    }
}
