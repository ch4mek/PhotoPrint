using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

internal class KnowledgeBaseUserEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
}

internal class KnowledgeBaseUserConfiguration : IEntityTypeConfiguration<KnowledgeBaseUserEntity>
{
    public void Configure(EntityTypeBuilder<KnowledgeBaseUserEntity> builder)
    {
        builder.ToTable("KnowledgeBaseUsers");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(550);

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);
    }
}
