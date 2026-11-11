using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.Requests.Departments.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Department")]
public class DepartmentsController : BaseApiController
{
    private readonly ISender _sender;

    public DepartmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all department
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager, SystemAdministrator
    /// </summary>
    /// <returns>list of departments</returns>
    [ProducesResponseType(typeof(DepartmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName)]
    public async Task<ActionResult<List<DepartmentOutput>>> GetAll()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var departmentDtos = await _sender.Send(new GetDepartmentsRequest.Query(), cancellationToken);

        var output = departmentDtos.Select(dto =>
                new DepartmentOutput(
                    dto.DepartmentName,
                    dto.Groups.Select(g => new DepartmentGroupOutput
                    {
                        GroupManagerEmail = g.GroupManagerEmail,
                        Id = g.Id,
                        Name = g.Name
                    }).ToList(),
                    dto.HeadOfDepartmentId)).ToList();

        return Ok(output);
    }
}
