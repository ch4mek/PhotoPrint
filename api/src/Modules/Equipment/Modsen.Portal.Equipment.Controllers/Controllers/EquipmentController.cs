using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;

namespace Modsen.Portal.Equipment.Controllers.Controllers;

[Area("[Equipment] Equipment")]
internal class EquipmentController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IClockService _clock;
    private readonly IEquipmentPortalSettingsAccessor _settingsAccessor;

    public EquipmentController(
        ISender sender,
        IClockService clock,
        IEquipmentPortalSettingsAccessor settingsAccessor)
    {
        _sender = sender;
        _clock = clock;
        _settingsAccessor = settingsAccessor;
    }

    /// <summary>
    /// Get a list equipment
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering and paging</param>
    /// <returns>A list of equipment</returns>
    [ProducesResponseType(typeof(PageableEquipmentsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<PageableEquipmentsOutput>> GetPageableEquipmentsAsync(
        ODataQueryOptions<OdpEquipment> queryOptions)
    {
        var pageableEquipments = await _sender.Send(
            new GetEquipmentsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        var outputModel = PageableEquipmentsOutput.Of(pageableEquipments);

        return Ok(outputModel);
    }

    /// <summary>
    /// Get filters for Equipments
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <returns>A list of filters for Equipment</returns>
    [ProducesResponseType(typeof(PageableEquipmentsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("availableFilters")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<AvailableFilterOptionsOutput>> GetEquipmentsFiltersAsync()
    {
        var availableFilterOptions = await _sender.Send(new GetEquipmentFiltersRequest.Query());

        return Ok(AvailableFilterOptionsOutput.Of(availableFilterOptions));
    }

    /// <summary>
    /// Save a list of equipment to Csv or Excel file
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging and choosing format of file</param>
    /// <param name="format">parameter for choosing a type of file</param>
    /// <returns>void</returns>
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("file/{format}")]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult> GetEquipmentsByFileAsync(ODataQueryOptions<OdpEquipment> queryOptions, DocumentType format)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var fileContents = await _sender.Send(new GetEquipmentsByFileRequest.Query(queryOptions, format),
            cancellationToken);

        var settings = await _settingsAccessor.GetSettings(cancellationToken);
        return format switch
        {
            DocumentType.Excel => File(
                fileContents,
                PortalMediaTypeNames.ExcelX,
                $"{settings.EquipmentsFileName}_{_clock.UtcNow.Date.ToString(settings.FileExportDateFormat, CultureInfo.InvariantCulture)}{PortalFileExtensionNames.Xlsx}"),
            _ => throw new InvalidDataAppException(PortalMessages.UnsupportedFileFormat)
        };
    }

    /// <summary>
    /// Get equipment by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="id">id of the equipment</param>
    /// <returns>Equipment model</returns>
    [ProducesResponseType(typeof(EquipmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("{id:int}", Name = nameof(GetEquipmentAsync))]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrHeadOrGmOrSysAdminPolicy)]
    public async Task<ActionResult<EquipmentOutput>> GetEquipmentAsync(int id)
    {
        var result = await _sender.Send(new GetEquipmentByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(EquipmentOutput.Of(result));
    }

    /// <summary>
    /// Create new equipment
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="model">model of a new equipment</param>
    /// <returns>Created equipment model</returns>
    [ProducesResponseType(typeof(EquipmentOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<EquipmentOutput>> CreateEquipmentAsync([FromBody] CreateEquipmentRequestDto model)
    {
        var request = new CreateEquipmentRequest.Command(model);

        var result = await _sender.Send(request, HttpContext.RequestAborted);

        return CreatedAtAction(
            nameof(GetEquipmentAsync),
            new { result.Id },
            EquipmentOutput.Of(result));
    }

    /// <summary>
    /// Update equipment
    /// Allowed Roles : CEO, CTO, SystemAdministrator
    /// Allowed Claims : ManageAllEquipment
    /// </summary>
    /// <param name="model">model of updating equipment</param>
    /// <returns>Updated equipment model</returns>
    [ProducesResponseType(typeof(EquipmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Authorize(Policy = PortalPolicyNames.ManageAllEquipmentOrChiefOrSysAdminPolicy)]
    public async Task<ActionResult<UpdatedEquipmentOutput>> UpdateEquipmentAsync(
        [FromBody] UpdateEquipmentRequestDto model)
    {
        var request = new UpdateEquipmentRequest.Command(model);

        var equipmentDto = await _sender.Send(request, HttpContext.RequestAborted);

        return Ok(UpdatedEquipmentOutput.Of(equipmentDto));
    }
}
