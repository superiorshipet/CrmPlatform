using CrmPlatform.Domain.Organization;
using CrmPlatform.Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmPlatform.Infrastructure.Persistence.Configurations;

internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(department => department.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(department => new { department.TenantId, department.Code })
            .IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(department => department.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
