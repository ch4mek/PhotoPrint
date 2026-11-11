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
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;
using Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] External feedbacks")]
[Route("api/external-feedbacks")]
internal class ExternalFeedbacksController : BaseApiController
{
    private readonly ISender _sender;

    public ExternalFeedbacksController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get external feedbacks
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">Parameters for filtering, searching, ordering</param>
    /// <returns>Pageable list of external feedbacks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpExternalFeedback>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpExternalFeedback>>> GetPageableExternalFeedbackAsync(
        ODataQueryOptions<OdpExternalFeedback> queryOptions)
    {
        var result = await _sender.Send(new GetOdpExternalFeedbacksRequest.Query(queryOptions),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Create external feedback
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters for creating</param>
    /// <returns>Created external feedback</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExternalFeedbackOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ExternalFeedbackOutput>> CreateExternalFeedbackAsync(
        [FromBody] CreateExternalFeedbackInput input)
    {
        var command = new CreateExternalFeedbackRequest.Command(input.ExternalFeedbackDate,
                input.ExternalFeedbackType, input.Feedback, input.ActionsTodo, input.Reviewer, input.ConversationId,
                input.ProjectId, input.UserId);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(ExternalFeedbackOutput.Of(result));
    }

    /// <summary>
    /// Update external feedback
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters for updating</param>
    /// <returns>Updated external feedback</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ExternalFeedbackDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ExternalFeedbackDto>> UpdateExternalFeedbackAsync(
        [FromBody] UpdateExternalFeedbackInput input)
    {
        var command = new UpdateExternalFeedbackRequest.Command(input.Id, input.ExternalFeedbackType,
                input.ActionsTodo, input.ConversationId, input.Reviewer);

        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get external feedback
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">ID for searching</param>
    /// <returns>External feedback</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExternalFeedbackWithProjectAndConversationOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ExternalFeedbackWithProjectAndConversationOutput>> GetExternalFeedbackByIdAsync(
        [FromRoute] int id)
    {
        var result = await _sender.Send(new GetExternalFeedbackByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(ExternalFeedbackWithProjectAndConversationOutput.Of(result));
    }

    /// <summary>
    /// Get unique clients of external feedbacks
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">Feedbacks` owner Id</param>
    /// <returns>Unique clients of external feedbacks</returns>
    [HttpGet("clients")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<string>>> GetUniqueExternalFeedbackClientsAsync([FromQuery] int userId)
    {
        var result = await _sender.Send(new GetUniqueExternalFeedbackClientsRequest.Query(userId),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get unique reviewers of external feedbacks
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">Feedbacks` owner Id</param>
    /// <returns>Unique reviewers of external feedbacks</returns>
    [HttpGet("reviewers")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<string>>> GetUniqueExternalFeedbackReviewersAsync([FromQuery] int userId)
    {
        var result = await _sender.Send(new GetUniqueExternalFeedbackReviewersRequest.Query(userId),
                HttpContext.RequestAborted);

        return Ok(result);
    }
}
