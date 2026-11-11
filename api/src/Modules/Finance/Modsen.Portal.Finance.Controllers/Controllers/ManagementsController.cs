using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Controllers.Contracts.Output;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] Managements")]
[Route("api/managements")]
internal sealed class ManagementsController : BaseApiController
{
    private readonly ISender _sender;

    public ManagementsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of managements.
    /// Allowed Roles : CEO, CTO, users with ViewManagementTab claim
    /// </summary>
    /// <param name="year">year</param>
    /// <param name="month">Month of the year</param>
    /// <param name="input">Model with search and sorting properties</param>
    /// <returns>Managements</returns>
    [HttpGet("{year:int}/{month:int}")]
    [ProducesResponseType(typeof(PageableManagementsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [Authorize(Policy = PortalPolicyNames.ViewManagementTabPolicy)]
    public async Task<ActionResult<PageableManagementsOutput>> GetManagements([FromRoute] int year, [FromRoute] int month,
        [FromQuery] GetManagementsInput input)
    {
        var managements = await _sender.Send(new GetManagementsRequest.Query
        {
            Year = year,
            Month = month,
            PageSize = input.PageSize ?? 25,
            PageNumber = input.PageNumber ?? 1,
            Ordering = new ManagementsOrderingCoreDto
            {
                Field = input.OrderingField ?? ManagementsOrderingField.UserId,
                IsDescendingOrdering = input.IsDescendingOrdering ?? false
            },
            Filter = new ManagementsFilterCoreDto
            {
                UserId = input.UserId,
                UserFullName = input.UserFullName,
                ProjectName = input.ProjectName,
                DepartmentIds = input.DepartmentIds,
                ClientName = input.ClientName,
                ClientLegalEntity = input.ClientLegalEntity,
            },
            ShouldCalculateColumnStatistics = input.ShouldCalculateColumnStatistics,
            ShouldCalculateMainStatistics = input.ShouldCalculateMainStatistics,
        }, HttpContext.RequestAborted);

        return Ok(PageableManagementsOutput.Of(managements));
    }
}
