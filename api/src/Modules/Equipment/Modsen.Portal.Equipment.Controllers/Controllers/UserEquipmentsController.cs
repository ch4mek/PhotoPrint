using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;
using Modsen.Portal.Equipment.Requests.Users.Requests;
using Modsen.Portal.Equipment.Requests.Users.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Controllers.Controllers;

[Area("[Equipment] Users")]
internal class UserEquipmentsController : BaseApiController
{
    private readonly ISender _mediator;
    private readonly IEquipmentPortalSettingsAccessor _equipmentPortalSettingsAccessor;

    public UserEquipmentsController(
        ISender mediator,
        IEquipmentPortalSettingsAccessor equipmentPortalSettingsAccessor)
    {
        _mediator = mediator;
        _equipmentPortalSettingsAccessor = equipmentPortalSettingsAccessor;
    }

    /// <summary>
    /// Get user equipment
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="queryOptions">Odata options for search</param>
    /// <returns>List of user equipment</returns>
    [ProducesResponseType(typeof(List<UserEquipmentOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("/api/users/{userId:int}/equipment")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableUserEquipmentOutput>> GetUsersEquipmentsAsync(
        [FromRoute] int userId,
        ODataQueryOptions<OdpUserAssignments> queryOptions)
    {
        var result = await _mediator.Send(
            new GetUserEquipmentRequest.Query(userId, queryOptions),
            HttpContext.RequestAborted);

        var settings = await _equipmentPortalSettingsAccessor.GetSettings(HttpContext.RequestAborted);

        return PageableUserEquipmentOutput.Of(result, settings);
    }

    /// <summary>
    /// Get equipment users
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <returns>List of equipment users</returns>
    [ProducesResponseType(typeof(List<UserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("/api/equipment-users/lookup")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<List<UserLookupDto>>> GetUsersLookupAsync(
        [FromQuery] List<string> roles)
    {
        var result = await _mediator.Send(
            new GetUserLookupRequest.Query(roles),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
