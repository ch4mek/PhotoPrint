using Modsen.Portal.Booking.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.Booking.Contracts;

internal class CreateSelfBookingTestInput : CreateBookingBaseInput
{
    public CreateSelfBookingTestInput()
    {
        RequestType = CreateSelf;
    }
}
