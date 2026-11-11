using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.Booking.Controllers.Contracts.Output;
using Modsen.Portal.Booking.Requests.Workplaces.Requests;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;

namespace Modsen.Portal.Booking.Controllers.Controllers;

[Area("[Booking] Reservations")]
public class ReservationController : BaseApiController
{
    private readonly ISender _sender;

    public ReservationController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get reservations for the period
    /// Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager, Employee, SystemAdministrator
    /// </summary>
    /// <param name="input">Input with period properties</param>
    /// <returns>List of reservations for the period</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetReservationForDate>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<GetReservationForDate>>> Get([FromQuery] GetReservationsForPeriodInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var cancellationToken = HttpContext.RequestAborted;
        var reservations = await _sender.Send(new GetReservationsRequest.Query(
            FromDate: input.FromDate,
            ToDate: input.ToDate,
            OfficeId: input.OfficeId,
            WorkspaceId: input.WorkspaceId), cancellationToken);

        return Ok(reservations.Select(GetReservationForDate.Of).ToList());
    }
}
