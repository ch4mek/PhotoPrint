using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.Booking.Requests.MeetingRooms.Core.CoreDto;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests;
using Modsen.Portal.Booking.Requests.MeetingRooms.Requests.RequestDto;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;

namespace Modsen.Portal.Booking.Controllers.Controllers;

[Area("[Booking] Meeting rooms")]
internal class MeetingRoomsController : BaseApiController
{
    private readonly ISender _sender;

    public MeetingRoomsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get collisions dates for given meeting room
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="meetingRoomId">Meeting room id</param>
    /// <param name="model">Model with search properties</param>
    /// <returns>Array of collision dates for meeting room</returns>
    [ProducesResponseType(typeof(MeetingRoomReservationCoreDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpGet("~/api/meeting-rooms/{meetingRoomId:int}/collisions")]
    public async Task<ActionResult<MeetingRoomReservationCoreDto[]>> GetMeetingRoomCollisions([FromRoute] int meetingRoomId, [FromQuery] GetMeetingRoomCollisionsInput model)
    {
        var reservations = await _sender.Send(new GetMeetingRoomCollisionsRequest.Query
        {
            UserId = model.UserId,
            MeetingRoomId = meetingRoomId,
            ReservationFromUtc = model.ReservationFromUtc,
            ReservationToUtc = model.ReservationToUtc,
            ReservationPeriodFromOfficeDate = model.ReservationPeriodFromOfficeDate,
            ReservationPeriodToOfficeDate = model.ReservationPeriodToOfficeDate,
            RepeatReservationWeeks = model.RepeatReservationWeeks
        }, HttpContext.RequestAborted);

        return Ok(reservations);
    }

    /// <summary>
    /// Creates meeting room reservation
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="meetingRoomId">Meeting room id</param>
    /// <param name="model">Model with reservation properties</param>
    /// <returns>Booked meeting room</returns>
    [ProducesResponseType(typeof(MeetingRoomEntryCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpPost("~/api/meeting-rooms/{meetingRoomId:int}/meeting-room-reservations")]
    public async Task<ActionResult<MeetingRoomEntryCoreDto>> BookMeetingRoom([FromRoute] int meetingRoomId, [FromBody] BookMeetingRoomInput model)
    {
        var entry = await _sender.Send(new BookMeetingRoomRequest.Command
        {
            UserId = model.UserId,
            MeetingRoomId = meetingRoomId,
            ReservationFromUtc = model.ReservationFromUtc,
            ReservationToUtc = model.ReservationToUtc,
            ReservationPeriodFromOfficeDate = model.ReservationPeriodFromOfficeDate,
            ReservationPeriodToOfficeDate = model.ReservationPeriodToOfficeDate,
            RepeatReservationWeeks = model.RepeatReservationWeeks
        }, HttpContext.RequestAborted);

        return Ok(entry);
    }

    /// <summary>
    /// Get all office meeting room reservations by date
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="officeId">Office id</param>
    /// <param name="date">Reservation date</param>
    /// <returns>Array of meeting rooms with reservations</returns>
    [ProducesResponseType(typeof(MeetingRoomReservationGroupRequestDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpGet("~/api/offices/{officeId:int}/calendar/{date}/meeting-room-reservations")]
    public async Task<ActionResult<MeetingRoomReservationGroupRequestDto[]>> GetAllOfficeReservationsByDate([FromRoute] int officeId, [FromRoute] DateOnly date)
    {
        var reservations = await _sender.Send(new GetAllOfficeReservationsByDateRequest.Query
        {
            OfficeId = officeId,
            DayInOfficeTimeZone = date
        }, HttpContext.RequestAborted);

        return Ok(reservations);
    }

    /// <summary>
    /// Get all office meeting room entries for today
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="officeId">Office id</param>
    /// <returns>Array of meeting room entries with reservations</returns>
    [ProducesResponseType(typeof(MeetingRoomEntryCoreDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpGet("~/api/offices/{officeId:int}/meeting-room-entries")]
    public async Task<ActionResult<MeetingRoomEntryCoreDto[]>> GetAllOfficeEntriesFromToday([FromRoute] int officeId)
    {
        var entries = await _sender.Send(new GetAllOfficeEntriesFromTodayRequest.Query
        {
            OfficeId = officeId
        }, HttpContext.RequestAborted);

        return Ok(entries);
    }

    /// <summary>
    /// Get all office workplaces
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="officeId">Office id</param>
    /// <returns>Array office workplaces</returns>
    [ProducesResponseType(typeof(OfficeWorkplaceRequestDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpGet("~/api/offices/{officeId:int}/workplaces")]
    public async Task<ActionResult<OfficeWorkplaceRequestDto[]>> GetOfficeWorkplaces([FromRoute] int officeId)
    {
        var workplaces = await _sender.Send(new GetOfficeWorkplacesRequest.Query(officeId), HttpContext.RequestAborted);
        return Ok(workplaces);
    }

    /// <summary>
    /// Delete meeting room entry
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="meetingRoomEntryId">Meeting room entry id</param>
    /// <returns>Array of deleted reservation ids</returns>
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [HttpDelete("~/api/meeting-rooms-entries/{meetingRoomEntryId:int}")]
    public async Task<ActionResult<int[]>> DeleteMeetingRoomEntry(int meetingRoomEntryId)
    {
        var reservationIds = await _sender.Send(new DeleteMeetingRoomEntryRequest.Command
        {
            MeetingRoomEntryId = meetingRoomEntryId
        }, HttpContext.RequestAborted);

        return Ok(reservationIds);
    }
}
