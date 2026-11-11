using MediatR;
using Modsen.Portal.Booking.Contract;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;

namespace Modsen.Portal.Booking.Requests;

internal class BookingContract : IBookingContract
{
    private readonly ICanBookingUserBeDeactivatedBusinessRule _canBookingUserBeDeactivatedBusinessRule;
    private readonly ISender _sender;

    public BookingContract(
        ICanBookingUserBeDeactivatedBusinessRule canBookingUserBeDeactivatedBusinessRule,
        ISender sender)
    {
        _canBookingUserBeDeactivatedBusinessRule = canBookingUserBeDeactivatedBusinessRule;
        _sender = sender;
    }

    public Task<bool> CanBookingUserBeDeactivated(int userId, CancellationToken cancellationToken)
    {
        var hasNoBookings = _canBookingUserBeDeactivatedBusinessRule.IsCorrespondsToBusiness(userId, cancellationToken);

        return hasNoBookings;
    }

    public Task<OfficeClassificationDto[]> GetOfficesClassificator(CancellationToken cancellationToken)
    {
        return _sender.Send(new GetOfficesClassificatorRequest.Query(), cancellationToken);
    }
}
