using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Calendar")]
[Route("api/calendar")]
internal class CalendarController(ISender sender) : BaseApiController
{
    /// <summary>
    /// Get user's contracts
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="date">Date</param>
    /// <returns>Users</returns>
    [HttpGet("{date}/birthdays")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<IActionResult> GetBirthdayForDayAsync([FromRoute] DateOnly date)
    {
        var users = await sender.Send(
            new GetUsersByBirthdayDateRequest.Query(Date: date),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get user's contracts
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="from">Date from</param>
    /// <param name="to">Date to</param>
    /// <returns>Users</returns>
    [HttpGet("{from}to/{to}/birthdays")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<IActionResult> GetBirthdayForIntervalAsync([FromRoute] DateOnly from, [FromRoute] DateOnly to)
    {
        var users = await sender.Send(
            new GetUsersByBirthdayIntervalRequest.Query(from, to),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get user's contracts
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="date">Date</param>
    /// <returns>Users</returns>
    [HttpGet("{date}/anniversaries")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<IActionResult> GetAnniversariesForDayAsync([FromRoute] DateOnly date)
    {
        var users = await sender.Send(
            new GetUsersByAnniversariesDateRequest.Query(date),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get user's contracts
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="from">Date from</param>
    /// <param name="to">Date to</param>
    /// <returns>Users</returns>
    [HttpGet("{from}/to/{to}/anniversaries")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<IActionResult> GetAnniversariesForIntervalAsync([FromRoute] DateOnly from, [FromRoute] DateOnly to)
    {
        var users = await sender.Send(
            new GetUsersByAnniversariesIntervalRequest.Query(from, to),
            HttpContext.RequestAborted);

        return Ok(users);
    }
}
