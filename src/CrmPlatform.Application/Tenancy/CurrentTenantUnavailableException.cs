namespace CrmPlatform.Application.Tenancy;

public sealed class CurrentTenantUnavailableException()
    : InvalidOperationException("The current request does not contain a trusted tenant identifier.");
