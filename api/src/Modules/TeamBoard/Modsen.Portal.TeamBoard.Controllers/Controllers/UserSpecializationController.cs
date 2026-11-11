using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Constants;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] User specialization")]
[Route("api/user-specialization")]
internal class UserSpecializationController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IFeatureManager _featureManager;

    public UserSpecializationController(ISender sender, IFeatureManager featureManager)
    {
        _sender = sender;
        _featureManager = featureManager;
    }

    /// <summary>
    /// lookup user's specializations.
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">User id to find specializations for.</param>
    /// <param name="specializationTemplateName">Param to filter by specialization template name.</param>
    /// <returns>User's specializations</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(UserSpecializationTemplateWithHardSkillsDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserSpecializationTemplateWithHardSkillsDto[]>> GetUserSpecializationByUserIdAsync(
        [FromQuery] int userId, [FromQuery] string? specializationTemplateName)
    {
        var query = new GetUserSpecializationsByUserIdRequest.Query(userId, specializationTemplateName);

        var result = await _sender.Send(query);

        return result;
    }

    /// <summary>
    /// Add user specialization
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userSpecializationInput">Model with user specialization properties</param>
    /// <returns>Created user specialization</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserSpecializationCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSpecializationCoreDto>> CreateUserSpecializationAsync(
        [FromBody] CreateUserSpecializationInput userSpecializationInput)
    {
        var query = new CreateUserSpecializationRequest.Command(
            UserId: userSpecializationInput.UserId,
            SpecializationTemplateId: userSpecializationInput.SpecializationTemplateId,
            SpecializationSeniorityLevelId: userSpecializationInput.SpecializationSeniorityLevelId,
            HardSkills: userSpecializationInput.HardSkills
        );

        var result = await _sender.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Update user's primary specialization.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="updateUserPrimarySpecializationInput">Model with user specialization properties.</param>
    /// <returns>Created primary user specialization</returns>
    [HttpPatch("primary")]
    [ProducesResponseType(typeof(UserSpecializationCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSpecializationCoreDto>> UpdateUserSpecializationAsync(
        [FromBody] UpdatePrimaryUserSpecializationInput updateUserPrimarySpecializationInput)
    {
        var command = new UpdatePrimaryUserSpecializationRequest.Command(
                updateUserPrimarySpecializationInput.UserId,
                updateUserPrimarySpecializationInput.NewSpecializationTemplateId);

        var result = await _sender.Send(command);

        return Ok(result);
    }

    /// <summary>
    /// Get user's specialization by id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">User specialization id.</param>
    /// <returns>User specialization</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserSpecializationCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSpecializationCoreDto>> GetUserSpecializationByIdAsync([FromRoute] int id)
    {
        if (await _featureManager.IsEnabledAsync(FeatureNames.FeatureGetUserSpecializationById))
        {
            var query = new GetUserSpecializationByIdRequest.Query(id);
            var result = await _sender.Send(query);

            return Ok(result);
        }

        return Ok();
    }

    /// <summary>
    /// Delete user's specialization by id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">User specialization id.</param>
    /// <returns>User specialization</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<int>> DeleteUserSpecializationAsync([FromRoute] int id)
    {
        if (await _featureManager.IsEnabledAsync(FeatureNames.FeatureDeleteUserSpecialization))
        {
            var command = new DeleteUserSpecializationRequest.Command(id);

            var result = await _sender.Send(command);

            return Ok(result);
        }

        return Ok();
    }

    /// <summary>
    /// Update user's specializations.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="updateUserSpecializationsInput">Model with user specialization properties.</param>
    /// <returns>Created snapshot</returns>
    [HttpPatch]
    [ProducesResponseType(typeof(UserSnapshotCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSnapshotCoreDto>> UpdateUserSpecializationsAsync(
        [FromBody] UpdateUserSpecializationsInput updateUserSpecializationsInput)
    {
        var command = new UpdateUserSpecializationsRequest.Command(
            updateUserSpecializationsInput.SnapshotId,
            updateUserSpecializationsInput.UserId,
            updateUserSpecializationsInput.UserSpecializations);

        var result = await _sender.Send(command);

        return Ok(result);
    }

    /// <summary>
    /// Get specializations with hard skills
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, Group Manager
    /// </summary>
    /// <param name="userId">The user for whom it searches specialization templates</param>
    /// <param name="areAssigned">The param indicates what (assigned or not assigned) to user specializations will be returned></param>
    /// <returns>Specializations with hard skills</returns>
    [HttpGet("~/api/users/{userId}/specializations")]
    [ProducesResponseType(typeof(SpecializationTemplateWithHardSkillsDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<SpecializationTemplateWithHardSkillsDto[]>> GetUserSpecializationsWithHardSkillsAsync(
        [FromRoute] int userId, [FromQuery] bool? areAssigned)
    {
        var query = new GetSpecializationsWithHardSkillsRequest.Query(userId, areAssigned);

        var result = await _sender.Send(query);

        return result;
    }
}
