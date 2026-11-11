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
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Conversations")]
internal class ConversationsController : BaseApiController
{
    private readonly ISender _sender;

    public ConversationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get conversations
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// Allowed Claims: ManageHRModule, ManageInterns, ManageAllInterns
    /// </summary>
    /// <param name="queryOptions">Parameters for filtering, searching, ordering</param>
    /// <returns>Pageable list of conversations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpConversation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRManageInternsPolicyName)]
    public async Task<ActionResult<PageableListOfItems<OdpConversation>>> GetPageableConversationsAsync(
        ODataQueryOptions<OdpConversation> queryOptions)
    {
        var result = await _sender.Send(new GetOdpConversationsRequest.Query(queryOptions),
                HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Create conversation
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// Allowed Claims: ManageHRModule
    /// </summary>
    /// <param name="input">Parameters for creating conversation</param>
    /// <returns>Created conversation</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ConversationOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRManageInternsPolicyName)]
    public async Task<ActionResult<ConversationOutput>> CreateConversationAsync(
        [FromBody] CreateConversationInput input)
    {
        var result = await _sender.Send(new CreateConversationRequest.Command(CreateConversationInput.Of(input)),
            HttpContext.RequestAborted);

        return Ok(ConversationOutput.Of(result));
    }

    /// <summary>
    /// Update conversation
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// Allowed Claims: ManageHRModule
    /// </summary>
    /// <param name="input">Parameters for updating conversation</param>
    /// <returns>Conversation</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ConversationOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRManageInternsPolicyName)]
    public async Task<ActionResult<ConversationOutput>> UpdateConversationAsync(
        [FromBody] UpdateConversationInput input)
    {
        var result = await _sender.Send(new UpdateConversationRequest.Command(
            UpdateConversationInput.Of(input)), HttpContext.RequestAborted);

        return Ok(ConversationOutput.Of(result));
    }

    /// <summary>
    /// Get supervisors of conversations related to user
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// Allowed Claims: ManageHRModule
    /// </summary>
    /// <param name="userId">User Id to search supervisors of user's conversations</param>
    /// <returns>Supervisors</returns>
    [HttpGet("supervisors")]
    [ProducesResponseType(typeof(List<SupervisorOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRManageInternsPolicyName)]
    public async Task<ActionResult<List<SupervisorOutput>>> GetSupervisorsByUserIdAsync([FromQuery] int userId)
    {
        var result = await _sender.Send(new GetSupervisorsByUserIdRequest.Query(userId), HttpContext.RequestAborted);

        return Ok(result.ConvertAll(x => new SupervisorOutput(x.Id, x.FullName)));
    }
}
