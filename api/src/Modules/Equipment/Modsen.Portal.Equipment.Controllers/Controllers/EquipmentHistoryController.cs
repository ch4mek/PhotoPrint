using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

namespace Modsen.Portal.Equipment.Controllers.Controllers;

[Area("[Equipment] History")]
[Route("api/[controller]/{equipmentId:int}")]
internal class EquipmentHistoryController : BaseApiController
{
    private readonly ISender _sender;

    public EquipmentHistoryController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get equipment holders
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="queryOptions">Odata options for search</param>
    /// <returns>Holders of equipment</returns>
    [ProducesResponseType(typeof(PageableAssignmentHistoryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Route("/api/[controller]/assignments")]
    [HttpGet]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<PageableAssignmentHistoryOutput>> GetPageableEquipmentHistoryLookupAsync(
        ODataQueryOptions<OdpAssignmentsHistory> queryOptions)
    {
        var result = await _sender.Send(new GetPageableAssignmentsHistoryRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        var outputModel = PageableAssignmentHistoryOutput.Of(result);

        return Ok(outputModel);
    }

    /// <summary>
    /// Get equipment holders by equipment id
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="equipmentId">ID of equipment</param>
    /// <param name="queryOptions">Odata options for search</param>
    /// <returns>Holders of equipment</returns>
    [ProducesResponseType(typeof(PageableListOfItems<OdpEquipmentHolder>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("history")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpEquipmentHolder>>> GetPageableEquipmentHistoryAsync(
        int equipmentId,
        ODataQueryOptions<OdpEquipmentHolder> queryOptions)
    {
        var result = await _sender.Send(new GetPageableEquipmentHistoryRequest.Query(equipmentId, queryOptions),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Assign a user to equipment
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="equipmentId">ID of equipment</param>
    /// <param name="model">Model which storage issue date and id of holder</param>
    /// <returns>Holder model</returns>
    [ProducesResponseType(typeof(EquipmentHolderOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("assign")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<EquipmentHolderOutput>> AssignUserToEquipmentAsync(
        [FromRoute] int equipmentId,
        [FromBody] AssignEquipmentInput model)
    {
        var assignment = await _sender.Send(
            new AssignUserToEquipmentRequest.Command(
                equipmentId,
                model.UserId,
                model.ApproverId,
                model.IssueDate),
            HttpContext.RequestAborted);

        return Ok(EquipmentHolderOutput.Of(assignment));
    }

    /// <summary>
    /// UnAssign a user from equipment
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="equipmentId">ID of equipment</param>
    /// <param name="model">object with return date</param>
    /// <returns>Holder model</returns>
    [ProducesResponseType(typeof(EquipmentHolderOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("return")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<EquipmentHolderOutput>> UnAssignUserFromEquipmentAsync(
        [FromRoute] int equipmentId,
        [FromBody] ReturnEquipmentInput model)
    {
        var holderDto = await _sender.Send(
            new SetReturnDateToAssignRequest.Command(equipmentId, model.ReturnDate),
            HttpContext.RequestAborted);

        return Ok(EquipmentHolderOutput.Of(holderDto));
    }

    /// <summary>
    /// Remove history without returnDate about equipment
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="equipmentId">ID of equipment</param>
    /// <param name="historyId">ID of history</param>
    /// <returns>Holder model</returns>
    [ProducesResponseType(typeof(EquipmentHolderOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete("history/{historyId:int}")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult> RemoveHistoryEquipmentAsync([FromRoute] int equipmentId, [FromRoute] int historyId)
    {
        var deletedAssignment = await _sender.Send(
            new DeleteEquipmentHistoryRequest.Command(equipmentId, historyId),
            HttpContext.RequestAborted);

        return Ok(EquipmentHolderOutput.Of(deletedAssignment));
    }
}
