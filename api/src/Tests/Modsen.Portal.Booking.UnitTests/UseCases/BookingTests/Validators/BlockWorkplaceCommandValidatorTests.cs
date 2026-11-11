using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Core.UseCases;

namespace Modsen.Portal.Booking.UnitTests.UseCases.BookingTests.Validators;

public class BlockWorkplaceCommandValidatorTests
{
    [Theory]
    [InlineData("2023-06-15")]
    [InlineData("2007-01-05")]
    [InlineData("2053-12-30T12:17:24Z")]
    [InlineData("2045-07-18T00:00:01Z")]
    public void ValidateWhenDataIsInvalidShouldReturnInvalidResult(DateTime startDate)
    {
        // Arrange
        var validator = new BlockWorkplaceRequest.Validator(new ClockService());

        var request = new BlockWorkplaceRequest.Command
        {
            StartDate = startDate,
            WorkplaceId = 1
        };

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }
}
