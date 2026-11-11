using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiry templates")]
[Route("api/inquiry-templates")]
internal class InquiryTemplatesController : BaseApiController
{
    private readonly ISender _sender;

    public InquiryTemplatesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all templates
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>List of InquiryTemplateOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<InquiryTemplateOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<List<InquiryTemplateOutput>>> GetAllTemplatesAsync()
    {
        var templates = await _sender.Send(new GetAllTemplatesRequest.Command(), HttpContext.RequestAborted);

        var output = templates.Select(InquiryTemplateOutput.Of).ToList();

        return Ok(output);
    }

    /// <summary>
    /// Get inquiry templates lookup
    /// Allowed Roles : all
    /// </summary>
    /// <returns>List of InquiryTemplateOutput</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<InquiryTemplatesLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageRequestsPolicy)]
    public async Task<ActionResult<List<InquiryTemplatesLookupDto>>> GetInquiryTemplatesLookupAsync()
    {
        var templateLookup = await _sender.Send(new GetInquiryTemplatesLookupRequest.Command(), HttpContext.RequestAborted);

        return Ok(templateLookup);
    }

    /// <summary>
    /// Get template by id
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Template id</param>
    /// <returns>InquiryTemplateOutput</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(InquiryTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<InquiryTemplateOutput>> GetTemplateByIdAsync(int id)
    {
        var template = await _sender.Send(new GetTemplateByIdRequest.Query(id), HttpContext.RequestAborted);

        var output = InquiryTemplateOutput.Of(template);

        return Ok(output);
    }

    /// <summary>
    /// Create template
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Template input model for entity creating</param>
    /// <returns>InquiryTemplateOutput</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InquiryTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<InquiryTemplateOutput>> CreateTemplateAsync(
        [FromBody] InquiryTemplateInput input)
    {
        var template = await _sender.Send(new CreateTemplateRequest.Command
        {
            Name = input.Name,
            Type = input.Type,
            Comment = input.Comment,
            ActivityType = input.ActivityType,
            TrackingUnit = input.TrackingUnitType,
            AccrualType = input.AccrualType,
            TransferType = input.TransferType,
            IsProhibitOnProbationPeriod = input.IsProhibitOnProbationPeriod,
            IsRequireAttachments = input.IsRequireAttachments,
            IsIncludeHolidays = input.IsIncludeHolidays,
            IsIncludeWeekends = input.IsIncludeWeekends,
            IsSubstitutesToApproveAllowed = input.IsSubstitutesToApproveAllowed,
            AcceptPolicies = input.AcceptPolicies.ToList(),
        }, HttpContext.RequestAborted);

        var output = InquiryTemplateOutput.Of(template);

        return Ok(output);
    }

    /// <summary>
    /// Update template
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Template id for updating</param>
    /// <param name="input">Template input model for entity updating</param>
    /// <returns>InquiryTemplateOutput</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(InquiryTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<InquiryTemplateOutput>> UpdateTemplateAsync(int id,
        [FromBody] InquiryTemplateInput input)
    {
        var template = await _sender.Send(new UpdateTemplateRequest.Command
        {
            Id = id,
            Name = input.Name,
            Type = input.Type,
            IsProhibitOnProbationPeriod = input.IsProhibitOnProbationPeriod,
            Comment = input.Comment,
            ActivityType = input.ActivityType,
            TrackingUnit = input.TrackingUnitType,
            IsRequireAttachments = input.IsRequireAttachments,
            IsIncludeHolidays = input.IsIncludeHolidays,
            IsIncludeWeekends = input.IsIncludeWeekends,
            TransferType = input.TransferType,
            AccrualType = input.AccrualType,
            IsSubstitutesToApproveAllowed = input.IsSubstitutesToApproveAllowed,
            AcceptPolicies = input.AcceptPolicies.ToList(),
        }, HttpContext.RequestAborted);

        var output = InquiryTemplateOutput.Of(template);

        return Ok(output);
    }

    /// <summary>
    /// Make template inactive
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Template id</param>
    /// <returns>InquiryTemplateOutput</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(InquiryTemplateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<InquiryTemplateOutput>> DeleteTemplateAsync([FromRoute] int id)
    {
        var template = await _sender.Send(new DeleteTemplateRequest.Command(id), HttpContext.RequestAborted);

        var templateOutput = InquiryTemplateOutput.Of(template);

        return Ok(templateOutput);
    }
}
