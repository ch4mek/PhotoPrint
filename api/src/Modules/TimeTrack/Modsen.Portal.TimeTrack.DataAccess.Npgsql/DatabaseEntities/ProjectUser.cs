using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

internal class ProjectUser : AuditableEntity
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }

    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }
    public bool IsTimeOnProjectBillable { get; set; }

    public decimal Fte { get; set; }
    public int? RefUserId { get; set; }
    public string? Comments { get; set; }

    public TimeTrackUser? RefUser { get; set; }
    public Project? Project { get; set; }
    public TimeTrackUser? User { get; set; }
    public TimeTrackUser? CreatedByUser { get; set; }
    public TimeTrackUser? UpdatedByUser { get; set; }

    public static Domain.ProjectUser? ToDomain(ProjectUser? source)
    {
        return source == null
            ? null
            : new Domain.ProjectUser
            {
                Id = source.UserId,
                FirstName = source.User?.FirstName,
                LastName = source.User?.LastName,
                DepartmentId = source.User?.DepartmentId,
                ProjectId = source.ProjectId,
                ProjectName = source.Project?.Name,
                ClientName = source.Project?.Client?.Name,
                UserOnProjectFrom = source.UserOnProjectFrom,
                UserOnProjectTo = source.UserOnProjectTo,
                IsTimeOnProjectBillable = source.IsTimeOnProjectBillable,
                UserId = source.UserId,
                Fte = source.Fte,
            };
    }

    public static ProjectUser Of(ProjectUserToRepository source)
    {
        return new ProjectUser()
        {
            Id = source.UserId,
            ProjectId = source.ProjectId,
        };
    }

    public static readonly Expression<Func<ProjectUser, OdpProjectUser>> MapProjectUser =
        x => new OdpProjectUser
        {
            Id = x.Id,
            UserId = x.UserId,
            ProjectId = x.ProjectId,
            FirstName = x.User!.FirstName,
            LastName = x.User.LastName,
            DepartmentId = x.User.DepartmentId,
            ProjectName = x.Project!.Name,
            UserFullName = x.User.FirstName + ' ' + x.User.LastName,
            ResourceGroupId = x.User.ResourceGroupId,
            IsTimeOnProjectBillable = x.IsTimeOnProjectBillable,
            ClientName = x.Project!.Client!.Name,
            UserOnProjectFrom = x.UserOnProjectFrom,
            UserOnProjectTo = x.UserOnProjectTo,
            IsActive = x.UserOnProjectTo == null
                || (x.UserOnProjectFrom <= DateOnly.FromDateTime(DateTime.UtcNow)
                    && DateOnly.FromDateTime(DateTime.UtcNow) <= x.UserOnProjectTo)
                || DateOnly.FromDateTime(DateTime.UtcNow) <= x.UserOnProjectFrom
        };


    internal class ProjectUserConfiguration : IEntityTypeConfiguration<ProjectUser>
    {
        public void Configure(EntityTypeBuilder<ProjectUser> builder)
        {
            builder.ToTable("ProjectUsers", TimeTrackDbContext.Schema);

            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.Property(x => x.CreatedById).IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
            builder.Property(x => x.UpdatedById).IsRequired(false);

            builder.Property(x => x.UserOnProjectFrom).IsRequired();
            builder.Property(x => x.UserOnProjectTo).IsRequired(false);
            builder.Property(x => x.IsTimeOnProjectBillable).IsRequired();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProjectId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.Fte).HasPrecision(18, 4);

            builder.HasOne(x => x.RefUser)
                .WithMany()
                .HasForeignKey(x => x.RefUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Comments).IsRequired(false).HasMaxLength(512);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.ProjectUsers)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.UserId)
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
