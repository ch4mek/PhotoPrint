using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.RateLimiting;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.PayslipsKey;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Payslips")]
[Route("api/payslips")]
internal class PayslipsController : BaseApiController
{
    private readonly ISender _sender;

    public PayslipsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Upload payslip. Access via API key
    /// </summary>
    /// <param name="payslipInput">Model with payslip data to upload</param>
    /// <returns>Added payslip</returns>
    [HttpPost]
    [EnableRateLimiting("PayslipsLimiter")]
    [ServiceFilter(typeof(PayslipsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(PayslipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<PayslipDto>> UploadPayslipAsync([FromBody] UploadPayslipInput payslipInput)
    {
        var result = await _sender.Send(
            new UploadPayslipRequest.Command(payslipInput.FileContent, payslipInput.Period, payslipInput.UserEmail),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get payslips details by user id.
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="queryOptions">Filters</param>
    /// <returns>List with payslips details</returns>
    [HttpGet("~/api/users/{userId:int}/payslips")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpPayslip>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpPayslip>>> GetUserPayslipsAsync(
        [FromRoute] int userId,
        ODataQueryOptions<OdpPayslip> queryOptions)
    {
        var result = await _sender.Send(
            new GetPayslipsRequest.Query(userId, queryOptions),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
