using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Cv.Controllers.Contracts.Input;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Requests;
using Modsen.Portal.Cv.Requests.Cvs.Requests.RequestDto;

namespace Modsen.Portal.Cv.Controllers.Controllers;

[Area("[Cv] Cvs")]
[Route("api/cvs")]
internal class CvsController : BaseApiController
{
    private readonly ISender _sender;

    public CvsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get pageable cvs
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">Parameters for filtering, searching, ordering</param>
    /// <returns>Pageable list of cvs</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpCvDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpCvDto>>> GetPageableCvsAsync(
        ODataQueryOptions<OdpCvDto> queryOptions)
    {
        var result = await _sender.Send(
            new GetPageableCvsRequest.Query(queryOptions));

        return Ok(result);
    }

    /// <summary>
    /// Get a lookup of all cvs
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>A list of all cvs</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CvLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<List<CvLookupDto>>> GetCvsLookup()
    {
        var cvs = await _sender.Send(
            new GetCvsLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(cvs);
    }

    /// <summary>
    /// Get cv by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="cvId">Cv id</param>
    /// <returns>CvCoreDto</returns>
    [HttpGet("{cvId:int}")]
    [ProducesResponseType(typeof(CvCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<CvCoreDto>> GetCvAsync(int cvId)
    {
        var result = await _sender.Send(
            new GetCvByIdRequest.Query(cvId),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Creates a cv
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Model with cv properties</param>
    /// <returns>Created cv</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CvCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<CvCoreDto>> CreateCv(CreateCvDto input)
    {
        var cvDto = await _sender.Send(
            new CreateCvRequest.Command(input),
            HttpContext.RequestAborted);

        return Ok(cvDto);
    }

    /// <summary>
    /// Updates a cv
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Model with cv properties</param>
    /// <returns>Updated cv</returns>
    [HttpPut]
    [ProducesResponseType(typeof(CvCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<CvCoreDto>> UpdateCv(UpdateCvDto input)
    {
        var cvDto = await _sender.Send(
            new UpdateCvRequest.Command(input),
            HttpContext.RequestAborted);

        return Ok(cvDto);
    }

    /// <summary>
    /// Changes cv status
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="cvId">cv id</param>
    /// <param name="input">Model with cv status, client and project</param>
    /// <returns>Created cv</returns>
    [HttpPut("{cvId:int}/status")]
    [ProducesResponseType(typeof(CvCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<CvCoreDto>> ChangeCvStatus(
        [FromRoute]int cvId, ChangeCvStatusInput input)
    {
        var cvDto = await _sender.Send(
            new ChangeCvStatusRequest.Command(
                cvId,
                input.ClientId,
                input.ProjectId,
                input.ClientName,
                input.ProjectName,
                input.Status
                ),
            HttpContext.RequestAborted);

        return Ok(cvDto);
    }

    /// <summary>
    /// Delete cv by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="cvId">Cv id</param>
    /// <returns></returns>
    [HttpDelete("{cvId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult> DeleteCvAsync(int cvId)
    {
        await _sender.Send(
            new DeleteCvRequest.Command(cvId),
            HttpContext.RequestAborted);

        return Ok();
    }
}
