using System.Security.Claims;
using CrmPlatform.Application.Tenancy;

namespace CrmPlatform.Api.Tenancy;

internal sealed class HttpCurrentTenant(IHttpContextAccessor httpContextAccessor) : ICurrentTenant
{
    public const string TenantIdClaim = "tenant_id";

    public Guid? Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(TenantIdClaim);
            return Guid.TryParse(value, out var tenantId) && tenantId != Guid.Empty
                ? tenantId
                : null;
        }
    }

    public Guid RequireId() => Id.RequireTenantId();
}
