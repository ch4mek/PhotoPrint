using FluentValidation;
using Modsen.Portal.Booking.Requests.Booking.Core;
using Modsen.Portal.Booking.Requests.Booking.Core.BusinessRules;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.Booking.Requests.Booking.Requests;

internal sealed class CreateSelfBookingRequest : ICommandHandler<CreateSelfBookingRequest.Command, BookingEntryCoreDto>
{
    private readonly IBookingEntryComposer _bookingComposer;
    private readonly IBookingEntryLoader _bookingEntryLoader;
    private readonly IIsAllowedToBookBusinessRule _isAllowedToBookBusinessRule;

    public CreateSelfBookingRequest(
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
            IsForGuest = false
        }, cancellationToken);

        return (await _bookingEntryLoader.Load(bookingEntity.Id, cancellationToken))!;
    }

    internal sealed record Command : ICommand<BookingEntryCoreDto>, IBookingRequest, ITransactionalRequest
    {
        public int WorkplaceId { get; init; }
        public DateTime FromDate { get; init; }
        public DateTime ToDate { get; init; }
        public int WeeksInInterval { get; init; }
    }

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator(IClockService clock)
        {
            RuleFor(x => x.WorkplaceId).GreaterThan(0);
            RuleFor(x => x.WeeksInInterval).InclusiveBetween(0, 4);
            RuleFor(x => x.FromDate.Date).GreaterThanOrEqualTo(clock.UtcNow.Date).WithMessage("FromDate should be greater than or equal to current date");
            RuleFor(x => x.FromDate.TimeOfDay).Equal(TimeSpan.Zero);
            RuleFor(x => x.ToDate.Date).GreaterThanOrEqualTo(clock.UtcNow.Date).WithMessage("EndDate should be greater than or equal to current date");
            RuleFor(x => x.ToDate.Date).LessThanOrEqualTo(clock.UtcNow.Date.AddMonths(3));
            RuleFor(x => x.ToDate.TimeOfDay).Equal(TimeSpan.Zero);
            RuleFor(x => x).Must(x => x.FromDate <= x.ToDate).WithMessage("FromDate should be less than EndDate!");
        }
    }
}
