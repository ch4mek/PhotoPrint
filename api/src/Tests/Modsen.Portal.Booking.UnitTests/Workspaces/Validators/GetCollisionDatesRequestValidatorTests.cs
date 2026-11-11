using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Core.Services;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workspaces.Validators;

public class GetCollisionDatesRequestValidatorTests
{
    [Theory]
    [InlineData("2023-01-01", "2023-02-01", -1, 0)]
    [InlineData("2023-01-01", "2023-01-07", 1, -1)]
    [InlineData("2022-01-01", "2023-01-01", 1, 0)]
    [InlineData("2023-02-01T12:17:24Z", "2023-01-01", 1, 1)]
    [InlineData("2023-01-01", "2022-01-01", 1, 1)]
    [InlineData("2023-01-01", "2022-05-01", 1, 1)]
    [InlineData("2023-01-01", "2023-02-01T12:17:24Z", 1, 1)]
    public void ValidateQueryWhenDataIsInvalidShouldReturnInvalidResult(DateTime fromDate, DateTime toDate, int workplaceId, int weeksInInterval)
    {
        // arrange
        Mock<IClockService> clockMock = new();

        clockMock
            .Setup(clock => clock.UtcNow)
            .Returns(new DateTime(2023, 01, 01));

        var validator = new GetCollisionDatesRequest.Validator(clockMock.Object);
        var query = new GetCollisionDatesRequest.Query(
            fromDate,
            toDate,
            workplaceId,
            weeksInInterval,
            0
        );

        // act
        var result = validator.Validate(query);

        // assert
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("2023-01-01", "2023-01-07", 1, 1)]
    public void ValidateQueryWhenDataIsValidShouldReturnValidResult(DateTime fromDate, DateTime toDate, int workplaceId, int weeksInInterval)
    {
        // arrange
        Mock<IClockService> clockMock = new();

        clockMock
            .Setup(clock => clock.UtcNow)
            .Returns(new DateTime(2023, 01, 01));

        var validator = new GetCollisionDatesRequest.Validator(clockMock.Object);
        var query = new GetCollisionDatesRequest.Query(
            fromDate,
            toDate,
            workplaceId,
            weeksInInterval,
            0
        );

        // act
        var result = validator.Validate(query);

        // assert
        result.IsValid.Should().BeTrue();
    }
}

