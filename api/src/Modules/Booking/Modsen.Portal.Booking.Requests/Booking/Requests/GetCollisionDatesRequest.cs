using FluentValidation;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.Queries;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Booking.Requests.Booking.Requests;

internal sealed class GetCollisionDatesRequest : IQueryHandler<GetCollisionDatesRequest.Query, List<DateTime>>
{
    private readonly ICollisionsForDatesGenerator _collisionsForDatesGenerator;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;
    private readonly IGetWorkPlaceByIdDbQuery _getWorkPlaceByIdDbQuery;

    public GetCollisionDatesRequest(
        ICollisionsForDatesGenerator collisionsForDatesGenerator,
        IAuthenticatedUserContext authenticatedUserContext,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor,
        IGetWorkPlaceByIdDbQuery getWorkPlaceByIdDbQuery)
    {
        _collisionsForDatesGenerator = collisionsForDatesGenerator;
        _authenticatedUserContext = authenticatedUserContext;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
        _getWorkPlaceByIdDbQuery = getWorkPlaceByIdDbQuery;
    }

    public async Task<List<DateTime>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRequiredUserId() != request.UserId &&
    !_authenticatedUserContext.IsManager)
        {
            throw new ActionNotAllowedException(PortalMessages.NotManagerCannotCheckCollisionsOfOtherUsers);
        }

        var workplace = await _getWorkPlaceByIdDbQuery.Get(request.WorkplaceId, cancellationToken);

        if (workplace == null)
        {
            throw new EntityNotFoundException(PortalMessages.NotExistingWorkspace);
        }

        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);

        var toDate = request.WeeksInterval > 0
            ? request.ToDate.AddMonths(settings.ReservationsMonthCount)
            : request.ToDate;

        var intervalInDays = request.WeeksInterval > 0 && request.ToDate == request.FromDate
            ? 7 * request.WeeksInterval
            : 1;

        var dates = DatesGenerator.GetDatesRangeTillDate(
            request.FromDate,
            toDate,
            intervalInDays).ToList();

        return await _collisionsForDatesGenerator.GenerateCollisionsForDates(
            request.WeeksInterval,
            request.WorkplaceId,
            request.UserId,
            dates,
            cancellationToken);
    }

    internal sealed record Query(
        DateTime FromDate,
        DateTime ToDate,
        int WorkplaceId,
        int WeeksInterval,
        int UserId)
        : IQuery<List<DateTime>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator(IClockService clock)
        {
            RuleFor(x => x.WorkplaceId)
                .GreaterThan(0);
            RuleFor(x => x.WeeksInterval)
                .InclusiveBetween(0, 4);
            RuleFor(x => x.FromDate.Date)
                .GreaterThanOrEqualTo(clock.UtcNow.Date)
                .WithMessage("FromDate should be greater than or equal to current date");
            RuleFor(x => x.FromDate.TimeOfDay)
                .Equal(TimeSpan.Zero);
            RuleFor(x => x.ToDate.Date)
                .GreaterThanOrEqualTo(clock.UtcNow.Date)
                .WithMessage("EndDate should be greater than or equal to current date");
            RuleFor(x => x.ToDate.Date)
                .LessThanOrEqualTo(clock.UtcNow.Date.AddMonths(3));
            RuleFor(x => x.ToDate.TimeOfDay)
                .Equal(TimeSpan.Zero);
            RuleFor(x => x)
                .Must(x => x.FromDate <= x.ToDate)
                .WithMessage("FromDate should be less than EndDate!");
        }
    }
}
