using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.Core.CoreDto;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Booking.Requests.Core.DbQueries;

internal interface ICreateOrUpdateUserDbQuery
{
    Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken);
}

internal class CreateOrUpdateUserDbQuery : ICreateOrUpdateUserDbQuery
{
    private readonly IBookingDbContext _bookingDbContext;
    private readonly IClockService _clockService;

    public CreateOrUpdateUserDbQuery(
        IBookingDbContext bookingDbContext,
        IClockService clockService)
    {
        _bookingDbContext = bookingDbContext;
        _clockService = clockService;
    }

    public async Task Execute(CreateOrUpdateUserCoreDto request, CancellationToken cancellationToken)
    {
        if (request.Position.IsIntern())
        {
            return;
        }

        var user = await _bookingDbContext.BookingUsers
           .Where(x => x.UserId == request.UserId)
           .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            user = new BookingUserEntity
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CreatedAtUtc = _clockService.UtcNow,
                CreatedById = null,
                IsActive = true,
            };

            _bookingDbContext.BookingUsers.Add(user);
        }
        else
        {
            user.UpdatedById = request.InitiatorId;
            user.UpdatedAtUtc = _clockService.UtcNow;
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.FirstNameRu = request.FirstNameRu;
        user.LastNameRu = request.LastNameRu;
        user.Email = request.Email;
        user.DepartmentId = request.DepartmentId;
        user.ResourceGroupId = request.ResourceGroupId;

        await _bookingDbContext.SaveChangesAsync(cancellationToken);
    }
}
