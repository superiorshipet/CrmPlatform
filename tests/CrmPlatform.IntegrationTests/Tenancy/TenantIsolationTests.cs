using CrmPlatform.Application.Tenancy;
using CrmPlatform.Domain.Organization;
using CrmPlatform.Domain.Tenants;
using CrmPlatform.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CrmPlatform.IntegrationTests.Tenancy;

public sealed class TenantIsolationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private DbContextOptions<CrmDbContext> _options = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        _options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var context = CreateContext(null);
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync() => await _connection.DisposeAsync();

    [Fact]
    public async Task Query_filter_returns_only_the_current_tenant_data()
    {
        var firstTenant = new Tenant("First Company", "first-company");
        var secondTenant = new Tenant("Second Company", "second-company");

        await using (var platformContext = CreateContext(null))
        {
            platformContext.Tenants.AddRange(firstTenant, secondTenant);
            await platformContext.SaveChangesAsync();
        }

        await using (var firstContext = CreateContext(firstTenant.Id))
        {
            firstContext.Departments.Add(new Department("Engineering", "ENG"));
            await firstContext.SaveChangesAsync();
        }

        await using (var secondContext = CreateContext(secondTenant.Id))
        {
            secondContext.Departments.Add(new Department("Human Resources", "HR"));
            await secondContext.SaveChangesAsync();
        }

        await using var queryContext = CreateContext(firstTenant.Id);
        var departments = await queryContext.Departments.AsNoTracking().ToListAsync();

        var department = Assert.Single(departments);
        Assert.Equal("Engineering", department.Name);
        Assert.Equal(firstTenant.Id, department.TenantId);
    }

    [Fact]
    public async Task SaveChanges_assigns_the_trusted_current_tenant()
    {
        var tenant = new Tenant("Acme", "acme");

        await using (var platformContext = CreateContext(null))
        {
            platformContext.Tenants.Add(tenant);
            await platformContext.SaveChangesAsync();
        }

        var department = new Department("Operations", "OPS");
        await using (var tenantContext = CreateContext(tenant.Id))
        {
            tenantContext.Departments.Add(department);
            await tenantContext.SaveChangesAsync();
        }

        Assert.Equal(tenant.Id, department.TenantId);
    }

    [Fact]
    public async Task SaveChanges_rejects_a_cross_tenant_update()
    {
        var firstTenant = new Tenant("First Company", "first-company");
        var secondTenant = new Tenant("Second Company", "second-company");
        var department = new Department("Engineering", "ENG");

        await using (var platformContext = CreateContext(null))
        {
            platformContext.Tenants.AddRange(firstTenant, secondTenant);
            await platformContext.SaveChangesAsync();
        }

        await using (var firstContext = CreateContext(firstTenant.Id))
        {
            firstContext.Departments.Add(department);
            await firstContext.SaveChangesAsync();
        }

        department.Rename("Compromised");
        await using var secondContext = CreateContext(secondTenant.Id);
        secondContext.Departments.Update(department);

        await Assert.ThrowsAsync<TenantIsolationViolationException>(
            () => secondContext.SaveChangesAsync());
    }

    private CrmDbContext CreateContext(Guid? tenantId) =>
        new(_options, new TestCurrentTenant(tenantId));
}
