using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Booking.DataAccess.Npgsql;

internal class DesignTimeBookingDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        return new BookingDbContext(new DbContextOptionsBuilder<BookingDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
