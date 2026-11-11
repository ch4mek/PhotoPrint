using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests.Workplaces.Core.Loaders;
using Modsen.Portal.Booking.Requests.Workplaces.Requests.RequestDto;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.Workplaces.Requests;

internal sealed class GetReservationsRequest : IQueryHandler<GetReservationsRequest.Query, List<ReservationDto>>
{
    private readonly IReservationsLoader _reservationsLoader;
    private readonly IBookingDbContext _bookingDbContext;

    public GetReservationsRequest(IBookingDbContext bookingDbContext, IReservationsLoader reservationsLoader)
    {
        _bookingDbContext = bookingDbContext;
        _reservationsLoader = reservationsLoader;
    }

    public async Task<List<ReservationDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var reservationIds = await GetReservationIds(request, cancellationToken);

        var result = await _reservationsLoader.LoadAsync(reservationIds, cancellationToken);

        return result;
    }

    private Task<int[]> GetReservationIds(Query request, CancellationToken cancellationToken)
    {
        return _bookingDbContext.Reservations
            .Where(x => request.OfficeId == null || x.BookingEntry!.Workplace!.OfficeId == request.OfficeId)
            .Where(x => request.WorkspaceId == null || x.BookingEntry!.Workplace!.WorkspaceId == request.WorkspaceId)
            .Where(reservation =>
                reservation.ReservationDate.Date >= request.FromDate.Date &&
                reservation.ReservationDate.Date <= request.ToDate.Date)
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .Distinct()
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Query(
        DateTime FromDate,
        DateTime ToDate,
        int? OfficeId,
        int? WorkspaceId) : IQuery<List<ReservationDto>>, IBookingRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.FromDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage("FromDate should be greater than or equal to current date");

            RuleFor(x => x.ToDate.TimeOfDay)
                .Equal(TimeSpan.Zero)
                .WithMessage("EndDate should be greater than or equal to current date");

            RuleFor(x => x)
                .Must(x => x.FromDate <= x.ToDate)
                .WithMessage("FromDate should be less than EndDate!");
        }
    }
}
