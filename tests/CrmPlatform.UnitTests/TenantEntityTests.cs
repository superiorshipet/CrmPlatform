using CrmPlatform.Domain.Organization;

namespace CrmPlatform.UnitTests;

public sealed class TenantEntityTests
{
    [Fact]
    public void AssignToTenant_cannot_move_entity_between_tenants()
    {
        var firstTenant = Guid.NewGuid();
        var department = new Department("Engineering", "eng");

        department.AssignToTenant(firstTenant);

        var exception = Assert.Throws<InvalidOperationException>(
            () => department.AssignToTenant(Guid.NewGuid()));

        Assert.Equal("A tenant-owned entity cannot change tenants.", exception.Message);
        Assert.Equal(firstTenant, department.TenantId);
    }

    [Fact]
    public void Department_normalizes_its_code()
    {
        var department = new Department(" Engineering ", " eng ");

        Assert.Equal("Engineering", department.Name);
        Assert.Equal("ENG", department.Code);
    }
}
