using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class TeamBoardClient
{
    public int ClientId { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;

    public List<ProjectExit> ProjectExits { get; set; } = [];

    internal class ClientConfiguration : IEntityTypeConfiguration<TeamBoardClient>
    {
        public virtual void Configure(EntityTypeBuilder<TeamBoardClient> builder)
        {
            builder.ToTable("TeamBoardClients", TeamBoardDbContext.Schema);

            builder.HasKey(x => x.ClientId);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(x => x.Name);

            builder.Property(x => x.IsActive).IsRequired();
        }
    }
}
