using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Agreement templates")]
[Route("api/agreement-templates")]
internal sealed class AgreementTemplatesController : BaseApiController
{
    private readonly ISender _sender;

    public AgreementTemplatesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create agreement template
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>Id</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<int>> CreateAgreementTemplate(CreateAgreementTemplateInput agreementTemplateInput)
    {
        var result = await _sender.Send(new CreateAgreementTemplateRequest.Command
        {
            Name = agreementTemplateInput.Name,
            DaysPerContractYear = agreementTemplateInput.DaysPerContractYear,
            IsDefault = agreementTemplateInput.IsDefault,
            InquiryTemplateId = agreementTemplateInput.InquiryTemplateId,
        });

        return Ok(result);
    }

    /// <summary>
    /// Delete agreement template
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>Id</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<int>> DeleteAgreementTemplate(int id)
    {
        var result = await _sender.Send(new DeleteAgreementTemplateRequest.Command(id));

        return Ok(result);
    }

    /// <summary>
    /// Update agreement template
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>Updated agreement</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<InquiryAgreementTemplateDto>> UpdateAgreementTemplate([FromRoute] int id, AgreementTemplateInput agreementTemplateInput)
    {
        var result = await _sender.Send(new UpdateAgreementTemplateRequest.Command
        {
            Id = id,
            Name = agreementTemplateInput.Name,
            DaysPerContractYear = agreementTemplateInput.DaysPerContractYear,
            IsDefault = agreementTemplateInput.IsDefault
        });

        return Ok(result);
    }
}
