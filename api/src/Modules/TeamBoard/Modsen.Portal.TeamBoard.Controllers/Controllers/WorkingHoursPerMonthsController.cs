using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Calendar.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Working Hours Per Months")]
[Route("api/working-hours-per-months")]
internal class WorkingHoursPerMonthsController : BaseApiController
{
    private readonly ISender _sender;

    public WorkingHoursPerMonthsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get working hours per months of year
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">Year to get working hours per months of</param>
    /// <returns>Working hours per months entries of year with total hours of year per country</returns>
    [HttpGet]
    [ProducesResponseType(typeof(WorkingHoursPerMonthsSheetOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<WorkingHoursPerMonthsSheetOutput>> GetWorkingHoursPerMonthsByYearAsync([FromQuery] int year)
    {
        var result = await _sender.Send(new GetWorkingHoursPerMonthsByYearRequest.Query(year),
                HttpContext.RequestAborted);

        return Ok(WorkingHoursPerMonthsSheetOutput.Of(result));
    }

    /// <summary>
    /// Create working hours per month entry
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Parameter to create working hours per month entry</param>
    /// <returns>Created working hours per month entry</returns>
    [HttpPost]
    [ProducesResponseType(typeof(WorkingHoursPerMonthOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<WorkingHoursPerMonthOutput>> CreateWorkingHoursPerMonthAsync([FromBody] CreateWorkingHoursPerMonthInput input)
    {
        var command = new CreateWorkingHoursPerMonthRequest.Command(input.Year, input.MonthOfYear, input.CountryId,
                input.Hours);
        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new WorkingHoursPerMonthOutput
        {
            Id = result.Id,
            Year = result.Year,
            MonthOfYear = result.MonthOfYear,
            CountryId = result.CountryId,
            Hours = result.Hours
        });
    }

    /// <summary>
    /// Update working hours per months of year
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Parameters to update working hours per month entry</param>
    /// <returns>Updated working hours per month entry</returns>
    [HttpPut]
    [ProducesResponseType(typeof(WorkingHoursPerMonthOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<WorkingHoursPerMonthOutput>> UpdateWorkingHoursPerMonthAsync([FromBody] UpdateWorkingHoursPerMonthInput input)
    {
        var result = await _sender.Send(new UpdateWorkingHoursPerMonthRequest.Command(input.Id, input.Hours),
                HttpContext.RequestAborted);

        return Ok(new WorkingHoursPerMonthOutput
        {
            Id = result.Id,
            Year = result.Year,
            MonthOfYear = result.MonthOfYear,
            CountryId = result.CountryId,
            Hours = result.Hours
        });
    }
}
