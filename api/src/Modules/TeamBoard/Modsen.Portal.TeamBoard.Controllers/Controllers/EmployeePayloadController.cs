using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Payload.Requests;
using Modsen.Portal.TeamBoard.Requests.Payload.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Employee Payload")]
[Route("api/employee-payload")]
internal class EmployeePayloadController : BaseApiController
{
    private readonly ISender _sender;

    public EmployeePayloadController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get employees payload
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">Year for statistics</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="model">Filters</param>
    /// <returns>Calculated payload</returns>
    [HttpPost]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [Route("~/api/calendar/{year:int}/employees-payload")]
    public async Task<ActionResult<PayloadCoreDto>> GetEmployeesPayload([FromRoute] int year,
        [FromQuery] int? pageSize, [FromQuery] int? pageNumber,
        [FromBody] GetEmployeePayloadInput model)
    {
        var payload = await _sender.Send(new GetEmployeesPayload.Query
        {
            Year = year,
            PageSize = pageSize ?? 25,
            PageNumber = pageNumber ?? 0,
            Ordering = new PayloadOrderingCoreDto
            {
                Field = model.OrderingField ?? PayloadOrderingField.UserId,
                IsDescending = model.IsDescendingOrdering ?? false
            },
            Filter = new PayloadFilterCoreDto
            {
                ProjectName = model.ProjectName,
                DepartmentIds = model.DepartmentIds,
                ResourceGroupName = model.ResourceGroupName,
                UserId = model.UserId,
                UserFullName = model.UserFullName,
                Comment = model.Comment,
                NeedProject = model.NeedProject,
                Plan = model.Plan,
                SpecializationName = model.SpecializationName
            }
        });

        return Ok(payload);
    }

    /// <summary>
    /// Get total employees payload
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">Year for statistics</param>
    /// <param name="model">Filters</param>
    /// <returns>Calculated total</returns>
    [HttpPost]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [Route("~/api/calendar/{year:int}/totals-employees-payload")]
    public async Task<ActionResult<PayloadTotalHoursCoreDto[]>> GetTotalEmployeesPayload([FromRoute] int year,
        [FromBody] GetEmployeePayloadInput model)
    {
        var payload = await _sender.Send(new GetEmployeesPayloadTotals.Query
        {
            Year = year,
            Ordering = new PayloadOrderingCoreDto
            {
                Field = model.OrderingField ?? PayloadOrderingField.UserId,
                IsDescending = model.IsDescendingOrdering ?? false
            },
            Filter = new PayloadFilterCoreDto
            {
                ProjectName = model.ProjectName,
                DepartmentIds = model.DepartmentIds,
                ResourceGroupName = model.ResourceGroupName,
                UserId = model.UserId,
                UserFullName = model.UserFullName,
                Comment = model.Comment,
                NeedProject = model.NeedProject,
                Plan = model.Plan,
                SpecializationName = model.SpecializationName
            }
        });

        return Ok(payload);
    }

    /// <summary>
    /// Update user plan fte
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">Year for updated</param>
    /// <param name="month">Month for updated</param>
    /// <param name="userId">User id</param>
    /// <param name="input">Filter</param>
    /// <returns>Updated data</returns>
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [HttpPut("~/api/calendar/{year:int}/month/{month:int}/users/{userId:int}")]
    public async Task<ActionResult<PayloadUserFteRequestDto>> UpdateUserPayloadFteAsync(
        [FromRoute] int year, [FromRoute] int month, [FromRoute] int userId,
        [FromBody] UpdateUserPayloadFteInput input)
    {
        var command = new UpdateUserPayloadFte.Command(userId, year, month, input.Fte);
        var payload = await _sender.Send(command, HttpContext.RequestAborted);
        return Ok(payload);
    }

    /// <summary>
    /// Update user payload details
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Input data</param>
    /// <param name="userId">User id</param>
    /// <returns>Calculated total</returns>

    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [HttpPut("~/api/users/{userId:int}/payload-details")]
    public async Task<ActionResult> UpdateUserPayloadDetails(
        [FromRoute] int userId,
        [FromBody] UpdateUserPayloadDetailsInput input)
    {
        var command = new UpdateUserPayloadDetailsRequest.Command(userId, input.AvatarColor);

        await _sender.Send(command, HttpContext.RequestAborted);

        return NoContent();
    }

}
