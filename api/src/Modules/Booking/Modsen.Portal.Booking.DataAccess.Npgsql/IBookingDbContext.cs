using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.IntegrationTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Booking.Requests")]
namespace Modsen.Portal.Booking.DataAccess.Npgsql;

internal interface IBookingDbContext
{
    public DbSet<BookingNotification> BookingNotifications { get; }
    public DbSet<BookingUserEntity> BookingUsers { get; }

    public DbSet<MeetingRoomEntity> MeetingRooms { get; }
    public DbSet<MeetingRoomEntryEntity> MeetingRoomEntries { get; }
    public DbSet<MeetingRoomItemEntity> MeetingRoomItems { get; }
    public DbSet<MeetingRoomReservationEntity> MeetingRoomReservations { get; }

    public DbSet<OfficeEntity> Offices { get; }
    public DbSet<Workspace> Workspaces { get; }

    public DbSet<Workplace> Workplaces { get; }
    public DbSet<BookingEntry> Bookings { get; }
    public DbSet<Reservation> Reservations  { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
