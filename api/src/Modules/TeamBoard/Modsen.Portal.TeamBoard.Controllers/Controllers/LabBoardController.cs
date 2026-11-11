using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Labboard")]
[Route("api/labboard")]
internal class LabBoardController : BaseApiController
{
    private readonly ISender _sender;

    public LabBoardController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of interns
    /// Allowed Roles : CEO, CTO, users with ManageInterns or ManageAllInterns claim
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <returns>A list of interns</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableOdpLabBoardInternsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewLabBoardPolicy)]
    public async Task<ActionResult<PageableOdpLabBoardInternsOutput>> GetPageableInterns(ODataQueryOptions<OdpLabBoardIntern> queryOptions)
    {
        var pageableLabBoardInterns = await _sender.Send(new GetOdpLabBoardInternsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        return Ok(new PageableOdpLabBoardInternsOutput(pageableLabBoardInterns));
    }

    /// <summary>
    /// Get a list of all interns
    /// Allowed Roles : CEO, CTO, users with ManageInterns or ManageAllInterns claim
    /// </summary>
    /// /// <param name="includeInactive">indicates if inactive interns should be included</param>
    /// <returns>A list of all interns</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<LabBoardInternRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewLabBoardPolicy)]
    public async Task<ActionResult<List<LabBoardInternRequestDto>>> GetLabBoardInternLookup([FromQuery] bool includeInactive = false)
    {
        var labBoardInterns = await _sender.Send(
            new GetLabBoardInternsLookupRequest.Query(includeInactive),
            HttpContext.RequestAborted);

        return Ok(labBoardInterns);
    }

    /// <summary>
    /// Get info about user's internship
    /// Allowed Roles : CEO, CTO, users with ManageInterns or ManageAllInterns or ManageHrModule claim
    /// </summary>
    /// <param name="userId">the id of intern</param>
    /// <returns>internship info</returns>
    [HttpGet("{userId}/internship")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(InternshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewInternshipPolicy)]
    public async Task<ActionResult<InternshipDto>> GetInternship([FromRoute] int userId)
    {
        var labBoardInterns = await _sender.Send(
            new GetInternshipRequest.Query(userId),
            HttpContext.RequestAborted);
        
        return Ok(labBoardInterns);
    }

    /// <summary>
    /// Edit info about user's internship
    /// Allowed Roles : CEO, CTO, users with ManageInterns or ManageAllInterns claim
    /// </summary>
    /// <remarks>users with 'Manage HR Module' claim cannot edit user's internship information</remarks>
    /// <param name="userId">the id of intern</param>
    /// <param name="input">the new internship info</param>
    /// <returns>updated internship info</returns>
    [HttpPut("{userId}/internship")]
    [ProducesResponseType(typeof(InternshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewLabBoardPolicy)]
    public async Task<ActionResult<InternshipDto>> EditInternship([FromRoute] int userId,
        [FromBody] EditInternshipInput input)
    {
        var updatedInternship = await _sender.Send(
            new EditInternshipRequest.Command(
                userId,
                new EditInternshipRequestDto(
                    input.Id,
                    input.Specialization,
                    input.Type,
                    input.EngineerStage,
                    input.CompletedPoints,
                    input.ApprovedPoints,
                    input.FinalResult,
                    input.StartDate,
                    input.PlannedEndDate),
                input.InternshipSpecializations.Select(s => new InternshipSpecializationRequestDto(
                    s.Id,
                    s.SeniorityLevelId,
                    s.SpecializationTemplateId,
                    s.HardSkills.Select(s => new HardSkillSeniorityDto { Id = s.Id, SeniorityLevelId = s.SeniorityLevelId })))),
            HttpContext.RequestAborted);

        return Ok(updatedInternship);
    }

    /// <summary>
    /// Get internship specializations with hard skills
    /// Allowed Roles : CEO, CTO, users with ManageInterns or ManageAllInterns claim
    /// </summary>
    /// <returns>Internship specializations with hard skills</returns>
    [HttpGet("internship/specializations")]
    [ProducesResponseType(typeof(List<SpecializationTemplateWithHardSkillsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewLabBoardPolicy)]
    public async Task<ActionResult<List<SpecializationTemplateWithHardSkillsDto>>> GetInternshipSpecializationsWithHardSkills()
    {
        var query = new GetInternshipSpecializationsWithHardSkillsRequest.Query();

        var result = await _sender.Send(query);

        return result;
    }
}
