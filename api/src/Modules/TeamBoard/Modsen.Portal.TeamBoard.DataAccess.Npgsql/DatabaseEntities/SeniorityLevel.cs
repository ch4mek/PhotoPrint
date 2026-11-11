using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Constants;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class SeniorityLevel
{
    public required string Id { get; set; }
    public required string ShortName { get; set; }
    public required string InternalName { get; set; }
    public required string ExternalName { get; set; }
    public required string Description { get; set; }
    public int OrderWeight { get; set; }
    public int Coefficient { get; set; }
    public double SpecializationRangeTo { get; set; }
    public SeniorityLevelScopes SeniorityLevelScope { get; set; }

    public static bool IsSeniorityLevelSigned(string? seniorityLevelId, bool checkForNoneSeniorityLevel)
    {
        if (seniorityLevelId.IsNullOrWhiteSpace())
        {
            return false;
        }

        if (checkForNoneSeniorityLevel && seniorityLevelId.Equals(SeniorityLevelsSigns.None, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return seniorityLevelId.EndsWith(SeniorityLevelsSigns.L1, StringComparison.OrdinalIgnoreCase) ||
            seniorityLevelId.EndsWith(SeniorityLevelsSigns.L2, StringComparison.OrdinalIgnoreCase) ||
            seniorityLevelId.EndsWith(SeniorityLevelsSigns.L3, StringComparison.OrdinalIgnoreCase);
    }
}

internal class SeniorityLevelConfiguration : IEntityTypeConfiguration<SeniorityLevel>
{
    public void Configure(EntityTypeBuilder<SeniorityLevel> builder)
    {
        builder.ToTable("SeniorityLevels");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasMaxLength(64);

        builder.Property(x => x.ShortName).HasMaxLength(8).IsRequired();

        builder.Property(x => x.InternalName).HasMaxLength(64).IsRequired();

        builder.Property(x => x.ExternalName).HasMaxLength(64).IsRequired();

        builder.Property(x => x.Description).HasMaxLength(512).IsRequired();

        builder.Property(x => x.OrderWeight).IsRequired();
        builder.HasIndex(x => x.OrderWeight);

        builder.Property(x => x.Coefficient).IsRequired();
        builder.HasIndex(x => x.Coefficient);

        builder.Property(x => x.SpecializationRangeTo).IsRequired();

        builder.Property(x => x.SeniorityLevelScope).IsRequired().HasConversion<int>();
    }
}
