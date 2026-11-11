using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class TeamBoardProject
{
    public int Id { get; set; }
    public required string ProjectName { get; set; }
    public required string ClientName { get; set; }
    public string? Color { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public bool IsActive { get; set; }

    public bool IsPublic { get; set; }

    public List<ProjectExit> ProjectExits { get; set; } = [];
}

internal class TeamBoardProjectConfiguration : IEntityTypeConfiguration<TeamBoardProject>
{
    public void Configure(EntityTypeBuilder<TeamBoardProject> builder)
    {
        builder.ToTable("TeamBoardProjects");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ProjectName).HasMaxLength(250).IsRequired();

        builder.Property(x => x.ClientName).HasMaxLength(250).IsRequired();

        builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);

        builder.Property(x => x.IsPublic).IsRequired();
    }
}
