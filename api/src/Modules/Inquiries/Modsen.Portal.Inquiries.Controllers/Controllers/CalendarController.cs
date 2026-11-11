using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Calendar.Requests.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Calendar exclusions")]
[Route("api/calendar/exclusions")]
internal class CalendarController : BaseApiController
{
    private readonly ISender _sender;

    public CalendarController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all calendar exclusions
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>List of CalendarExclusionOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CalendarExclusionOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<CalendarExclusionCoreDto>>> GetAllExclusionsAsync(
        ODataQueryOptions<CalendarExclusionCoreDto> queryOptions)
    {
        var exclusions = await _sender.Send(new GetAllExclusionsRequest.Query(queryOptions), HttpContext.RequestAborted);

        return Ok(exclusions);
    }

    /// <summary>
    /// Get calendar name lookup
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>List calendar names</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CalendarNameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<CalendarNameDto>>> GetCalendarNameLookup()
    {
        var result = await _sender.Send(new GetCalendarNameLookup.Query(), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get calendar exclusion by id
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Calendar exclusion id</param>
    /// <returns>CalendarExclusionOutput</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CalendarExclusionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<CalendarExclusionOutput>> GetExclusionAsync([FromRoute] int id)
    {
        var exclusion = await _sender.Send(new GetExclusionByIdRequest.Query(id), HttpContext.RequestAborted);

        var output = CalendarExclusionOutput.Of(exclusion);

        return Ok(output);
    }

    /// <summary>
    /// Create calendar exclusion
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Calendar exclusion input model for entity creating</param>
    /// <returns>CalendarExclusionOutput</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CalendarExclusionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<CalendarExclusionOutput>> CreateExclusionAsync(
        [FromBody] CalendarExclusionInput input)
    {
        var exclusion =
            await _sender.Send(
                new CreateExclusionRequest.Command
                {
                    Name = input.Name, Date = input.Date, DateType = input.DateType, NewDate = input.NewDate, CountryId = input.CountryId
                }, HttpContext.RequestAborted);

        var output = CalendarExclusionOutput.Of(exclusion);

        return Ok(output);
    }

    /// <summary>
    /// Update calendar exclusion
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Calendar exclusion id for updating</param>
    /// <param name="input">Calendar exclusion input model for entity updating</param>
    /// <returns>CalendarExclusionOutput</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CalendarExclusionOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<CalendarExclusionOutput>> UpdateCalendarExclusionAsync(int id,
        [FromBody] CalendarExclusionUpdateInput input)
    {
        var exclusion =
            await _sender.Send(
                new UpdateExclusionRequest.Command(id)
                {
                    Name = input.Name, DateType = input.DateType, Date = input.Date, NewDate = input.NewDate, CountryId = input.CountryId
                }, HttpContext.RequestAborted);

        var output = CalendarExclusionOutput.Of(exclusion);

        return Ok(output);
    }

    /// <summary>
    /// Delete the calendar exclusion
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Calendar exclusion id for deleting</param>
    /// <returns>Deleted calendar exclusion id</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult> DeleteCalendarExclusionAsync([FromRoute] int id)
    {
        var output = await _sender.Send(new DeleteExclusionRequest.Command(id), HttpContext.RequestAborted);

        return Ok(output);
    }
}
