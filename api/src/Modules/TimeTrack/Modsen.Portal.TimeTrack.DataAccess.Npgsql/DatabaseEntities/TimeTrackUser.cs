using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class TimeTrackUser
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public TimeTrackResourceGroup? ResourceGroup { get; set; }
    public string? Color { get; set; }
    public string? AvatarFileId { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedById { get; set; }
    public List<TimesheetSubmitStatus> TimesheetSubmitStatuses { get; set; } = [];
    public List<Timesheet> Timesheets { get; set; } = [];
    public List<ProjectUser> Projects { get; set; } = [];

    public static Domain.TimeTrackUser? ToDomain(TimeTrackUser? source)
    {
        if (source == null)
        {
            return null;
        }

        return new Domain.TimeTrackUser()
        {
            UserId = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Email = source.Email,
            DepartmentId = source.DepartmentId,
            ResourceGroupId = source.ResourceGroupId,
            Color = source.Color,
            AvatarFileId = source.AvatarFileId,
            IsActive = source.IsActive,
        };
    }
}

internal class TimeTrackUserConfiguration : IEntityTypeConfiguration<TimeTrackUser>
{
    public void Configure(EntityTypeBuilder<TimeTrackUser> builder)
    {
        builder.ToTable("TimeTrackUsers", TimeTrackDbContext.Schema);

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Color).IsRequired(false).HasMaxLength(7).IsFixedLength();

        builder.Property(x => x.AvatarFileId).IsRequired(false).HasMaxLength(128);

        builder.Property(x => x.Email).IsRequired()
            .HasMaxLength(PortalSharedConstants.EmailMaxLength);
        builder.HasIndex(x => x.Email);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(value: true);
        builder.HasIndex(x => x.IsActive);

        builder.Property(x => x.Position)
            .IsRequired(false)
            .HasConversion<string>();

        builder.Property(x => x.CreatedById).IsRequired(false);
    }
}
