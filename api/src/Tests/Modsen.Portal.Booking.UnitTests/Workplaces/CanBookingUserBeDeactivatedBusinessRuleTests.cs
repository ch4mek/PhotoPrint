using MockQueryable.Moq;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Booking.Requests.Workplaces.Core.BusinessRules;
using Modsen.Portal.Core.Services;
using Moq;

namespace Modsen.Portal.Booking.UnitTests.Workplaces;

public class CanBookingUserBeDeactivatedBusinessRuleTests
{
    private readonly ICanBookingUserBeDeactivatedBusinessRule _rule;
    private readonly List<MeetingRoomReservationEntity> _meetingRoomReservations = [];
    private readonly List<Reservation> _workplaceReservations = [];
    private readonly Mock<IClockService> _clockMock;
    private readonly CancellationToken _token = CancellationToken.None;

    public CanBookingUserBeDeactivatedBusinessRuleTests()
    {

        var dbContextMock = new Mock<IBookingDbContext>();

        dbContextMock
            .Setup(x => x.MeetingRoomReservations)
            .Returns(_meetingRoomReservations.BuildMockDbSet().Object);

        dbContextMock
            .Setup(x => x.Reservations)
            .Returns(_workplaceReservations.BuildMockDbSet().Object);

        _clockMock = new Mock<IClockService>(MockBehavior.Strict);

        _rule = new CanBookingUserBeDeactivatedBusinessRule(
            dbContextMock.Object,
            _clockMock.Object);
    }

    [Fact]
    public async Task Test1()
    {
        // Arrange
        _clockMock.Setup(x => x.TodayStartOfDayUtc())
            .Returns(new DateTime(2024, 10, 16));

        var userId = 1;
        _meetingRoomReservations.Add(new MeetingRoomReservationEntity
        {
            UserId = userId,
            ReservationFromUtc = new DateTime(2024, 10, 17, 12, 0, 0),
            ReservationToUtc = new DateTime(2024, 10, 17, 13, 0, 0),
            IsDeleted = false
        });

        // Act
        var isCorrespond = await _rule.IsCorrespondsToBusiness(userId, _token);

        //Assert
        isCorrespond.Should().Be(false);
    }
}
