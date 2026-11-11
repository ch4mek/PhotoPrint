using FluentValidation;
using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.Queries;
using Modsen.Portal.Booking.Requests.Services.Contracts;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.MeetingRooms.Requests;

internal sealed class GetMeetingRoomCollisionsRequest : IQueryHandler<GetMeetingRoomCollisionsRequest.Query, MeetingRoomReservationCoreDto[]>
{
    private readonly IMeetingRoomReservationDateGenerator _reservationDateGenerator;
    private readonly IMeetingRoomAccessBusinessRule _meetingRoomAccessManager;
    private readonly IMeetingRoomReservationConflictResolver _iMeetingRoomReservationConflictResolver;
    private readonly IGetMeetingRoomByIdDbQuery _getMeetingRoomByIdDbQuery;
    private readonly IMeetingRoomReservationLoader _meetingRoomReservationLoader;
    private readonly IMeetingRoomReservationDateValidator _reservationDateValidator;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;

    public GetMeetingRoomCollisionsRequest(
        IMeetingRoomReservationDateGenerator reservationDateGenerator,
        IMeetingRoomAccessBusinessRule meetingRoomAccessManager,
        IMeetingRoomReservationConflictResolver iMeetingRoomReservationConflictResolver,
        IGetMeetingRoomByIdDbQuery getMeetingRoomByIdDbQuery,
        IMeetingRoomReservationLoader meetingRoomReservationLoader,
        IMeetingRoomReservationDateValidator reservationDateValidator,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor)
    {
        _reservationDateGenerator = reservationDateGenerator;
        _meetingRoomAccessManager = meetingRoomAccessManager;
        _iMeetingRoomReservationConflictResolver = iMeetingRoomReservationConflictResolver;
        _getMeetingRoomByIdDbQuery = getMeetingRoomByIdDbQuery;
        _meetingRoomReservationLoader = meetingRoomReservationLoader;
        _reservationDateValidator = reservationDateValidator;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
    }

    public async Task<MeetingRoomReservationCoreDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        await _meetingRoomAccessManager.VerifyCurrentUserCanSeeCollisionForUserId(request.UserId, cancellationToken);
        var meetingRoom = await _getMeetingRoomByIdDbQuery.Get(request.MeetingRoomId, cancellationToken);
        var timeZone = meetingRoom.Office?.TimeZone;

        ArgumentException.ThrowIfNullOrEmpty(timeZone);
        if (!meetingRoom.IsActive)
        {
            throw new AppException(PortalMessages.CannotGetCollisionsForInactiveMeetingRoom);
        }
        _reservationDateValidator.Validate(request.ReservationFromUtc, request.ReservationToUtc, timeZone);

        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var reservationDates = _reservationDateGenerator
            .GenerateReservationDates(new MeetingRoomReservationDateGeneratorRequest
            {
                OfficeTimeZone = timeZone,
                ReservationFromUtc = request.ReservationFromUtc,
                ReservationToUtc = request.ReservationToUtc,
                ReservationPeriodToOfficeDate = request.ReservationPeriodToOfficeDate,
                ReservationPeriodFromOfficeDate = request.ReservationPeriodFromOfficeDate,
                RepeatReservationWeeks = request.RepeatReservationWeeks,
                ReservationsMonthCount = settings.ReservationsMonthCount
            })
            .ToList();

        return await GetCollisions(reservationDates, request, cancellationToken);
    }

    private async Task<MeetingRoomReservationCoreDto[]> GetCollisions(List<DateRangeCoreDto> reservationDates,
        Query request, CancellationToken cancellationToken)
    {
        var reservationIds = await _iMeetingRoomReservationConflictResolver.ResolveConflictsFor(
            reservationDates, request.MeetingRoomId, request.UserId, cancellationToken);
        if (reservationIds.Length == 0)
        {
            return [];
        }

        return await _meetingRoomReservationLoader.Load(reservationIds, cancellationToken);
    }

    internal sealed record Query : IQuery<MeetingRoomReservationCoreDto[]>, IBookingRequest
    {
        public DateTime ReservationFromUtc { get; set; }
        public DateTime ReservationToUtc { get; set; }
        public DateOnly ReservationPeriodFromOfficeDate { get; set; }
        public DateOnly ReservationPeriodToOfficeDate { get; set; }
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public int? RepeatReservationWeeks { get; set; }
    }

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator(IClockService clock, IBookingClockService bookingClockService,
            ITimeZoneConverter officeTimeManager)
        {
            RuleFor(x => x.ReservationPeriodFromOfficeDate)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(x => clock.TodayUtc())
                .WithMessage("The booking date cannot be in the past.");

            RuleFor(x => x.ReservationPeriodToOfficeDate)
                .Equal(x => x.ReservationPeriodFromOfficeDate)
                .When(x => x.RepeatReservationWeeks > 0)
                .WithMessage("From and to dates must be equal");

            RuleFor(x => x.ReservationFromUtc)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(x => bookingClockService.RoundUpToHalfAnHourWithBufferSeconds(clock.UtcNow))
                .When(x => !officeTimeManager.IsAllDay(x.ReservationFromUtc, x.ReservationToUtc))
                .WithMessage("The start time cannot be earlier than the current time.");

            RuleFor(x => x.ReservationToUtc)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(x => bookingClockService.RoundDownToMinute(clock.UtcNow))
                .WithMessage("The end time cannot be earlier than the current time.");
        }
    }
}
