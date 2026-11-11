using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Development plans")]
[Route("api/development-plans")]
internal class DevelopmentPlansController : BaseApiController
{
    private readonly ISender _sender;

    public DevelopmentPlansController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get development plan by Id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">ID of development plan to get</param>
    /// <returns>Development plan</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DevelopmentPlanOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<DevelopmentPlanOutput>> GetDevelopmentPlanByIdAsync([FromRoute] int id)
    {
        var result = await _sender.Send(new GetDevelopmentPlanByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(DevelopmentPlanOutput.Of(result));
    }

    /// <summary>
    /// Get development plans
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <returns>Development plans</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpDevelopmentPlan>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpDevelopmentPlan>>> GetPageableDevelopmentPlansAsync(
        [FromQuery] DevelopmentPlanSource? source,
        [FromQuery] bool? isDone,
        [FromQuery] int? conversationId,
        ODataQueryOptions<OdpDevelopmentPlan> queryOptions)
    {
        var result = await _sender.Send(new GetOdpDevelopmentPlansRequest.Query(source, isDone, conversationId, queryOptions),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Create development plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to create new development plan</param>
    /// <param name="source">Create requested from</param>
    /// <returns>Created development plan</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DevelopmentPlanOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<DevelopmentPlanOutput>> CreateDevelopmentPlanAsync(
        [FromQuery] DevelopmentPlanSource? source,
        [FromBody] CreateDevelopmentPlanInput input)
    {
        var command = new CreateDevelopmentPlanRequest.Command(input.StartDate, input.DeadlineDate, input.Summary ?? string.Empty, input.Notes, input.UserId, source, input.ConversationId);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(DevelopmentPlanOutput.Of(result));
    }

    /// <summary>
    /// Delete development plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Development plan id</param>
    /// <returns>Deleted development plan id</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<int>> DeleteDevelopmentPlanAsync(int id)
    {
        var command = new DeleteDevelopmentPlanRequest.Command(id);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update Status many development plan
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="input">Parameters to update status for development plan</param>
    /// <returns>updated development plans</returns>
    [HttpPatch("development-plans/status")]
    [ProducesResponseType(typeof(DevelopmentPlanDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<DevelopmentPlanDto>> UpdateDevelopmentPlanStatus(
        [FromBody] UpdateDevelopmentPlanStatusInput input)
    {
        var command = new UpdateDevelopmentPlanStatusRequest.Command(
            input.DevelopmentPlanId,
            input.DevelopmentPlanStatus,
            input.ConfirmationDate);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update development plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to update development plan</param>
    /// <returns>Updated development plan</returns>
    [HttpPut]
    [ProducesResponseType(typeof(DevelopmentPlanOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<DevelopmentPlanOutput>> UpdateDevelopmentPlanAsync(
        [FromBody] UpdateDevelopmentPlanInput input)
    {
        var command = new UpdateDevelopmentPlanRequest.Command(input.Id, input.DeadlineDate, input.Summary ?? string.Empty, input.Notes);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(DevelopmentPlanOutput.Of(result));
    }

    /// <summary>
    /// Create many development plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to create new development plan</param>
    /// <param name="source">Create requested from</param>
    /// <returns>Created development plans</returns>
    [HttpPost("development-plans")]
    [ProducesResponseType(typeof(List<DevelopmentPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<DevelopmentPlanDto>>> CreateDevelopmentPlansAsync(
        [FromQuery] DevelopmentPlanSource? source,
        [FromBody] List<CreateDevelopmentPlanInput> input)
    {
        var command = new CreateDevelopmentPlanListRequest.Command(
            input.ConvertAll(x => new CreateDevelopmentPlanDto
            (
                Id: 0,
                StartDate: x.StartDate,
                DeadlineDate: x.DeadlineDate,
                EndDate: null,
                Summary: x.Summary ?? string.Empty,
                Notes: x.Notes,
                UserId: x.UserId,
                ConversationId: x.ConversationId
            )), source);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update development plan list
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters to update development plans</param>
    /// <returns>Updated development plan list</returns>
    [HttpPatch("development-plans")]
    [ProducesResponseType(typeof(List<DevelopmentPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<UpdateDevelopmentPlansDto>>> UpdateDevelopmentPlansAsync(
        [FromBody] List<UpdateDevelopmentPlanInput> input)
    {
        var command = new UpdateDevelopmentPlanListRequest.Command(
            input.ConvertAll(x => new UpdateDevelopmentPlansDto(
                Id: x.Id,
                DeadlineDate: x.DeadlineDate,
                EndDate: null,
                Summary: x.Summary ?? string.Empty,
                Notes: x.Notes)));

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }
}
