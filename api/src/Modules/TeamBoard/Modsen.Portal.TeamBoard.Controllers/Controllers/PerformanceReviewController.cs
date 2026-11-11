using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Performance review")]
[Route("api/performance-review")]
internal class PerformanceReviewController : BaseApiController
{
    private readonly ISender _sender;

    public PerformanceReviewController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of users performance reviews
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <param name="issuedOnly">parameters for filtering users according to performance review status</param>
    /// <param name="overdueDaysCount">overdueDaysCount</param>
    /// <returns>A list of users performance reviews</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<UserPerformanceReviewOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserPerformanceReviewOutput>>> GetPageableUsersPerformanceReviewsAsync(
        [FromQuery] int overdueDaysCount,
        ODataQueryOptions<OdpTeamBoardUser> queryOptions,
        [FromQuery] bool issuedOnly = false)
    {
            var result = await _sender.Send(
                new GetUsersPerformanceReviewRequest.Query(overdueDaysCount, issuedOnly, queryOptions),
                HttpContext.RequestAborted);

            return Ok(result);
    }

    /// <summary>
    /// Create a performance review for user
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Model of a performance review</param>
    /// <returns>Created performance review</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreatePerformanceReviewOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<CreatePerformanceReviewOutput>> CreatePerformanceReviewAsync(
        [FromBody] CreatePerformanceReviewDto input)
    {
        var result = await _sender.Send(new CreatePerformanceReviewRequest.Command(input), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a list of user performance reviews
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <param name="overdueDaysCount">overdueDaysCount</param>
    /// <returns>A list of user performance reviews</returns>
    [HttpGet("user")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpPerformanceReview>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpPerformanceReview>>> GetPageablePerformanceReviewsByUserIdAsync(
        [FromQuery] int overdueDaysCount,
        ODataQueryOptions<OdpPerformanceReview> queryOptions)
    {
        var result = await _sender.Send(new GetPerformanceReviewsByUserIdRequest.Query(overdueDaysCount, queryOptions),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a lookup of performance reviews
    /// Allowed Roles : Authenticated User with ViewDashboard claim
    /// </summary>
    /// <param name="overdueDays"></param>
    /// <param name="upperDaysBorder"></param>
    /// <param name="departmentId">filter allowed for CEO, CTO</param>
    /// <returns></returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(PerformanceReviewLookupDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDashboardPolicy)]
    public async Task<ActionResult<PerformanceReviewLookupDto[]>> GetPerformanceReviewLookupAsync(
        [FromQuery] int overdueDays, [FromQuery] string? departmentId, [FromQuery] int upperDaysBorder)
    {
        var result = await _sender.Send(new GetPerformanceReviewLookupRequest.Query(overdueDays, upperDaysBorder, departmentId),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
