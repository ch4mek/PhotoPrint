namespace Modsen.Portal.Booking.Contract;

public interface IBookingContract
{
    Task<bool> CanBookingUserBeDeactivated(int userId, CancellationToken cancellationToken);
    Task<OfficeClassificationDto[]> GetOfficesClassificator(CancellationToken cancellationToken);
}
