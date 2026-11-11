using Modsen.Portal.Booking.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.Booking.Contracts;

internal class CreateUserBookingTestInput : CreateBookingBaseInput
{
    public CreateUserBookingTestInput()
    {
        RequestType = CreateForUser;
    }
}
