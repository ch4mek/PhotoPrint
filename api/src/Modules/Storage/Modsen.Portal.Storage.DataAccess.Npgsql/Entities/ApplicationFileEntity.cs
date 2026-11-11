using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Storage.DataAccess.Npgsql.Entities;

public class ApplicationFileEntity
{
    public required string FileId { get; set; }
    public required string BucketName { get; set; }
    public required string VirtualDirectory { get; set; }
    public required string FilePath { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public int? UploadedByUserId { get; set; }
    public DateTime? AvailableTill { get; set; }
    public ImageResizingStatus ImageStatus { get; set; }
    public DateTime? ImageVariantsBackfilledAtUtc { get; set; }
    public FileSource FileSource { get; set; }
    public ApplicationFilePurpose FilePurpose { get; set; }
}

internal class ApplicationFileConfiguration : IEntityTypeConfiguration<ApplicationFileEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationFileEntity> builder)
    {
        builder.ToTable("ApplicationFiles");

        builder.HasKey(x => x.FileId);
        builder.Property(x => x.FileId).ValueGeneratedNever();
        builder.Property(x => x.FileId).HasMaxLength(64);

        builder.Property(x => x.BucketName).IsRequired().HasMaxLength(64);

        builder.Property(x => x.VirtualDirectory).IsRequired().HasMaxLength(256);

        builder.Property(x => x.FilePath).IsRequired().HasMaxLength(256);

        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(256);

        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(128);

        builder.Property(x => x.UploadedByUserId).IsRequired(false);

        builder.Property(x => x.AvailableTill).IsRequired(false);

        builder.Property(x => x.ImageStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ImageResizingStatus.NotProcessed);

        builder.Property(x => x.ImageVariantsBackfilledAtUtc)
            .IsRequired(false);

        builder.Property(x => x.FileSource)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(FileSource.None);

        builder.Property(x => x.FilePurpose)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ApplicationFilePurpose.General);

        builder
            .HasIndex(x => x.FileId, name: "ix_applicationfiles_pending_backfill")
            .HasFilter("\"ImageStatus\" != 'Processed'");
    }
}
