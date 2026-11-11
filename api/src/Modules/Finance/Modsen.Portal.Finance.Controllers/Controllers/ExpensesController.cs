using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Controllers.Contracts.Input;
using Modsen.Portal.Finance.Requests.Expenses.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] Expenses")]
[Route("api/expenses")]
internal sealed class ExpensesController : BaseApiController
{
    private readonly ISender _sender;

    public ExpensesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets expences entries for certain year
    /// Allowed Roles : CEO, CTO, users with ManageExpenses claim
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="pageSize">The page size. 25 by default</param>
    /// <param name="pageNumber">The pageNumber. starts with 0. 0 by default</param>
    /// <param name="model">options for searching and ordering expences</param>
    /// <returns>Expenses entries</returns>
    [HttpPost("{year:int}")]
    [ProducesResponseType(typeof(ExpensesTabPageCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpensesTabPageCoreDto>> GetExpensesAsync(
        [FromRoute] int year, [FromQuery] int? pageSize, [FromQuery] int? pageNumber,
        [FromBody] GetExpensesInput model)
    {
        var command = new GetExpenseRequest.Query
        {
            Year = year,
            PageSize = pageSize ?? 25,
            PageNumber = pageNumber ?? 0,
            Ordering = new ExpenseOrderingCoreDto
            {
                Field = model.OrderingField ?? ExpenseOrderingField.OrderWeight,
                IsDescending = model.IsDescendingOrdering ?? false
            },
            Filter = new ExpenseFilterCoreDto
            {
                CategoryName = model.CategoryName
            }
        };

        var expenses = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(expenses);
    }

    /// <summary>
    /// Adds a new expense entry
    /// Allowed Roles : CEO, CTO, users with ManageExpenses claim
    /// </summary>
    /// <param name="input">Model of a new expense entry</param>
    /// <returns>Created expense entry</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpenseCoreDto>> CreateExpenseEntryAsync([FromBody] CreateExpenseInput input)
    {
        var command = new CreateExpenseRequest.Command
        {
            ExpenseCategoryId = input.ExpenseCategoryId,
            Year = input.Year,
            MonthOfYear = input.MonthOfYear,
            ExpenseAmount = input.ExpenseAmount,
        };

        var created = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(created);
    }

    /// <summary>
    /// Updates an existing expense entry
    /// Allowed Roles : CEO, CTO, users with ManageExpenses claim
    /// </summary>
    /// <param name="id">The id of an updating expense entry</param>
    /// <param name="input">Model of an updated expense entry</param>
    /// <returns>Updated expense entry</returns>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ExpenseCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageExpensesPolicy)]
    public async Task<ActionResult<ExpenseCoreDto>> EditExpenseEntryAsync(int id, [FromBody] UpdateExpenseInput input)
    {
        var command = new UpdateExpenseRequest.Command(id, input.ExpenseAmount);

        var updated = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(updated);
    }
}
