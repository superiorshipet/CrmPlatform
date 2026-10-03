namespace CrmPlatform.Application.Tenancy;

public interface ICurrentTenant
{
    Guid? Id { get; }

    Guid RequireId();
}
