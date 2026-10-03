namespace CrmPlatform.Domain.Tenants;

public sealed class Tenant
{
    private Tenant()
    {
    }

    public Tenant(string name, string slug)
    {
        Id = Guid.NewGuid();
        Name = Required(name, nameof(name), 160);
        Slug = Required(slug, nameof(slug), 80).ToLowerInvariant();
        Status = TenantStatus.Active;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private init; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public TenantStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public void Rename(string name) => Name = Required(name, nameof(name), 160);

    public void Suspend() => Status = TenantStatus.Suspended;

    public void Activate() => Status = TenantStatus.Active;

    private static string Required(string value, string parameterName, int maximumLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("A value is required.", parameterName);
        }

        if (normalized.Length > maximumLength)
        {
            throw new ArgumentException($"The value cannot exceed {maximumLength} characters.", parameterName);
        }

        return normalized;
    }
}

public enum TenantStatus
{
    Active = 1,
    Suspended = 2,
    Archived = 3,
}
