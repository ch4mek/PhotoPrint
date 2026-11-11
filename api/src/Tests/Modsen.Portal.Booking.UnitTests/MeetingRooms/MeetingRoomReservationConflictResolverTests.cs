using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.MeetingRooms;

public class IMeetingRoomReservationReservationConflictResolverTests
{
    private readonly IMeetingRoomReservationConflictResolver _reservationConflictResolver;
    private const int s_meetingRoomId = 2;
    private const int s_userId = 6543;
    private const int s_officeId = 100;
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly MeetingRoomEntity _meetingRoom = new()
    {
        Id = s_meetingRoomId,
        OfficeId = s_officeId,
        Name = "Meeting Room 1"
    };

    private readonly MeetingRoomReservationEntity _reservation = new()
    {
        Id = 18,
        MeetingRoomEntry = new MeetingRoomEntryEntity
        {
            MeetingRoomId = s_meetingRoomId,
            UserId = s_userId
        },
        ReservationFromUtc = new DateTime(2024, 01, 01, 15, 0, 0, DateTimeKind.Utc),
        ReservationToUtc = new DateTime(2024, 01, 01, 16, 0, 0, DateTimeKind.Utc)
    };

    public IMeetingRoomReservationReservationConflictResolverTests()
    {
        var bookingDbContextMock = new Mock<IBookingDbContext>();
        var otherOfficeMeetingRoom = new MeetingRoomEntity
        {
            Id = s_meetingRoomId + 1,
            OfficeId = s_officeId + 1,
            Name = "Meeting Room 1"
        };

        var reservations = new List<MeetingRoomReservationEntity>
        {
            _reservation
        };

        var meetingRooms = new List<MeetingRoomEntity>
        {
            _meetingRoom,
            otherOfficeMeetingRoom
        };

        _reservation.MeetingRoomEntry.MeetingRoom = _meetingRoom;

        bookingDbContextMock
            .Setup(x => x.MeetingRoomReservations)
            .Returns(reservations.BuildMockDbSet().Object);

        bookingDbContextMock
            .Setup(x => x.MeetingRooms)
            .Returns(meetingRooms.BuildMockDbSet().Object);

        _reservationConflictResolver = new MeetingRoomReservationReservationConflictResolver(bookingDbContextMock.Object);
    }

    [Fact]
    public async Task GetConflictsForDateEarlierAtTheDayReturnZeroConflicts()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 13, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetConflictsForDateLaterAtTheDayReturnZeroConflicts()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 20, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 21, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetConflictsForDateEarlierContinuouslyTheStartOfOneExistingReturnZeroConflicts()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 14, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 15, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetConflictsForDateLaterContinuouslyTheEndOfOneExistingReturnZeroConflicts()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 16, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 17, 0, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts.Should().HaveCount(0);
    }

    [Fact]
    public async Task GetConflictsForDateStartsOfOneExistingReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 15, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 16, 15, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateStartsOfOneExistingAndEndAtReservationEndReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 15, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 16, 00, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateInsideExistingReservationReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 15, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 15, 50, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateOutsideExistingReservationReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 12, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 20, 50, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateEndsOfOneExistingReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 14, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 15, 15, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateEndsOfOneExistingAndEndAtReservationEndReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 14, 45, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 16, 00, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForDateStatsOfOneExistingAndEndLaterReturnExistingReservationAsConflict()
    {
        // Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 15, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 16, 15, 0, DateTimeKind.Utc)
            }
        };

        // Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(dates, s_meetingRoomId, s_userId, _token);

        // Assert
        conflicts
            .Should()
            .BeEquivalentTo([_reservation.Id]);
    }

    [Fact]
    public async Task GetConflictsForOverlappingReservationInDifferentOfficesReturnZeroConflicts()
    {
        //Arrange
        var dates = new List<DateRangeCoreDto>
        {
            new()
            {
                FromUtc = new DateTime(2024, 01, 01, 15, 0, 0, DateTimeKind.Utc),
                ToUtc = new DateTime(2024, 01, 01, 16, 15, 0, DateTimeKind.Utc)
            }
        };

        //Act
        var conflicts = await _reservationConflictResolver.ResolveConflictsFor(
            dates,
            s_meetingRoomId + 1,
            s_userId,
            _token);

        //Assert
        conflicts.Should().HaveCount(0);
    }
}
