using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.Booking.Controllers.Contracts.Output;
using Modsen.Portal.Booking.Requests.Booking.Core.CoreDto;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Booking.Requests.Workplaces.Requests;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;

namespace Modsen.Portal.Booking.Controllers.Controllers;

[Area("[Booking] Booking")]
internal class BookingController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public BookingController(ISender sender, IAuthenticatedUserContext authenticatedUserContext)
    {
        _sender = sender;
        _authenticatedUserContext = authenticatedUserContext;
    }

    /// <summary>
    /// Returns all bookings or bookings for user
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="getBookingsInput">Model with search properties</param>
    /// <returns>List of bookings</returns>
    [ProducesResponseType(typeof(BookingAndMeetingRoomsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<BookingAndMeetingRoomsDto>> Get([FromQuery] GetBookingsInput getBookingsInput)
    {
        var bookings = await _sender.Send(new GetBookingsRequest.Query(
            getBookingsInput.UserId,
            getBookingsInput.OfficeId,
            getBookingsInput.WorkspaceId,
            getBookingsInput.IncludeEmployee), HttpContext.RequestAborted);

        return Ok(bookings);
    }

    /// <summary>
    /// Creates booking for current user, or for specified user by CTO,CTO,HeadOfDepartment,GroupManager  or SystemAdministrator , or created block booking for specified user by CTO,CTO, HeadOfDepartment, GroupManager
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, Employee, SystemAdministrator
    /// </summary>
    /// <param name="createBookingInput">Model with properties of booking</param>
    /// <returns>Created Booking ID</returns>
    [ProducesResponseType(typeof(BookingOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpPost]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<int>> Create([FromBody] CreateBookingBaseInput createBookingInput)
    {
        BookingEntryCoreDto newBooking;
        var token = HttpContext.RequestAborted;

        switch (createBookingInput.RequestType)
        {
            case CreateBookingBaseInput.CreateSelf:

                newBooking = await _sender.Send(new CreateSelfBookingRequest.Command
                {
                    FromDate = createBookingInput.FromDate,
                    ToDate = createBookingInput.ToDate,
                    WorkplaceId = createBookingInput.WorkplaceId,
                    WeeksInInterval = createBookingInput.WeeksInInterval
                },
                token);

                return Ok(new BookingOutput { BookingId = newBooking.BookingId });

            case CreateBookingBaseInput.CreateForUser when !_authenticatedUserContext.GetRoles().IsEmployee():
                var userId = createBookingInput.UserId ?? _authenticatedUserContext.GetRequiredUserId();

                newBooking = await _sender.Send(new CreateBookingByManagerRequest.Command
                {
                    FromDate = createBookingInput.FromDate,
                    ToDate = createBookingInput.ToDate,
                    WorkplaceId = createBookingInput.WorkplaceId,
                    UserId = userId,
                    WeeksInInterval = createBookingInput.WeeksInInterval
                },
                token);

                return Ok(new BookingOutput { BookingId = newBooking.BookingId });

            case CreateBookingBaseInput.CreateBlock when
                _authenticatedUserContext.GetRoles().IsCeo() ||
                _authenticatedUserContext.GetRoles().IsCto() ||
                _authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
                _authenticatedUserContext.GetRoles().IsGroupManager():
                userId = createBookingInput.UserId ?? _authenticatedUserContext.GetRequiredUserId();

                newBooking = await _sender.Send(new BlockWorkplaceRequest.Command
                {
                    WorkplaceId = createBookingInput.WorkplaceId,
                    StartDate = createBookingInput.FromDate,
                    UserId = userId
                },
                token);

                return Ok(new BookingOutput { BookingId = newBooking.BookingId });

            case CreateBookingBaseInput.CreateForGuest when
                _authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto() ||
                _authenticatedUserContext.GetRoles().IsHeadOfDepartment() || _authenticatedUserContext.GetRoles().IsGroupManager():

                newBooking = await _sender.Send(
                    new CreateGuestBookingRequest.Command
                    {
                        FromDate = createBookingInput.FromDate,
                        ToDate = createBookingInput.ToDate,
                        WorkplaceId = createBookingInput.WorkplaceId,
                        WeeksInInterval = createBookingInput.WeeksInInterval,
                        GuestName = createBookingInput.GuestName!
                    }, token);

                return Ok(new BookingOutput { BookingId = newBooking.BookingId });

            default:
                return BadRequest("You can't use this type of request");
        }
    }

    /// <summary>
    /// Remove booking
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, Employee, SystemAdministrator
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _sender.Send(new CancelBookingRequest.Command(id), HttpContext.RequestAborted);

        return Ok();
    }
}
