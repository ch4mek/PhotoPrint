using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities
{
    internal class TimeTrackResourceGroup
    {
        public int ResourceGroupId { get; set; }
        public required string ResourceGroupName { get; set; }
        public bool IsActive { get; set; }
    }

    internal class TimeTrackResourceGroupConfiguration : IEntityTypeConfiguration<TimeTrackResourceGroup>
    {
        public void Configure(EntityTypeBuilder<TimeTrackResourceGroup> builder)
        {
            builder.ToTable("TimeTrackResourceGroups");

            builder.HasKey(x => x.ResourceGroupId);
            builder.Property(x => x.ResourceGroupId).ValueGeneratedNever().HasMaxLength(64);

            builder.Property(x => x.ResourceGroupName).IsRequired().HasMaxLength(128);
        }
    }
}
