using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Contracts")]
[Route("api/contracts")]
internal class ContractsController : BaseApiController
{
    private readonly ISender _sender;

    public ContractsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user's contracts
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <returns>User Contracts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpUserContract>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpUserContract>>> GetUserContractsAsync(
        ODataQueryOptions<OdpUserContract> queryOptions)
    {
        var result = await _sender.Send(
                new GetOdpUserContractsRequest.Query(queryOptions),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Create user contract
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to create new user contract</param>
    /// <returns>Created user contract</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserContractOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRPolicyName)]
    public async Task<ActionResult<UserContractOutput>> CreateUserContractAsync(
        [FromBody] CreateUserContractInput input)
    {
        var command = new CreateUserContractRequest.Command(
                input.UserId,
                input.ContractFrom,
                input.ContractTo,
                input.FireDate,
                input.FireType,
                input.FireReason,
                input.FireDescription);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new UserContractOutput(
            result.Id,
            result.UserId,
            result.ContractFrom,
            result.ContractTo,
            result.FireDate,
            result.FireType,
            result.FireReason,
            result.FireDescription
            ));
    }

    /// <summary>
    /// Update user contract
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to update user contract</param>
    /// <returns>Updated user contract</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UserContractOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRPolicyName)]
    public async Task<ActionResult<UserContractOutput>> UpdateUserContractAsync(
        [FromBody] UpdateUserContractInput input)
    {
        var command = new UpdateUserContractRequest.Command(
            input.Id,
            input.ContractFrom,
            input.ContractTo,
            input.FireDate,
            input.FireType,
            input.FireReason,
            input.FireDescription);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new UserContractOutput(
            result.Id,
            result.UserId,
            result.ContractFrom,
            result.ContractTo,
            result.FireDate,
            result.FireType,
            result.FireReason,
            result.FireDescription));
    }

    /// <summary>
    /// Get user contract
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>User Contract</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserContractOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantPolicyName)]
    public async Task<ActionResult<UserContractOutput>> GetUserContractByIdAsync([FromRoute] int id)
    {
        var result = await _sender.Send(
                new GetUserContractByIdRequest.Query(id),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get active users with contracts expiring soon
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>List of user names and contract end dates</returns>
    [HttpGet("active-expiring")]
    [ProducesResponseType(typeof(List<ActiveExpiringContractOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<ActiveExpiringContractOutput>>> GetActiveExpiringContractsAsync([FromQuery] string? departmentId)
    {
        var result = await _sender.Send(new GetActiveExpiringContractsRequest.Query(departmentId), HttpContext.RequestAborted);

        return Ok(result);
    }
}
