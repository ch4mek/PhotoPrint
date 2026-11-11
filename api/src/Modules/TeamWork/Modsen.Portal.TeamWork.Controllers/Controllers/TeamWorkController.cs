using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.ApiKey;
using Modsen.Portal.TeamWork.UseCases;
using Modsen.Portal.TeamWork.UseCases.GetDepartmentSnapshotByYear;
using Modsen.Portal.TeamWork.UseCases.GetSnapshotByYear;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;

namespace Modsen.Portal.TeamWork.Controllers.Controllers;

[Route("api/[controller]")]
[ApiController]
[Area("[TeamWork] Team Work")]
[ServiceFilter(typeof(ApiKeyHeaderAttribute))]
public class TeamWorkController : ControllerBase
{

    private readonly IMediator _sender;

    public TeamWorkController(IMediator sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get snapshot by year
    /// Allowed Roles : Auth
    /// </summary>
    /// <param name="teamWorkViewModel">model for creating snapshot</param>
    /// <returns>User child model</returns>
    [ProducesResponseType(typeof(UserChild), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("user")]
    public async Task<GetSnapshotByYearResponse<UserChild>> GetSnapshotByYearAsync(
        [FromBody] GetSnapshotByYearDto teamWorkViewModel)
    {
        ArgumentNullException.ThrowIfNull(teamWorkViewModel);
        var request = new GetSnapshotByYearQuery(teamWorkViewModel.StartYear,
            teamWorkViewModel.EndYear,
            teamWorkViewModel.DepartmentId,
            teamWorkViewModel.Projects,
            teamWorkViewModel.Clients);

        return await _sender.Send(request, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get department snapshot by year
    /// Allowed Roles : Auth
    /// </summary>
    /// <param name="teamWorkViewModel">model for creating snapshot</param>
    /// <returns>Department child model</returns>
    [ProducesResponseType(typeof(GetSnapshotByYearResponse<DepartmentChild>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("department")]
    public async Task<GetSnapshotByYearResponse<DepartmentChild>> GetDepartmentSnapshotByYearAsync(
        [FromBody] GetSnapshotByYearDto teamWorkViewModel)
    {
        ArgumentNullException.ThrowIfNull(teamWorkViewModel);
        var request = new GetDepartmentSnapshotByYearQuery(teamWorkViewModel.StartYear,
            teamWorkViewModel.EndYear,
            teamWorkViewModel.DepartmentId,
            teamWorkViewModel.Projects,
            teamWorkViewModel.Clients);

        return await _sender.Send(request, HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get users with inactive time tracking
    /// Allowed Roles : Auth
    /// </summary>
    /// <returns>A list of emails and names of filtered users</returns>
    [ProducesResponseType(typeof(List<UserEmail>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("users/inactive-time-tracking")]
    public async Task<IReadOnlyCollection<UserEmail>> GetUsersWithInactiveTimeTracking()
    {
        var response = await _sender.Send(new GetUsersWithInactiveTimeTrackingUseCase.Query(),
            HttpContext.RequestAborted);

        return response.Users;
    }

    /// <summary>
    /// Get users with inactive time tracking
    /// Allowed Roles : Auth
    /// </summary>
    /// <returns>A list of emails and names of filtered users</returns>
    [ProducesResponseType(typeof(List<ProjectsTrakedTimeOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost("project")]
    public async Task<IReadOnlyCollection<ProjectsTrakedTimeOutput>> GetProjectsSumTracked(
        [FromBody] IReadOnlyCollection<int>? projectIds)
    {

        var response = await _sender.Send(new GetProjectsTrackedTimeUseCase.Query(projectIds ?? []));
        return response.Projects;
    }
}
