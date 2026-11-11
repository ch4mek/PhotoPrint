using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Education.Requests;
using Modsen.Portal.TeamBoard.Requests.Education.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Users Education")]
[Route("api/teamboard-users/educations")]
internal sealed class UserEducationController : BaseApiController
{
    private readonly ISender _sender;

    public UserEducationController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of user educations
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <param name="userId">id of user</param>
    /// <returns>A list of educations</returns>
    [HttpGet("~/api/teamboard-users/{userId:int}/educations")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpUserEducation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpUserEducation>>> GetPageableUserEducationsAsync(int userId,
        ODataQueryOptions<OdpUserEducation> queryOptions)
    {
        var response = await _sender.Send(new GetOdpEducationsByUserIdRequest.Query(userId, queryOptions),
            HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Create user education
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="createDto"> model to create education</param>
    /// <returns>Created user education</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserEducationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<ActionResult<UserEducationDto>> CreateUserEducation([FromBody] CreateUserEducationDto createDto)
    {
        var result = await _sender.Send(new CreateUserEducationRequest.Command(createDto),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update user education
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id"> id of education</param>
    /// <param name="updateDto"> model to update education</param>
    /// <returns>Updated user education</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserEducationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<ActionResult<UserEducationDto>> UpdateUserEducation([FromBody] UpdateUserEducationDto updateDto,
        int id)
    {
        var result = await _sender.Send(new UpdateUserEducationRequest.Command(updateDto, id),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
