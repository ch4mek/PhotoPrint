using FluentValidation;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
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

internal sealed class BookMeetingRoomRequest : ICommandHandler<BookMeetingRoomRequest.Command, MeetingRoomEntryCoreDto>
{
    private readonly IMeetingRoomEntryComposer _meetingRoomEntryComposer;
    private readonly IMeetingRoomAccessBusinessRule _meetingRoomAccessManager;
    private readonly IMeetingRoomReservationConflictResolver _meetingRoomReservationConflictResolver;
    private readonly IGetMeetingRoomByIdDbQuery _getMeetingRoomByIdDbQuery;
    private readonly IMeetingRoomEntryLoader _meetingRoomEntryLoader;
    private readonly IMeetingRoomReservationDateGenerator _reservationDateGenerator;
    private readonly IMeetingRoomReservationDateValidator _reservationDateValidator;
    private readonly IBookingPortalSettingsAccessor _bookingPortalSettingsAccessor;

    public BookMeetingRoomRequest(
        IMeetingRoomEntryComposer meetingRoomEntryComposer,
        IMeetingRoomAccessBusinessRule meetingRoomAccessManager,
        IMeetingRoomReservationConflictResolver meetingRoomReservationConflictResolver,
        IGetMeetingRoomByIdDbQuery getMeetingRoomByIdDbQuery,
        IMeetingRoomEntryLoader meetingRoomEntryLoader,
        IMeetingRoomReservationDateGenerator reservationDateGenerator,
        IMeetingRoomReservationDateValidator reservationDateValidator,
        IBookingPortalSettingsAccessor bookingPortalSettingsAccessor)
    {
        _meetingRoomEntryComposer = meetingRoomEntryComposer;
        _meetingRoomAccessManager = meetingRoomAccessManager;
        _meetingRoomReservationConflictResolver = meetingRoomReservationConflictResolver;
        _getMeetingRoomByIdDbQuery = getMeetingRoomByIdDbQuery;
        _meetingRoomEntryLoader = meetingRoomEntryLoader;
        _reservationDateGenerator = reservationDateGenerator;
        _reservationDateValidator = reservationDateValidator;
        _bookingPortalSettingsAccessor = bookingPortalSettingsAccessor;
    }

    public async Task<MeetingRoomEntryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await _meetingRoomAccessManager.VerifyCurrentUserCanBookForUserId(request.UserId, cancellationToken);

        var meetingRoom = await _getMeetingRoomByIdDbQuery.Get(request.MeetingRoomId, cancellationToken);
        var timeZone = meetingRoom.Office?.TimeZone;

        ArgumentException.ThrowIfNullOrEmpty(timeZone);
        if (!meetingRoom.IsActive)
        {
            throw new AppException(PortalMessages.CannotBookInactiveMeetingRoom);
        }

        _reservationDateValidator.Validate(request.ReservationFromUtc, request.ReservationToUtc, timeZone);

        var settings = await _bookingPortalSettingsAccessor.GetSettings(cancellationToken);
        var reservationDates = GenerateReservationDates(request, timeZone, settings.ReservationsMonthCount);
        var conflicts = await _meetingRoomReservationConflictResolver.ResolveConflictsFor(
            reservationDates, meetingRoom.Id, request.UserId, cancellationToken);

        if (conflicts.Length > 0)
        {
            throw new AppException(PortalMessages.ReservationConflicts);
        }

        var meetingRoomEntry = await BuildMeetingRoomEntry(request, reservationDates, cancellationToken);

        return await GetMeetingRoomEntry(meetingRoomEntry.Id, cancellationToken);
    }

    private DateRangeCoreDto[] GenerateReservationDates(Command request, string timeZone, int reservationsMonthCount)
    {
        return _reservationDateGenerator
            .GenerateReservationDates(new MeetingRoomReservationDateGeneratorRequest
            {
                OfficeTimeZone = timeZone,
                ReservationFromUtc = request.ReservationFromUtc,
                ReservationToUtc = request.ReservationToUtc,
                ReservationPeriodToOfficeDate = request.ReservationPeriodToOfficeDate,
                ReservationPeriodFromOfficeDate = request.ReservationPeriodFromOfficeDate,
                RepeatReservationWeeks = request.RepeatReservationWeeks,
                ReservationsMonthCount = reservationsMonthCount
            })
            .ToArray();
    }

    private Task<MeetingRoomEntryEntity> BuildMeetingRoomEntry(
        Command request,
        IReadOnlyCollection<DateRangeCoreDto> reservationDates,
        CancellationToken token)
    {
        return _meetingRoomEntryComposer.Compose(
            new MeetingRoomEntryComposerRequest
            {
                UserId = request.UserId,
                MeetingRoomId = request.MeetingRoomId,
                RepeatReservationWeeks = request.RepeatReservationWeeks,
                ReservationPeriodFromOfficeDate = request.ReservationPeriodFromOfficeDate,
                ReservationPeriodToOfficeDate = request.ReservationPeriodToOfficeDate,
                ReservationFromUtc = request.ReservationFromUtc,
                ReservationToUtc = request.ReservationToUtc,
                ReservationDates = reservationDates
            }, token);
    }

    private async Task<MeetingRoomEntryCoreDto> GetMeetingRoomEntry(int meetingRoomEntryId, CancellationToken token)
    {
        var entry = await _meetingRoomEntryLoader.Load(meetingRoomEntryId, token);
        return entry ?? throw new EntityNotFoundException(PortalMessages.NotExistingMeetingRoomEntry);
    }

    internal sealed record Command : ICommand<MeetingRoomEntryCoreDto>, IBookingRequest, ITransactionalRequest
    {
        public DateTime ReservationFromUtc { get; set; }
        public DateTime ReservationToUtc { get; set; }
        public DateOnly ReservationPeriodFromOfficeDate { get; set; }
        public DateOnly ReservationPeriodToOfficeDate { get; set; }
        public int UserId { get; set; }
        public int MeetingRoomId { get; set; }
        public int? RepeatReservationWeeks { get; set; }
    }

    internal sealed class Validator : AbstractValidator<Command>
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
