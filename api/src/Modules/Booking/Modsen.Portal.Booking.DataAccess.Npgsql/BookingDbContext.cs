#pragma warning disable

using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Booking.DataAccess.Npgsql;

internal class BookingDbContext : DbContext, IBookingDbContext
{
    public static readonly string Schema = "booking";

    public DbSet<BookingNotification> BookingNotifications { get; set; }
    public DbSet<BookingUserEntity> BookingUsers { get; set; }

    public DbSet<MeetingRoomEntity> MeetingRooms { get; set; }
    public DbSet<MeetingRoomEntryEntity> MeetingRoomEntries { get; set; }
    public DbSet<MeetingRoomItemEntity> MeetingRoomItems { get; set; }
    public DbSet<MeetingRoomReservationEntity> MeetingRoomReservations { get; set; }

    public DbSet<OfficeEntity> Offices { get; set; }
    public DbSet<Workspace> Workspaces { get; set; }

    public DbSet<Workplace> Workplaces { get; set; }
    public DbSet<BookingEntry> Bookings { get; set; }
    public DbSet<Reservation> Reservations { get; set; }


    public BookingDbContext(DbContextOptions<BookingDbContext> dbContextOptions)
        : base(dbContextOptions) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
    }
}
