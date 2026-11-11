using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.FeatureManagement;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Constants;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Specializations")]
[Route("api/specialization-templates")]
internal class SpecializationTemplateController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IFeatureManager _featureManager;

    public SpecializationTemplateController(ISender sender, IFeatureManager featureManager)
    {
        _sender = sender;
        _featureManager = featureManager;
    }

    /// <summary>
    /// Create specialization template
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="model">model with specialization template properties</param>
    /// <returns>Created specialization template</returns>
    [ProducesResponseType(typeof(SpecializationTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<SpecializationTemplateOutput>> CreateSpecializationTemplateAsync(
        [FromBody] CreateSpecializationTemplateInput model)
    {
        var command = new CreateSpecializationTemplateRequest.Command(model.ToRequestDto());

        var createdSpecializationTemplate = await _sender.Send(
            command,
            HttpContext.RequestAborted);

        return Ok(createdSpecializationTemplate);
    }

    /// <summary>
    /// Update specialization template
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="model">model with specialization template properties</param>
    /// <returns>Updated specialization template</returns>
    [ProducesResponseType(typeof(SpecializationTemplateWithDepartmentsAndHardSkillsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPut]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<SpecializationTemplateWithDepartmentsAndHardSkillsDto>> UpdateSpecializationTemplateAsync(
        [FromBody] UpdateSpecializationTemplateInput model)
    {
        var command = new UpdateSpecializationTemplateRequest.Command(model.ToRequestDto());

        var updatedSpecializationTemplate = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(updatedSpecializationTemplate);
    }

    /// <summary>
    /// Get a list of specialization templates
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <param name="departmentId">parameters for filtering by department</param>
    /// <returns>A list of specialization templates</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpSpecializationTemplate>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpSpecializationTemplate>>> GetPageableSpecializationTemplatesAsync(
        ODataQueryOptions<OdpSpecializationTemplate> queryOptions,
        [FromQuery] string? departmentId)
    {
        var response = await _sender.Send(
            new GetOdpSpecializationTemplatesRequest.Query(queryOptions, departmentId),
            HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Get a specialization template by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">parameter for searchin specialization templates</param>
    /// <returns>Specialization template with specific id</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SpecializationTemplateWithDepartmentsAndHardSkillsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<SpecializationTemplateWithDepartmentsAndHardSkillsOutput>> GetSpecializationTemplateByIdAsync([FromRoute] int id)
    {
        var response = await _sender.Send(
            new GetSpecializationTemplateByIdRequest.Query(id),
            HttpContext.RequestAborted);

        return Ok(new SpecializationTemplateWithDepartmentsAndHardSkillsOutput()
        {
            Id = response.Id,
            Name = response.Name,
            HardSkills = response.HardSkills,
            DepartmentDtos = response.DepartmentDtos.ToArray()
        });
    }

    /// <summary>
    /// Delete specialization template by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">parameter for specialization template deletion by its id</param>
    /// <returns>An id of deleted specialization template</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteSpecializationTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<int>> DeleteSpecializationTemplatesAsync([FromRoute] int id)
    {
        var response = await _sender.Send(new DeleteSpecializationTemplateRequest.Command(id),
               HttpContext.RequestAborted);

        return Ok(new DeleteSpecializationTemplateOutput(response.Id, response.Name));
    }

    /// <summary>
    /// Get specialization lookup
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">The user for whom it searches specialization templates</param>
    /// <param name="areAssigned">The param indicates what (assigned or not assigned) to user specialization templates will be returned</param>
    /// <returns>Specialization lookup</returns>
    [HttpGet("~/api/users/{userId:int}/specialization-templates")]
    [ProducesResponseType(typeof(List<SpecializationTemplateLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<SpecializationTemplateLookupOutput>>> GetSpecializationTemplatesLookup([FromRoute] int userId, [FromQuery] bool? areAssigned)
    {
        if (await _featureManager.IsEnabledAsync(FeatureNames.FeatureGetSpecializationTemplatesUserDoesNotHave))
        {
            var response = await _sender.Send(
                new GetSpecializationTemplatesLookupRequest.Query(userId, areAssigned), HttpContext.RequestAborted);

            return Ok(response.Select(template =>
                new SpecializationTemplateLookupOutput(Id: template.Id, Name: template.Name)).ToList());
        }

        return Ok(new List<SpecializationTemplateOutput>());
    }
}
