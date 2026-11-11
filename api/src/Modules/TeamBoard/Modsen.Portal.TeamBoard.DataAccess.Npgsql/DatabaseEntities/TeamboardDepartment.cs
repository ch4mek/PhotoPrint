using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class TeamboardDepartment
{
    public required string DepartmentId { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public int DepartmentOrderWeight { get; set; }
    public string? DepartmentReportingToRoleName { get; set; }
}

internal class TeamboardDepartmentConfiguration : IEntityTypeConfiguration<TeamboardDepartment>
{
    public void Configure(EntityTypeBuilder<TeamboardDepartment> builder)
    {
        builder.ToTable("TeamboardDepartments");

        builder.HasKey(x => x.DepartmentId);
        builder.Property(x => x.DepartmentId).ValueGeneratedNever();
        builder.Property(x => x.DepartmentId).HasMaxLength(64).IsRequired();

        builder.Property(x => x.ShortName).IsRequired().HasMaxLength(32);
        builder.Property(x => x.FullName).IsRequired().HasMaxLength(128);
        builder.Property(x => x.DepartmentOrderWeight).IsRequired();

        builder.Property(x => x.DepartmentReportingToRoleName).IsRequired();
    }
}
