using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ExpenseCategories.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] Expense categories")]
[Route("api/expense-categories")]
internal sealed class ExpenseCategoriesController : BaseApiController
{
    private readonly ISender _sender;

    public ExpenseCategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all expense categories
    /// Allowed Roles : CEO, CTO, users with ManageExpenses claim
    /// </summary>
    /// <returns>A list of all not-deleted expense categories names</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<List<string>>> GetExpenseCategoriesLookup()
    {
        var currencies = await _sender.Send(
            new ExpenseCategoriesLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(currencies);
    }

    /// <summary>
    /// Adds a new expense category
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <param name="input">Model of a new expense category</param>
    /// <returns>Created expense category</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseCategoryCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpenseCategoryCoreDto>> CreateExpenseCategoryAsync([FromBody] CreateExpenseCategoryInput input)
    {

        var command = new CreateExpenseCategoryRequest.Command(input.CategoryName);        

        var created = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(created);
    }

    /// <summary>
    /// Edits an existing expense category
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <param name="id">id of an edited expense category</param>
    /// <param name="input">Model of an edited Expense category</param>
    /// <returns>Edited expense category</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ExpenseCategoryCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpenseCategoryCoreDto>> EditExpenseCategoryAsync(int id, [FromBody] EditExpenseCategoryInput input)
    {

        var command = new EditExpenseCategoryRequest.Command(id, input.CategoryName);

        var created = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(created);
    }

    /// <summary>
    /// Deletes an existing expense category
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <param name="id">id of a deleting expense category</param>
    /// <returns>Edited expense category</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ExpenseCategoryCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpenseCategoryCoreDto>> DeleteExpenseCategoryAsync(int id)
    {

        var command = new DeleteExpenseCategoryRequest.Command(id);

        var created = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(created);
    }
}
