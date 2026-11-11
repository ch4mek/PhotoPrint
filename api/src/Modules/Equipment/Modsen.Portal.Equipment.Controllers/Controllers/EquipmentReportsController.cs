using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.SecurityHash;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Reports.Requests;

namespace Modsen.Portal.Equipment.Controllers.Controllers;

[Area("[Equipment] Reports")]
internal class EquipmentReportsController : BaseApiController
{
    private readonly ISender _sender;

    public EquipmentReportsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Upload file with an equipment report json data to server
    /// Allowed Roles: Need SecurityHash header
    /// </summary>
    /// <param name="file">File form to upload</param>
    /// <returns>Equipment report</returns>
    [ProducesResponseType(typeof(EquipmentReportOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [AllowAnonymous]
    [ServiceFilter(typeof(SecurityHashHeaderAttribute))]
    public async Task<ActionResult<EquipmentReportOutput>> CreateEquipmentReportAsync(
        IFormFile file)
    {
        var result = await _sender.Send(
            new CreateEquipmentReportRequest.Command(file),
            HttpContext.RequestAborted);

        return CreatedAtAction(
            nameof(GetEquipmentReportAsync),
            new { result.Id },
            EquipmentReportOutput.Of(result));
    }

    /// <summary>
    /// Get equipment report relevance periods by serial number
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="serialNumber">Equipment serial number</param>
    /// <returns>Equipment report relevance periods</returns>
    [ProducesResponseType(typeof(EquipmentReportOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{serialNumber}")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<List<EquipmentReportRelevancePeriodOutput>>> GetEquipmentReportRelevancePeriodsAsync(
        [FromRoute] string serialNumber)
    {
        var result = await _sender.Send(
            new GetEquipmentReportRelevancePeriodsRequest.Query(serialNumber),
            HttpContext.RequestAborted);

        return Ok(result.Select(EquipmentReportRelevancePeriodOutput.Of).ToList());
    }

    /// <summary>
    /// Get equipment report by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="id">Equipment Report id</param>
    /// <returns>Equipment Report</returns>
    [ProducesResponseType(typeof(EquipmentReportOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:int}")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<EquipmentReportOutput>> GetEquipmentReportAsync(
        [FromRoute] int id)
    {
        var result = await _sender.Send(
            new GetEquipmentReportByIdRequest.Query(id),
            HttpContext.RequestAborted);

        return Ok(EquipmentReportOutput.Of(result));
    }
}
