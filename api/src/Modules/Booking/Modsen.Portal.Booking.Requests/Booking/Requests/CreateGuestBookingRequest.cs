using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.Booking.Requests;

internal sealed class CreateGuestBookingRequest : ICommandHandler<CreateGuestBookingRequest.Command, BookingEntryCoreDto>
{
    private readonly IBookingEntryComposer _bookingComposer;
    private readonly IBookingEntryLoader _bookingEntryLoader;
    private readonly IIsAllowedToBookBusinessRule _isAllowedToBookBusinessRule;

    public CreateGuestBookingRequest(
        IBookingEntryComposer bookingComposer,
        IBookingEntryLoader bookingEntryLoader,
        IIsAllowedToBookBusinessRule isAllowedToBookBusinessRule)
    {
        _bookingComposer = bookingComposer;
        _bookingEntryLoader = bookingEntryLoader;
        _isAllowedToBookBusinessRule = isAllowedToBookBusinessRule;
    }

    public async Task<BookingEntryCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (! await _isAllowedToBookBusinessRule.IsCorrespondsToBusiness(request.WorkplaceId, cancellationToken))
        {
            throw new ActionNotAllowedException(PortalMessages.DifferentDepartments);
        }

        var bookingEntity = await _bookingComposer.Compose(new BookingEntryComposerRequest
        {
            WorkplaceId = request.WorkplaceId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            WeeksInInterval = request.WeeksInInterval,
            IsForGuest = true,
            GuestName = request.GuestName,
        }, cancellationToken);

        return (await _bookingEntryLoader.Load(bookingEntity.Id, cancellationToken))!;
    }

    internal sealed record Command : ICommand<BookingEntryCoreDto>, IBookingRequest, ITransactionalRequest
    {
        public string? GuestName { get; init; }
        public int WorkplaceId { get; init; }
        public DateTime FromDate { get; init; }
        public DateTime ToDate { get; init; }
        public int WeeksInInterval { get; init; }
    }
}
