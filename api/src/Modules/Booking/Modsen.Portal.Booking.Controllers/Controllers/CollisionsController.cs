using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.Booking.Requests.Booking.Requests;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;

namespace Modsen.Portal.Booking.Controllers.Controllers;

[Area("[Booking] Collisions")]
internal class CollisionsController : BaseApiController
{
    private readonly ISender _sender;

    public CollisionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get collisions dates for given workplace number
    /// Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager, Employee, SystemAdministrator
    /// </summary>
    /// <param name="request">request with search properties</param>
    /// <returns>List of collision dates</returns>
    [ProducesResponseType(typeof(List<DateTime>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<DateTime>>> GetAsync([FromQuery] GetCollisionsInput request)
    {
        var token = HttpContext.RequestAborted;
        var dates = await _sender.Send(new GetCollisionDatesRequest.Query(
            request.FromDate,
            request.ToDate,
            request.WorkplaceId,
            request.WeeksInterval,
            request.UserId
        ), token);

        return Ok(dates);
    }
}
