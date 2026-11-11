using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class Project : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public bool IsPublic { get; set; }
    public bool IsBillable { get; set; }
    public bool IsActive { get; set; }
    public string? TechStack { get; set; }
    public string? Industry { get; set; }
    public string? Architecture { get; set; }
    public string? CloudServices { get; set; }

    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public int? ManagerUserId { get; set; }
    public TimeTrackUser? ManagerUser { get; set; }

    [NotMapped]
    public int FactTrackedTime { get; set; }

    [NotMapped]
    public int BillableTrackedTime { get; set; }

    public List<ProjectUser> ProjectUsers { get; set; } = [];
    public List<Timesheet> Timesheets { get; set; } = [];
    public TimeTrackUser? CreatedByUser { get; set; }
    public TimeTrackUser? UpdatedByUser { get; set; }

    public static Project Of(Domain.Project source)
    {
        return new Project
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            TechStack = source.TechStack,
            Industry = source.Industry,
            Architecture = source.Architecture,
            CloudServices = source.CloudServices,
            ClientId = source.ClientId,
            ManagerUserId = source.ManagerId,
            FactTrackedTime = source.FactTrackedTimeInMinutes ?? 0,
            BillableTrackedTime = source.BillableTrackedTimeInMinutes ?? 0,
            ProjectUsers = source.ProjectUsers
                .Select(u => new ProjectUser(){Id = u})
                .ToList(),
        };
    }

    public static Domain.Project? ToDomain(Project? source)
    {
        return source == null
               ? null
               : new Domain.Project
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            TechStack = source.TechStack,
            Industry = source.Industry,
            Architecture = source.Architecture,
            CloudServices = source.CloudServices,
            ClientId = source.ClientId,
            ManagerId = source.ManagerUserId,
            FactTrackedTimeInMinutes = source.FactTrackedTime,
            BillableTrackedTimeInMinutes = source.BillableTrackedTime,
            CreatedById = source.CreatedById,
            ProjectUsers = source.ProjectUsers.Select(x => x.UserId).ToList(),
        };
    }

    public static readonly Expression<Func<Project, Domain.Project>> Map =
        project => new Domain.Project
        {
            Id = project.Id,
            Name = project.Name,
            Color = project.Color,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable,
            IsActive = project.IsActive,
            ClientId = project.ClientId,
            ManagerId = project.ManagerUserId,
            CreatedById = project.CreatedById,
            FactTrackedTimeInMinutes = project.Timesheets.Sum(timesheet => timesheet.FactTrackedInMinutes),
            BillableTrackedTimeInMinutes = project.Timesheets.Sum(timesheet => timesheet.BillableTrackedInMinutes),
            ProjectUsers = project.ProjectUsers.Select(user => user.UserId).ToList(),
            TechStack = project.TechStack,
            Architecture = project.Architecture,
            CloudServices = project.CloudServices,
            Industry = project.Industry
        };

    public static readonly Expression<Func<Project, OdpProject>> MapOdpProject =
        project => new OdpProject
        {
            Id = project.Id,
            Name = project.Name,
            Color = project.Color,
            IsPublic = project.IsPublic,
            IsBillable = project.IsBillable,
            IsActive = project.IsActive,
            ClientId = project.ClientId,
            ClientName = project.Client!.Name,
            FactTrackedTime = project.Timesheets.Sum(timesheet => timesheet.FactTrackedInMinutes),
            BillableTrackedTime = project.Timesheets.Sum(timesheet => timesheet.BillableTrackedInMinutes),
            ProjectUsers = project.ProjectUsers
                .AsQueryable()
                .Select(ProjectUser.MapProjectUser)
                .ToList()
        };

    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public virtual void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects", TimeTrackDbContext.Schema);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.HasIndex(x => x.Name);

            builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength();

            builder.Property(x => x.IsPublic).IsRequired();
            builder.Property(x => x.IsBillable).IsRequired();

            builder.Property(x => x.IsActive).IsRequired();
            builder.HasIndex(x => x.IsActive);

            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
            builder.Property(x => x.UpdatedById).IsRequired(false);

            builder.Property(x => x.TechStack).IsRequired(false).HasMaxLength(512);

            builder.Property(x => x.Industry).IsRequired(false).HasMaxLength(64);

            builder.Property(x => x.Architecture).IsRequired(false).HasMaxLength(256);

            builder.Property(x => x.CloudServices).IsRequired(false).HasMaxLength(512);

            builder.HasOne(x => x.Client)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.ClientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ManagerUser)
                .WithMany()
                .HasForeignKey(x => x.ManagerUserId)
                .IsRequired(required: false)
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
