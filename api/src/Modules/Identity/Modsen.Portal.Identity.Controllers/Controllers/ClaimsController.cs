using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Claims.Requests;
using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Claims")]
[Route("api")]
public class ClaimsController : BaseApiController
{

    private readonly ISender _sender;

    public ClaimsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>list of departments with claims</returns>
    [ProducesResponseType(typeof(List<DepartmentsClaimsOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpGet("departments/claims")]
    public async Task<ActionResult<List<DepartmentsClaimsOutput>>> GetDepartmentsClaims()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var claims = await _sender.Send(new GetDepartmentClaimsRequest.Query(), cancellationToken);
        var result = claims.Select(DepartmentsClaimsOutput.Of).ToList();
        return Ok(result);
    }

    /// <summary>
    /// Get roles claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>list of roles with claims</returns>
    [ProducesResponseType(typeof(List<RolesClaimsOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpGet("roles/claims")]
    public async Task<ActionResult<List<RolesClaimsOutput>>> GetRolesClaims()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var claims = await _sender.Send(new GetRoleClaimsRequest.Query(), cancellationToken);
        var result = claims.Select(RolesClaimsOutput.Of).ToList();
        return Ok(result);
    }

    /// <summary>
    /// Get users claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>list of users with claims</returns>
    [ProducesResponseType(typeof(PageableListOfItems<PageableListOfItems<UserClaimsDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpGet("users/claims")]
    public async Task<ActionResult<PageableListOfItems<PageableListOfItems<UserClaimsDto>>>> GetUsersClaimsList(
        ODataQueryOptions<UserClaimsDto> queryOptions, string? claimName)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var claims = await _sender.Send(new GetUsersClaimsRequest.Query(queryOptions, claimName), cancellationToken);

        return Ok(claims);
    }

    /// <summary>
    /// Update user claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="claims">User claims</param>
    /// <returns>list of user claims</returns>
    [ProducesResponseType(typeof(List<ClaimGroupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpPatch("users/{userId}/claims")]
    public async Task<ActionResult<List<ClaimGroupOutput>>> UpdateUserClaims(
        [FromRoute] int userId, UserClaimsInput claims)
    {
        ArgumentNullException.ThrowIfNull(claims);

        var cancellationToken = HttpContext.RequestAborted;
        var response = await _sender.Send(new UpdateUserClaimsRequest.Command(userId, claims.ClaimTypes), cancellationToken);

        return response.Select(ClaimGroupOutput.Of).ToList();
    }

    /// <summary>
    /// Get user claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns>list of user claims</returns>
    [ProducesResponseType(typeof(List<ClaimGroupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpGet("users/{userId}/claims")]
    public async Task<ActionResult<List<GroupClaimsCoreDto>>> GetUserClaims(
        [FromRoute] int userId)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var response = await _sender.Send(new GetUserClaimsRequest.Query(userId), cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Get group claims
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>list of groups with claims</returns>
    [ProducesResponseType(typeof(List<ClaimGroupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    [HttpGet("/api/claim-groups/claims")]
    public async Task<ActionResult<List<ClaimGroupOutput>>> GetGroupClaims()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var claims = await _sender.Send(new GetGroupClaimsRequest.Query(), cancellationToken);
        var result = claims.Select(ClaimGroupOutput.Of).ToList();

        return Ok(result);
    }
}
