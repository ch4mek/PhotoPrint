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
using Modsen.Portal.Identity.DataAccess.Npgsql.OData;
using Modsen.Portal.Identity.Requests.Departments.Requests;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Resource Group")]
[Route("api/resource-groups")]
internal class ResourceGroupController : BaseApiController
{
    private readonly ISender _sender;
    public ResourceGroupController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get resource group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">group id</param>
    /// <returns>Group model</returns>
    [ProducesResponseType(typeof(ResourceGroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:int}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ResourceGroupOutput>> GetGroupById(int id)
    {
        var groupDto = await _sender.Send(
            new GetResourceGroupByIdRequest.Query(id),
            HttpContext.RequestAborted);

        var groupOutput = new ResourceGroupOutput
        {
            Id = groupDto.Id,
            GroupManagerId = groupDto.GroupManagerId,
            Name = groupDto.Name,
            ManagerFirstName = groupDto.ManagerFirstName,
            ManagerLastName = groupDto.ManagerLastName,
            DepartmentId = groupDto.DepartmentId
        };

        return Ok(groupOutput);
    }

    /// <summary>
    /// Get resource groups lookup
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="departmentId">department id</param>
    /// <param name="hasGroupManager">has group manager</param>
    /// <returns>Group lookup models</returns>
    [ProducesResponseType(typeof(List<GroupLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("lookup")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<GroupLookupDto>>> GetGroupsLookup(
        [FromQuery] string? departmentId,
        [FromQuery] bool? hasGroupManager)
    {
        var groupDtos = await _sender.Send(
            new GetResourceGroupsLookup.Query(departmentId, hasGroupManager),
            HttpContext.RequestAborted);

        return Ok(groupDtos);
    }

    /// <summary>
    /// Returns department group managers
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="id">department id</param>
    /// <param name="hasActiveGroup">flag if manager has active group</param>
    /// <returns>List of department group managers</returns>
    [ProducesResponseType(typeof(List<DepartmentGroupManagerOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("~/api/departments/{id}/resource-groups/managers")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<List<DepartmentGroupManagerOutput>>> GetDepartmentGroupManagers(
        [FromQuery] bool? hasActiveGroup,
        string id)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var managerDtos = await _sender.Send(
            new GetDepartmentGroupManagersRequest.Query(id, hasActiveGroup),
            cancellationToken);

        var output = managerDtos
            .Select(dto => new DepartmentGroupManagerOutput
            {
                Id = dto.Id, FirstName = dto.FirstName, LastName = dto.LastName,
            }).ToList();

        return Ok(output);
    }

    /// <summary>
    /// Returns group members
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="id">resource group id</param>
    /// <param name="queryOptions">OData query options</param>
    /// <returns>List of group members</returns>
    [ProducesResponseType(typeof(PageableListOfItems<OdpGroupMember>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:int}/users")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpGroupMember>>> GetGroupMembersAsync(int id,
        ODataQueryOptions<OdpGroupMember> queryOptions)
    {
        var output = await _sender.Send(new GetResourceGroupMembersRequest.Query(id, queryOptions),
            HttpContext.RequestAborted);

        return output;
    }

    /// <summary>
    /// Create a new group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">model of the new group</param>
    /// <param name="id">department id</param>
    /// <returns>Created group's id</returns>
    [ProducesResponseType(typeof(CreateResourceGroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("~/api/departments/{id}/resource-groups")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<CreateResourceGroupOutput>> CreateGroupAsync(
        [FromBody] CreateResourceGroupInput input,
        string id)
    {
        var groupId = await _sender.Send(
            new CreateResourceGroupRequest.Command(input.GroupManagerId, id, input.GroupName),
            HttpContext.RequestAborted);

        return Ok(new CreateResourceGroupOutput { Id = groupId });
    }

    /// <summary>
    /// Update existing group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">model of group updates</param>
    /// <param name="id">group id</param>
    /// <returns>Updated group</returns>
    [ProducesResponseType(typeof(ResourceGroupOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:int}")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<ResourceGroupOutput>> UpdateGroupAsync(
        [FromBody] UpdateResourceGroupInput input,
        int id)
    {
        var updateGroupDto = await _sender.Send(
            new UpdateResourceGroupRequest.Command(id, input.Name, input.GroupManagerId, input.DepartmentId),
            HttpContext.RequestAborted);

        var groupOutput = new ResourceGroupOutput
        {
            Id = updateGroupDto.GroupId,
            GroupManagerId = updateGroupDto.GroupManagerId,
            Name = updateGroupDto.GroupName,
            DepartmentId = input.DepartmentId
        };

        return Ok(groupOutput);
    }

    /// <summary>
    /// Delete existing group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="id">group's id</param>
    /// <returns>Flag is deleted</returns>
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:int}")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<bool>> DeleteGroupAsync(int id)
    {
        var isGroupDeleted = await _sender.Send(
            new DeleteResourceGroupRequest.Command(id),
            HttpContext.RequestAborted);

        return Ok(isGroupDeleted);
    }

    /// <summary>
    /// Add a user to the group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="id">group id</param>
    /// <returns>Updated user</returns>
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("{id:int}/users")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<UserDto>> AddUserToGroupAsync([FromBody] int userId, int id)
    {
        var userDto = await _sender.Send(
            new AddUserToResourceGroupRequest.Command(id, userId),
            HttpContext.RequestAborted);

        return Ok(userDto);
    }

    /// <summary>
    /// Remove a user from the group
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="id">group id</param>
    /// <returns>Updated user</returns>
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete("{id:int}/users/{userId:int}")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<UserDto>> RemoveUserFromGroupAsync(int userId, int id)
    {
        var userDto = await _sender.Send(
            new RemoveUserFromResourceGroupRequest.Command(id, userId),
            HttpContext.RequestAborted);

        return Ok(userDto);
    }
}
