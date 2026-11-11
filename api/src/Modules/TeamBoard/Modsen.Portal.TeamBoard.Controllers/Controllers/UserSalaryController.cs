using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] User salary")]
[Route("api/user-salary")]
internal class UserSalaryController : BaseApiController
{
    private readonly ISender _sender;

    public UserSalaryController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user salary details.
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="conversationId">Conversation id</param>
    /// <param name="userId">User id</param>
    /// <param name="queryOptions">Filters</param>
    /// <returns>List with salary details</returns>
    [HttpGet("~/api/users/{userId}/salaries")]
    [ProducesResponseType(typeof(PageableListOfItems<UserSalaryDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserSalaryDetailsDto>>> GetUserSalaryDetailsAsync(
        int userId,
        [FromQuery] int? conversationId,
        ODataQueryOptions<UserSalaryDetailsDto> queryOptions)
    {
        var query = new GetOdpUserSalaryDetailsByUserIdRequest.Query(conversationId, userId, queryOptions);

        var result = await _sender.Send(query);

        return result;
    }

    /// <summary>
    /// Get user salary details by user id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>salary details</returns>
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType(typeof(SalaryDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<SalaryDetailsDto>> GetUserSalaryDetailsByIdAsync(int id)
    {
        var query = new GetUserSalaryDetailsByIdRequest.Query(id);

        var salaryDetails = await _sender.Send(query);

        return salaryDetails;
    }

    /// <summary>
    /// Add user salary by user id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">model with properties need to create</param>
    /// <param name="source">creation source</param>
    /// <returns>Added salary and userId</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserSalaryOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<CreateUserSalaryOutput>> CreateUserSalaryDetailsAsync(
        [FromQuery] UserSalarySource? source,
        [FromBody] CreateSalaryDto input)
    {
        var command = new CreateUserSalaryRequest.Command(input, source);

        var salaryDto = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(CreateUserSalaryOutput.Of(salaryDto));
    }

    /// <summary>
    /// Update user salary by user id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">model with properties need to update</param>
    /// <returns>Updated salary and userId</returns>
    [HttpPatch]
    [ProducesResponseType(typeof(UpdateUserSalaryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<UpdateUserSalaryOutput>> UpdateUserSalaryByUserIdAsync(UpdateSalaryDto input)
    {
        var command = new UpdateUserSalaryRequest.Command(input);

        var salaryDto = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(UpdateUserSalaryOutput.Of(salaryDto));
    }

    /// <summary>
    /// Add many user salaries by user id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">model with properties need to create</param>
    /// <param name="source">creation source</param>
    /// <returns>Created list of user salary</returns>
    [HttpPost("salaries")]
    [ProducesResponseType(typeof(List<CreateUserSalaryOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<List<CreateUserSalaryOutput>>> CreateUserSalaryDetailsListAsync(
        [FromQuery] UserSalarySource? source,
        [FromBody] List<CreateSalaryDto> input)
    {
        var command = new CreateUserSalaryListUseCase.Command(input, source);

        var salaryDtos = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(salaryDtos.ConvertAll(CreateUserSalaryOutput.Of));
    }

    /// <summary>
    /// Update many user salaries by user id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">models with properties need to update</param>
    /// <returns>Updated list of user salary</returns>
    [HttpPatch("salaries")]
    [ProducesResponseType(typeof(List<UpdateUserSalaryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<List<UpdateUserSalaryOutput>>> UpdateUserSalariesByUserIdAsync(
        List<UpdateSalaryDto> input)
    {
        var command = new UpdateUserSalaryListUseCase.Command(input);

        var salaryDtos = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(salaryDtos.ConvertAll(UpdateUserSalaryOutput.Of));
    }
}
