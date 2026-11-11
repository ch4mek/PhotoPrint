using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] User agreements")]
[Route("api/agreements")]
internal class AgreementsController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public AgreementsController(ISender sender, IHostEnvironmentContext hostEnvironmentContext)
    {
        _sender = sender;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    /// <summary>
    /// Get all agreements
    /// Allowed Roles : all
    /// </summary>
    /// <returns>List of UserAgreementOutput</returns>
    [HttpGet("~/api/users/agreements")]
    [ProducesResponseType(typeof(List<UserAgreementOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserAgreementOutput>>> GetAllAgreementsAsync()
    {
        var agreements = await _sender.Send(new GetAllAgreementsRequest.Query(), HttpContext.RequestAborted);

        var output = agreements.Select(UserAgreementOutput.Of).ToList();

        return Ok(output);
    }

    /// <summary>
    /// Get user agreement history
    /// Allowed Roles : all
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <param name="year">Year number for filtration by year</param>
    /// <param name="queryOptions">OData filtration options</param>
    /// <returns>List of TransactionShortOutput</returns>
    [HttpGet("~/api/users/{userId:int}/history")]
    [ProducesResponseType(typeof(PageableListOfItems<UserAbsenceHistoryCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>>> GetUserAgreementHistoryAsync(
        [FromRoute] int userId,
        [FromQuery] int? year,
        ODataQueryOptions<UserAbsenceHistoryCoreDto> queryOptions)
    {
        var agreementHistory = await _sender.Send(
            new GetUserAgreementHistoryRequest.Query(userId, year, queryOptions),
            HttpContext.RequestAborted);
        return agreementHistory;
    }

    /// <summary>
    /// Start CreateAgreementTransactionsJob manually with specific date
    /// For testing purposes only
    /// Allowed Roles : CTO
    /// </summary>
    [HttpPost("~/api/agreements/create-transactions")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = PortalRoleNames.Cto)]
    public async Task<ActionResult> StartCreateAgreementTransactionsJobAsync(
        [FromQuery] DateOnly date, [FromQuery] int? userId)
    {
        if (!(_hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment()))
        {
            throw new ActionNotAllowedException();
        }

        var userIds = await _sender.Send(
            new CreateAgreementTransactionsRequest.Command()
            {
                TransactionTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                UserId = userId
            },
            HttpContext.RequestAborted);

        return Ok(userIds);
    }


    /// <summary>
    /// Start CreateDisposalTransactionsJob manually with specific date
    /// For testing purposes only
    /// Allowed Roles : CTO
    /// </summary>
    [HttpPost("create-reset-transactions")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = PortalRoleNames.Cto)]
    public async Task<ActionResult> StartCreateDisposalTransactionsJobAsync(
        [FromQuery] DateOnly date, [FromQuery] int? userId)
    {
        if (!(_hostEnvironmentContext.IsStagingEnvironment() ||
            _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment()))
        {
            throw new ActionNotAllowedException();
        }

        var users = await _sender.Send(
            new CreateDisposalTransactionsRequest.Command()
            {
                TransactionDate = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                UserId = userId
            },
            HttpContext.RequestAborted);

        return Ok(users);
    }

    /// <summary>
    /// Create user agreement correction transaction manually
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="agreementTemplateId">Inquiry agreement template id</param>
    /// <param name="input"></param>
    /// <returns>Correction transaction id</returns>
    [HttpPost("~/api/users/{userId:int}/agreements/{agreementTemplateId:int}/correction")]
    [ProducesResponseType(typeof(int[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefAndHRPolicyName)]
    public async Task<ActionResult> CreateAgreementTransactionsAsync(
        [FromRoute] int userId,
        [FromRoute] int agreementTemplateId,
        CorrectionAgreementTransactionInput input)
    {
        var userIds = await _sender.Send(
            new CreateCorrectionAgreementTransactionRequest.Command()
            {
                UserId = userId,
                AgreementTemplateId = agreementTemplateId,
                DaysCount = input.DaysCount,
                Description = input.Description,
            },
            HttpContext.RequestAborted);

        return Ok(userIds);
    }
}
