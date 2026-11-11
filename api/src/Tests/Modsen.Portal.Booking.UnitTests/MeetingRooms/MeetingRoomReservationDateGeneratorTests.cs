using Modsen.Portal.Booking.Requests.BookingPortalSettings;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;

namespace Modsen.Portal.Booking.UnitTests.MeetingRooms;

public class MeetingRoomReservationDateGeneratorTests
{
    private readonly IMeetingRoomReservationDateGenerator _meetingRoomReservationDateGenerator = new MeetingRoomReservationDateGenerator(new TimeZoneConverter());
    private readonly BookingPortalSettingsDto _settingsDto = new()
    {
        ReservationsMonthCount = 3
    };

    [Fact]
    public void GenerateReservationDatesWithRepeatOneWeekShouldReturnEveryWeekReservationForNextThreeMonths()
    {
        // Arrange
        var request = new MeetingRoomReservationDateGeneratorRequest
        {
            ReservationFromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            ReservationToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc),
            ReservationPeriodFromOfficeDate = new DateOnly(2024, 01, 01),
            ReservationPeriodToOfficeDate = new DateOnly(2024, 01, 01),
            RepeatReservationWeeks = 1,
            OfficeTimeZone = "Europe/Minsk",
            ReservationsMonthCount = _settingsDto.ReservationsMonthCount
        };
        var expected = new[]
        {
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 08, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 08, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 15, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 15, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 22, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 22, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 29, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 29, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 05, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 05, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 12, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 12, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 19, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 19, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 26, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 26, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 4, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 4, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 11, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 11, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 18, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 18, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 25, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 25, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 04, 1, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 04, 1, 12, 30, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var dates = _meetingRoomReservationDateGenerator.GenerateReservationDates(request).ToList();

        // Assert
        dates.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateReservationDatesWithRepeatTwoWeekShouldReturnEverySecondWeekReservationForNextThreeMonths()
    {
        // Arrange
        var request = new MeetingRoomReservationDateGeneratorRequest
        {
            ReservationFromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            ReservationToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc),
            ReservationPeriodFromOfficeDate = new DateOnly(2024, 01, 01),
            ReservationPeriodToOfficeDate = new DateOnly(2024, 01, 01),
            RepeatReservationWeeks = 2,
            OfficeTimeZone = "Europe/Minsk",
            ReservationsMonthCount = _settingsDto.ReservationsMonthCount
        };
        var expected = new[]
        {
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 15, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 15, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 29, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 29, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 12, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 12, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 26, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 26, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 11, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 11, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 25, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 25, 12, 30, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var dates = _meetingRoomReservationDateGenerator.GenerateReservationDates(request).ToList();

        // Assert
        dates.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateReservationDatesWithRepeatThreeWeekShouldReturnEveryThirdWeekReservationForNextThreeMonths()
    {
        // Arrange
        var request = new MeetingRoomReservationDateGeneratorRequest
        {
            ReservationFromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            ReservationToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc),
            ReservationPeriodFromOfficeDate = new DateOnly(2024, 01, 01),
            ReservationPeriodToOfficeDate = new DateOnly(2024, 01, 01),
            RepeatReservationWeeks = 3,
            OfficeTimeZone = "Europe/Minsk",
            ReservationsMonthCount = _settingsDto.ReservationsMonthCount
        };
        var expected = new[]
        {
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 22, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 22, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 12, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 12, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 4, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 4, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 25, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 25, 12, 30, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var dates = _meetingRoomReservationDateGenerator.GenerateReservationDates(request).ToList();

        // Assert
        dates.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GenerateReservationDatesWithRepeatFourWeekShouldReturnEveryFourWeekReservationForNextThreeMonths()
    {
        // Arrange
        var request = new MeetingRoomReservationDateGeneratorRequest
        {
            ReservationFromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
            ReservationToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc),
            ReservationPeriodFromOfficeDate = new DateOnly(2024, 01, 01),
            ReservationPeriodToOfficeDate = new DateOnly(2024, 01, 01),
            RepeatReservationWeeks = 4,
            OfficeTimeZone = "Europe/Minsk",
            ReservationsMonthCount = _settingsDto.ReservationsMonthCount
        };
        var expected = new[]
        {
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 01, 29, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 29, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 02, 26, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 02, 26, 12, 30, 0, DateTimeKind.Utc)
            },
            new DateRangeCoreDto
            {
                FromUtc = new DateTime(2024, 03, 25, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 03, 25, 12, 30, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var dates = _meetingRoomReservationDateGenerator.GenerateReservationDates(request).ToList();

        // Assert
        dates.Should().BeEquivalentTo(expected);
    }
}
