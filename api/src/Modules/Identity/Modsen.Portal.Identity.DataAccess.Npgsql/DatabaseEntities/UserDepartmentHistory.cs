using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities
{
    internal class UserDepartmentHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public required string DepartmentId { get; set; }

        public DateOnly DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }

    internal class UserDepartmentHistoryConfiguration : IEntityTypeConfiguration<UserDepartmentHistory>
    {
        public void Configure(EntityTypeBuilder<UserDepartmentHistory> builder)
        {
            builder.ToTable("UserDepartmentHistory", IdentityModuleDbContext.Schema);

            builder.HasKey(e => e.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.DepartmentId)
                .IsRequired();

            builder.Property(x => x.DateFrom)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.DepartmentId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserDepartmentHistory)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Department>()
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
