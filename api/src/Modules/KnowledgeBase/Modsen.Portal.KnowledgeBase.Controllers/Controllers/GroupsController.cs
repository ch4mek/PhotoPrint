using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

namespace Modsen.Portal.KnowledgeBase.Controllers.Controllers;

[Area("[KnowledgeBase] Groups")]
[Route("api/knowledge-groups")]
internal class GroupsController : BaseApiController
{
    private readonly ISender _sender;

    public GroupsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get main groups
    /// Allowed Claims : ViewWiki
    /// </summary>
    /// <returns>Groups with children</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GroupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewWikiPolicy)]
    public async Task<ActionResult<List<GroupOutput>>> GetMainGroupsAsync()
    {
        var groups = await _sender.Send(new GetMainGroupsRequest.Query(), HttpContext.RequestAborted);

        var groupsOutput = groups.Select(GroupOutput.Of).ToList();

        return Ok(groupsOutput);
    }

    /// <summary>
    /// Get group
    /// Allowed Claims : ViewWiki
    /// </summary>
    /// <param name="id">Group id</param>
    /// <returns>Group</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewWikiPolicy)]
    public async Task<ActionResult<GroupOutput>> GetGroupByIdAsync([FromRoute] int id)
    {
        var group = await _sender.Send(new GetGroupByIdRequest.Query(id), HttpContext.RequestAborted);

        var groupOutput = GroupOutput.Of(group);

        return Ok(groupOutput);
    }

    /// <summary>
    /// Create group
    /// Allowed Claims : ManageGroupOrSubGroup
    /// </summary>
    /// <param name="input">Create group input</param>
    /// <returns>Created Group</returns>
    [HttpPost]
    [ProducesResponseType(typeof(GroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageGroupOrSubGroupPolicy)]
    public async Task<ActionResult<GroupOutput>> CreateGroupAsync(
        [FromBody] CreateGroupInput input)
    {
        var group = await _sender.Send(
            new CreateGroupRequest.Command(
                ParentGroupId: input.ParentGroupId,
                Name: input.Name,
                Icon: input.Icon,
                IsActive: input.IsActive,
                IsAvailableOnMainScreen: input.IsAvailableOnMainScreen),
            HttpContext.RequestAborted);

        var groupOutput = GroupOutput.Of(group);

        return Ok(groupOutput);
    }

    /// <summary>
    /// Update group
    /// Allowed Claims : ManageGroup or ManageSubGroup according to ParentGroupId value
    /// </summary>
    /// <param name="id">Group id</param>
    /// <param name="input">Update group input</param>
    /// <returns>Updated Group</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageGroupOrSubGroupPolicy)]
    public async Task<ActionResult<GroupOutput>> UpdateGroupAsync(
        [FromRoute] int id,
        [FromBody] UpdateGroupInput input)
    {
        var group = await _sender.Send(
            new UpdateGroupRequest.Command(
                id,
                ParentGroupId: input.ParentGroupId,
                ChildGroups: input.ChildGroups,
                Name: input.Name,
                Icon: input.Icon,
                IsActive: input.IsActive,
                IsAvailableOnMainScreen: input.IsAvailableOnMainScreen,
                Articles: input.Articles),
            HttpContext.RequestAborted);

        var groupOutput = GroupOutput.Of(group);

        return Ok(groupOutput);
    }

    /// <summary>
    /// Delete Group
    /// Allowed Claims : ManageGroup or ManageSubGroup according to ParentGroupId value
    /// </summary>
    /// <param name="id">Group id</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageGroupOrSubGroupPolicy)]
    public async Task<ActionResult> DeleteGroupAsync(
        [FromRoute] int id)
    {
        await _sender.Send(new DeleteGroupRequest.Command(id), HttpContext.RequestAborted);

        return Ok();
    }

    /// <summary>
    /// Update group name
    /// Allowed Claims : ManageGroup or ManageSubGroup according to ParentGroupId value
    /// </summary>
    /// <param name="id">Group id</param>
    /// <param name="name">New group name</param>
    /// <returns>New group name</returns>
    [HttpPatch("{id:int}/name/{name}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [Authorize(Policy = PortalPolicyNames.ManageGroupOrSubGroupPolicy)]
    public async Task<ActionResult> UpdateGroupName(int id, string name)
    {
        var result = await _sender.Send(new UpdateGroupNameRequest.Command(id, name), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Set order in group
    /// Allowed Claims : ManageOrder
    /// </summary>
    /// <param name="groupOrderInput">Update group order input</param>
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [Authorize(Policy = PortalPolicyNames.ManageOrderPolicy)]
    public async Task<ActionResult> SetOrderInGroupAsync([FromBody] GroupOrderInput groupOrderInput)
    {
        await _sender.Send(new SetGroupOrderRequest.Command(groupOrderInput.GroupOrders), HttpContext.RequestAborted);

        return NoContent();
    }
}
