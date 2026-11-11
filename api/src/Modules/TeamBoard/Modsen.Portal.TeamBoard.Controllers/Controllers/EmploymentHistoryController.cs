using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.EmploymentHistory.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Employment history")]
[Route("api/employment-history")]
internal class EmploymentHistoryController : BaseApiController
{
    private readonly ISender _sender;

    public EmploymentHistoryController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user's employment history
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">ID of a user to find an employment history for</param>
    /// <returns>Employment history</returns>
    [HttpGet]
    [ProducesResponseType(typeof(EmploymentHistoryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<EmploymentHistoryOutput>> GetEmploymentHistoryAsync([FromQuery] int userId)
    {
        var query = new GetEmploymentHistoryRequest.Query(userId);

        var result = await _sender.Send(query);

        return new EmploymentHistoryOutput(
            ContractEndDate: result.ContractEndDate,
            FirstProjectStartDate: result.FirstProjectStartDate,
            ProfessionalActivityStartDate: result.ProfessionalActivityStartDate,
            HireDate: result.HireDate,
            FireDate: result.FireDate,
            NdaStartDate: result.NdaStartDate,
            NdaEndDate: result.NdaEndDate,
            ShouldDisplayNda: result.ShouldDisplayNda
        );
    }

    /// <summary>
    /// Update user's employment history
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupoManager
    /// </summary>
    /// <param name="input">Data to update user's employment history</param>
    /// <returns>Updated employment history</returns>
    [HttpPatch]
    [ProducesResponseType(typeof(EmploymentHistoryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<ActionResult<EmploymentHistoryOutput>> UpdateEmploymentHistoryAsync(
        [FromBody] UpdateEmploymentHistoryInput input)
    {
        var command = new UpdateEmploymentHistoryRequest.Command(
            input.UserId,
            input.ProfessionalActivityStartDate,
            input.FireDate,
            input.NdaStartDate,
            input.NdaEndDate);

        var result = await _sender.Send(command);

        return Ok(new EmploymentHistoryOutput(
            ContractEndDate: result.ContractEndDate,
            FirstProjectStartDate: result.FirstProjectStartDate,
            ProfessionalActivityStartDate: result.ProfessionalActivityStartDate,
            HireDate: result.HireDate,
            FireDate: result.FireDate,
            NdaStartDate: result.NdaStartDate,
            NdaEndDate: result.NdaEndDate,
            ShouldDisplayNda: result.ShouldDisplayNda
        ));
    }
}
