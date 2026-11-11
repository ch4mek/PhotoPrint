using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToBookingMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly BookingDbContext _bookingDbContext;

    public PublishExistingUsersToBookingMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        BookingDbContext bookingDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _bookingDbContext = bookingDbContext;
    }

    public string TaskName => "PublishExistingUsersToBooking";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyUsers;

        var bookingUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var bookingUsers = new List<BookingUserEntity>();

            page++;

            foreach (var user in users)
            {
                var bookingUser = _bookingDbContext.BookingUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (bookingUser == null)
                {
                    bookingUser = new BookingUserEntity
                    {
                        UserId = user.Id,
                        FirstName = "test",
                        LastName = "test",
                        Email = "test@modsen-software.com"
                    };

                    CopyFields(bookingUser, user);
                    _bookingDbContext.Add(bookingUser);
                }
                else
                {
                    CopyFields(bookingUser, user);
                }

                bookingUsers.Add(bookingUser);
                bookingUserIds.Add(bookingUser.UserId);
            }

            _bookingDbContext.SaveChanges();

            foreach (var bookingUser in bookingUsers)
            {
                _bookingDbContext.BookingUsers.Entry(bookingUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();
        } while (hasAnyUsers);

        _bookingDbContext.BookingUsers
            .Where(bookingUser => !bookingUserIds.Contains(bookingUser.UserId))
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls.SetProperty(
                bookingUser => bookingUser.IsActive, false));

        _bookingDbContext.SaveChanges();
    }

    private static void CopyFields(BookingUserEntity bookingUser, User user)
    {
        bookingUser.Email = user.Email!;
        bookingUser.FirstName = user.FirstName;
        bookingUser.LastName = user.LastName;
        bookingUser.FirstNameRu = user.FirstNameRu;
        bookingUser.LastNameRu = user.LastNameRu;
        bookingUser.DepartmentId = user.DepartmentId;
        bookingUser.IsActive = user.IsActive;
        bookingUser.ResourceGroupId = user.ResourceGroupId;
    }
}
