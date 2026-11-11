using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class JiraSetting
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public required string Pat { get; set; }
    public required string JiraProjectTag { get; set; }
    public string? Email { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    public int UserId { get; set; }
    public TimeTrackUser? User { get; set; }
    public bool IsActive { get; set; }
}

internal class JiraSettingConfiguration : IEntityTypeConfiguration<JiraSetting>
{
    public void Configure(EntityTypeBuilder<JiraSetting> builder)
    {
        builder.ToTable("JiraSettings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Url).IsRequired();
        builder.Property(x => x.Pat).IsRequired();
        builder.Property(x => x.JiraProjectTag).IsRequired();
        builder.Property(x => x.Email)
            .HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

