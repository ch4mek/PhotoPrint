using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Kpi Colors")]
[Route("api/kpi-colors")]
internal class KpiColorsController : BaseApiController
{
    private readonly ISender _sender;

    public KpiColorsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get kpi colors
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>List of kpi colors with right borders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<KpiColorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<List<KpiColorDto>>> GetKpiColors()
    {
        var result = await _sender.Send(new GetKpiColorsRequest.Query(), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update kpi colors
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns>List of updated kpi colors with right borders</returns>
    [HttpPut]
    [ProducesResponseType(typeof(List<KpiColorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<List<KpiColorDto>>> UpdateKpiColors(UpdateKpiColorDto[] updateDtos)
    {
        var result = await _sender.Send(new UpdateKpiColorsRequest.Command(updateDtos.ToList()),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
