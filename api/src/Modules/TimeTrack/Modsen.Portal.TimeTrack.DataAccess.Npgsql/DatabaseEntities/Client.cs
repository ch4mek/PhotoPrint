using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class Client : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntity { get; set; }
    public string? Color { get; set; }
    public string? Comments { get; set; }
    public bool IsActive { get; set; } = true;
    public string? RegisteredLocation { get; set; }
    public int? SalesManagerId { get; set; }
    public TimeTrackUser? SalesManagerUser { get; set; }

    [NotMapped]
    public int FactTrackedTime { get; set; }

    [NotMapped]
    public int BillableTrackedTime { get; set; }

    public List<Project> Projects { get; set; } = [];
    public TimeTrackUser? CreatedByUser { get; set; }
    public TimeTrackUser? UpdatedByUser { get; set; }

    public static Client Of(Domain.Client source)
    {
        return new Client()
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            Comments = source.Comments,
            IsActive = source.IsActive,
            RegisteredLocation = source.RegisteredLocation,
            FactTrackedTime = source.FactTrackedTimeInMinutes,
            BillableTrackedTime = source.BillableTrackedTimeInMinutes,
            SalesManagerId = source.SalesManagerId
        };
    }

    public static Domain.Client? ToDomain(Client? source)
    {
        return source == null
            ? null
            : new Domain.Client()
            {
                Id = source.Id,
                Name = source.Name,
                Color = source.Color,
                Comments = source.Comments,
                IsActive = source.IsActive,
                RegisteredLocation = source.RegisteredLocation,
                FactTrackedTimeInMinutes = source.Projects
                    .Where(p => p.IsActive)
                    .SelectMany(p => p.Timesheets)
                    .Sum(t => t.FactTrackedInMinutes),
                BillableTrackedTimeInMinutes = source.Projects
                    .Where(p => p.IsActive)
                    .SelectMany(p => p.Timesheets)
                    .Sum(t => t.BillableTrackedInMinutes),
                Projects = source.Projects.Select(x => x.Id).ToList(),
                IsProjectExist = source.Projects.Count != 0,
                SalesManagerId = source.SalesManagerId
            };
    }

    public static readonly Expression<Func<Client, OdpClient>> MapClient =
        x => new OdpClient
        {
            Id = x.Id,
            Name = x.Name,
            LegalEntity = x.LegalEntity,
            Color = x.Color,
            Comments = x.Comments,
            IsActive = x.IsActive,
            RegisteredLocation = x.RegisteredLocation,
            FactTrackedTimeInMinutes = x.Projects
                .Where(p => p.IsActive)
                .SelectMany(p => p.Timesheets)
                .Sum(t => t.FactTrackedInMinutes),
            BillableTrackedTimeInMinutes = x.Projects
                .Where(p => p.IsActive)
                .SelectMany(p => p.Timesheets)
                .Sum(t => t.BillableTrackedInMinutes),
            IsProjectExist = x.Projects.Any(),
            HasActiveProject = x.Projects.Any(p => p.IsActive),
            SalesManagerId = x.SalesManagerId
        };

    internal class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public virtual void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients", TimeTrackDbContext.Schema);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(x => x.Name);

            builder.Property(x => x.LegalEntity).IsRequired(false).HasMaxLength(250);

            builder.Property(x => x.Comments).IsRequired(false).HasMaxLength(8192);
            builder.Property(x => x.IsActive).IsRequired();

            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
            builder.Property(x => x.UpdatedById).IsRequired(false);

            builder.Property(x => x.RegisteredLocation).IsRequired();

            builder.Property(x => x.SalesManagerId).IsRequired(false);

            builder.HasOne(x => x.SalesManagerUser)
                .WithMany()
                .HasForeignKey(x => x.SalesManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
