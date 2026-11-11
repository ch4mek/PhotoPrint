using Modsen.Portal.Booking.Requests.Workplaces.Requests;

namespace Modsen.Portal.Booking.UnitTests.Workplaces.GetReservationRequest;

public class GetReservationRequestValidatorTests
{
    private readonly GetReservationsRequest.Validator _validator = new();

    [Theory]
    [InlineData("2023-02-01T12:17:24Z", "2023-01-01")]
    [InlineData("2023-01-01", "2023-02-01T12:17:24Z")]
    public void ValidateWhenDataIsInvalidShouldReturnInvalidResult(DateTime fromDate, DateTime toDate)
    {
        // arrange
        var query = new GetReservationsRequest.Query(
            FromDate : fromDate,
            ToDate : toDate,
            OfficeId: null,
            WorkspaceId: null
        );

        // act
        var result = _validator.Validate(query);

        // assert
        result.IsValid.Should().BeFalse();
    }
}
