using Modsen.Portal.Booking.Requests.Services;

namespace Modsen.Portal.Booking.UnitTests.Services;

public class BookingClockServiceTests
{
    private readonly BookingClockService _service = new();
    private static readonly DateOnly _date = DateOnly.FromDateTime(DateTime.UtcNow);
    private const DateTimeKind _dateTimeKind = DateTimeKind.Utc;

    [Theory]
    [MemberData(nameof(TestData))]
    public void RoundUpToHalfAnHourWithBufferSecondsShouldRoundCorrectly(DateTime input, DateTime expected)
    {
        //Act
        var result = _service.RoundUpToHalfAnHourWithBufferSeconds(input);

        //Assert
        result.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return [
            new DateTime(_date, new TimeOnly(14, 0, 0), _dateTimeKind),
            new DateTime(_date, new TimeOnly(13, 30, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 0, 12), _dateTimeKind),
            new DateTime(_date, new TimeOnly(13, 30, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 0, 59), _dateTimeKind),
            new DateTime(_date, new TimeOnly(13, 30, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 1, 0), _dateTimeKind),
            new DateTime(_date, new TimeOnly(14, 0, 0), _dateTimeKind)
        ];
        
        yield return [
            new DateTime(_date, new TimeOnly(14, 29, 59), _dateTimeKind),
            new DateTime(_date, new TimeOnly(14, 0, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 30, 59), _dateTimeKind),
            new DateTime(_date, new TimeOnly(14, 0, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 31, 0), _dateTimeKind),
            new DateTime(_date, new TimeOnly(14, 30, 0), _dateTimeKind)
        ];

        yield return [
            new DateTime(_date, new TimeOnly(14, 59, 59), _dateTimeKind),
            new DateTime(_date, new TimeOnly(14, 30, 0), _dateTimeKind)
        ];
    }
}
