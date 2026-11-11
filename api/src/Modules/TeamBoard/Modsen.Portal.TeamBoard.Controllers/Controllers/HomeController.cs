using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.UserEventsKey;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Home.Requests;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Home")]
[Route("api/home")]
internal class HomeController : BaseApiController
{
    private readonly ISender _sender;

    public HomeController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get upcoming events
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <returns>List of user upcoming events</returns>
    [HttpGet("{id:int}/events")]
    [ProducesResponseType(typeof(List<UpcomingEventOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy)]
    public async Task<ActionResult<List<UpcomingEventOutput>>> GetUserUpcomingEvents(int id,
        [FromQuery] int overdueDaysCount)
    {
        var upcomingEvents = await _sender.Send(
            new GetUserUpcomingEventsRequest.Query(id, overdueDaysCount),
            HttpContext.RequestAborted);

        return Ok(upcomingEvents.Select(UpcomingEventOutput.Of));
    }

    /// <summary>
    /// Get all employees birthdays
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="keyword">Keyword used for search by username</param>
    /// <param name="month">Month of birthdays</param>
    /// <returns>List of all birthdays</returns>
    [HttpGet("birthdays")]
    [ProducesResponseType(typeof(List<MonthWithBirthdaysDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy)]
    public async Task<ActionResult<List<MonthWithBirthdaysDto>>> GetAllBirthdays(
        [FromQuery] string? keyword = null,
        [FromQuery] int? month = null)
    {
        var birthdays = await _sender.Send(new GetAllBirthdaysRequest.Query(keyword, month),
            HttpContext.RequestAborted);

        return Ok(birthdays);
    }

    /// <summary>
    /// Get employee birthdays by date
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <returns>List of birthdays for date</returns>
    [HttpGet("birthdays/{date}")]
    [ProducesResponseType(typeof(List<UserBirthdayShortOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmEmployeeSysAdminAndViewHomeOnlyPolicy)]
    public async Task<ActionResult<List<UserBirthdayShortOutput>>> GetBirthdaysByDate(DateOnly date)
    {
        var birthdays = await _sender.Send(new GetBirthdaysByDateRequest.Query(date),
            HttpContext.RequestAborted);

        return Ok(birthdays.ConvertAll(UserBirthdayShortOutput.Of));
    }

    /// <summary>
    /// Get employee probation period ends
    /// Allowed Roles : Authenticated User
    /// Allowed Claims : ViewDashboard
    /// </summary>
    /// <returns>List of probation period ends</returns>
    [HttpGet("~/api/users/probation-periods-ends")]
    [ProducesResponseType(typeof(List<ProbationPeriodEndDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDashboardPolicy)]
    public async Task<ActionResult<List<ProbationPeriodEndDto>>> GetProbationPeriodsEnd([FromQuery] string? departmentId)
    {
        var probationPeriods = await _sender.Send(new GetProbationPeriodEndRequest.Query(departmentId),
            HttpContext.RequestAborted);

        return Ok(probationPeriods);
    }

    /// <summary>
    /// Get employee probation period ends 
    /// Access via API key 
    /// </summary>
    /// <param name="date">Probation period end date</param>
    /// <returns></returns>
    [HttpGet("~/api/users/probation-periods")]
    [ServiceFilter(typeof(UserEventsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(List<UserProbationPeriodEndDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserProbationPeriodEndDto>>> GetUserProbationPeriodsEnd([FromQuery] DateOnly? date)
    {
        var users = await _sender.Send(new GetUserProbationPeriodEndsRequest.Query(date),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get employees today anniversaties 
    /// Access via API key 
    /// </summary>
    /// <returns>List of annivarsaries of active users</returns>
    [HttpGet("~/api/users/anniversaries/today")]
    [ServiceFilter(typeof(UserEventsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(List<UserAnniversaryDateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserAnniversaryDateDto>>> GetUsersAnniversariesTodayAsync()
    {
        var users = await _sender.Send(new GetUsersAnniversariesRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get employees anniversaties by month
    /// Access via API key 
    /// </summary>
    /// <param name="month"></param>
    /// <returns>List of annivarsaries of active users</returns>
    [HttpGet("~/api/users/anniversaries/month/{month:int}")]
    [ServiceFilter(typeof(UserEventsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(List<UserAnniversaryDateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserAnniversaryDateDto>>> GetUsersAnniversaryByMonthAsync(int month)
    {
        var users = await _sender.Send(new GetUsersAnniversariesRequest.Query(MonthNumber: month),
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Get employee birthdays by date
    /// Access via API key 
    /// </summary>
    /// <returns>List of birthdays for date</returns>
    [HttpGet("birthdays/today")]
    [ServiceFilter(typeof(UserEventsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(List<UserBirthdayInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserBirthdayInfoDto>>> GetTodayBirthdaysAsync()
    {
        var birthdays = await _sender.Send(new GetBirthdaysByDateRequest.Query(DateOnly.FromDateTime(DateTime.Today)));

        return Ok(birthdays.ConvertAll(UserInfoShortOutput.Of));
    }

    /// <summary>
    /// Access employees related events (such like: birthdays and hiring anniversaries)
    /// Allowed Roles : Managers
    /// </summary>
    /// <returns>List of birthdays for date</returns>
    [HttpGet("employees-events")]
    [ProducesResponseType(typeof(ICollection<EmployeeEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDashboardPolicy)]
    public async Task<ActionResult<ICollection<EmployeeEventDto>>> GetEmployeesEvents([FromQuery] string? departmentId)
    {
        var employeeEvents = await _sender.Send(new GetEmployeesEventsRequest.Query
        {
            DepartmentId = departmentId
        });

        return Ok(employeeEvents);
    }
}
