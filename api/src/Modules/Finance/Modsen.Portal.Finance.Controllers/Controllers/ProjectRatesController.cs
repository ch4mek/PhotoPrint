using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Controllers.Contracts.Output;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] ProjectRates")]
[Route("api/project-rates")]
internal sealed class ProjectRatesController : BaseApiController
{
    private readonly ISender _sender;

    public ProjectRatesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Adds a new rate
    /// Allowed Roles : CEO, CTO, users with ViewRates or ViewAllRates claims
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">Month of the year</param>
    /// <param name="pageSize">The page size. 25 by default</param>
    /// <param name="pageNumber">The pageNumber. starts with 0. 0 by default</param>
    /// <param name="model">options for searching and ordering rates</param>
    /// <returns>rates</returns>
    [HttpPost("{year:int}/{month:int}")]
    [ProducesResponseType(typeof(PageableListOfItems<UserProjectRatesOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewProjectRatesPolicy)]
    public async Task<ActionResult<PageableListOfItems<UserProjectRatesOutput>>> GetRateAsync([FromRoute] int year, [FromRoute] int month,
        [FromQuery] int? pageSize, [FromQuery] int? pageNumber,
        [FromBody] GetProjectRatesInput model)
    {
        var rates = await _sender.Send(new GetAllProjectRatesRequest.Query
        {
            Year = year,
            Month = month,
            PageSize = pageSize ?? 25,
            PageNumber = pageNumber ?? 0,
            Ordering = new FinanceOrderingCoreDto
            {
                Field = model.OrderingField ?? FinanceOrderingField.UserId,
                IsDescending = model.IsDescendingOrdering ?? false
            },
            Filter = new RatesFilterCoreDto
            {
                UserId = model.UserId,
                UserFullName = model.UserFullName,
                ProjectName = model.ProjectName,
                DepartmentIds = model.DepartmentIds,
                ClientName = model.ClientName,
                ClientLegalEntity = model.ClientLegalEntity,
            }
        }, HttpContext.RequestAborted);

        return Ok(rates);
    }

    /// <summary>
    /// Adds a new project rate
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <param name="input">Model of a new project rate</param>
    /// <returns>Created project rate</returns>
    /// <remarks>
    /// <c>RateFrom</c> is normalized to the first day of its month.
    /// For example, both <c>2026-06-01</c> and <c>2026-06-26</c> create a rate for June 2026,
    /// and the returned <c>RateFrom</c> value will be <c>2026-06-01</c>.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectRateCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectRatesPolicy)]
    public async Task<ActionResult<ProjectRateCoreDto>> CreateRateAsync([FromBody] CreateProjectRateInput input)
    {

        var command = new CreateProjectRateRequest.Command
        {
            UserId = input.UserId,
            ProjectId = input.ProjectId,
            HourlyRate = input.HourlyRate,
            CurrencyId = input.CurrencyId,
            RateFrom = input.RateFrom
        };

        var createdRate = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(createdRate);
    }

    /// <summary>
    /// Edites an existing project rate
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <param name="id">The id of an edited project rate</param>
    /// <param name="input">Model of an edited project rate</param>
    /// <returns>Edited project rate</returns>
    /// <remarks>
    /// <c>RateFrom</c> is normalized to the first day of its month.
    /// For example, both <c>2026-06-01</c> and <c>2026-06-26</c> create a rate for June 2026,
    /// and the returned <c>RateFrom</c> value will be <c>2026-06-01</c>.
    /// </remarks>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ProjectRateCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectRatesPolicy)]
    public async Task<ActionResult<ProjectRateCoreDto>> EditRateAsync(int id, [FromBody] EditProjectRateInput input)
    {
        var command = new EditProjectRateRequest.Command
        {
            RateId = id,
            HourlyRate = input.HourlyRate,
            CurrencyId = input.CurrencyId,
            RateFrom = input.RateFrom
        };

        var editedRate = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(editedRate);
    }

    /// <summary>
    /// Gets history of rate for user at the specified project
    /// Allowed Roles : CEO, CTO, users with ViewRates or ViewAllRates claims
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <param name="projectId">The id of the user</param>
    /// <returns>Project rates history</returns>
    [HttpGet("~/api/projects/{projectId}/users/{userId}/rates")]
    [ProducesResponseType(typeof(ProjectRateHistoryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewProjectRatesPolicy)]
    public async Task<ActionResult<ProjectRateHistoryOutput>> GetRateHistoryAsync(int userId, int projectId)
    {
        var command = new GetProjectRateHistoryRequest.Query(userId, projectId);

        var history = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(ProjectRateHistoryOutput.Of(history));
    }
}
