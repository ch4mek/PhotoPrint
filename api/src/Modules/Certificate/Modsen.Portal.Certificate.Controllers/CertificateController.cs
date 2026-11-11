using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Certificate.UseCases;
using Modsen.Portal.Certificate.UseCases.Dto;
using Modsen.Portal.Certificate.UseCases.Requests;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Certificate.Controllers;

[Area("[Certificate] Certificate")]
internal class CertificateController : BaseApiController
{
    private readonly ISender _sender;

    public CertificateController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all user certificates
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="queryOptions">Odata options for search</param>
    /// <returns>List of user certificates</returns>
    [ProducesResponseType(typeof(PageableListOfItems<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("{userId:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<CertificateDto>>> GetUserCertificates(
        int userId,
        ODataQueryOptions<CertificateDto> queryOptions)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var request = new GetUserCertificatesQueryUseCase.Query(userId, queryOptions);
        var response = await _sender.Send(request, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Creates certificate for specific recipient
    /// Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="createCertificateModel">Model with certificate properties</param>
    /// <returns>Created certificate</returns>
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult> AddingCertificate(CreateCertificateDto createCertificateModel)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var request = new CreateCertificateCommandUseCase.Command(createCertificateModel);
        var response = await _sender.Send(request, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Deletes certificate by its ID
    /// Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="certificateId">Certificate ID</param>
    /// <param name="userId">ID of the user whose certificate provided to deletion</param>
    /// <returns>Deleted certificate</returns>
    [HttpDelete]
    [Route("{certificateId}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [ProducesResponseType(typeof(CertificateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteCertificateAsync([FromRoute] string certificateId, [FromQuery] string userId)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var request = new DeleteCertificateUseCase.Command(certificateId, userId);

        var response = await _sender.Send(request, cancellationToken);

        return Ok(response);
    }
}
