using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class Department
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public string? ModsenApiValue { get; set; }
    public int OrderWeight { get; set; }
    public Role? DepartmentReportingToRole { get; set; }
    public int? DepartmentReportingToRoleId { get; set; }
    public ICollection<DepartmentClaim> Claims { get; set; } = [];
}

internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(x => x.ShortName)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.ModsenApiValue)
            .IsRequired(false)
            .HasMaxLength(128);

        builder.HasOne(x => x.DepartmentReportingToRole)
            .WithMany()
            .HasForeignKey(x => x.DepartmentReportingToRoleId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
