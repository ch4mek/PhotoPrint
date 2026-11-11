using Modsen.Portal.Booking.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.Booking.Contracts;

internal class CreateBlockTestInput : CreateBookingBaseInput
{
    public CreateBlockTestInput()
    {
        RequestType = CreateBlock;
    }
}
