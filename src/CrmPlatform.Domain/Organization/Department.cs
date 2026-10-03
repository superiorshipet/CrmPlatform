using CrmPlatform.Domain.Common;

namespace CrmPlatform.Domain.Organization;

public sealed class Department : TenantEntity
{
    private Department()
    {
    }

    public Department(string name, string code)
    {
        Name = Normalize(name, nameof(name), 120);
        Code = Normalize(code, nameof(code), 30).ToUpperInvariant();
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public void Rename(string name) => Name = Normalize(name, nameof(name), 120);

    public void Deactivate() => IsActive = false;

    private static string Normalize(string value, string parameterName, int maximumLength)
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
