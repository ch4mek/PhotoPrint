using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

namespace Modsen.Portal.KnowledgeBase.Controllers.Controllers;

[Area("[Knowledge-base] Discount-categories")]
[Route("api/discount-categories")]
internal class DiscountCategoriesController : BaseApiController
{
    private readonly ISender _sender;

    public DiscountCategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of discount categories
    /// Allowed Roles : CTO
    /// </summary>
    /// <returns>A list of discount categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<DiscountCategoryShortOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<List<DiscountCategoryShortOutput>>> GetDiscountCategoriesAsync()
    {
        var discountCategoriesDto = await _sender.Send(new GetAllDiscountCategoriesRequest.Query(), HttpContext.RequestAborted);

        var discountCategoriesOutput = discountCategoriesDto.Select(DiscountCategoryShortOutput.Of).ToList();

        return Ok(discountCategoriesOutput);
    }

    /// <summary>
    /// Get a discount category
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="id">Discount category id</param>
    /// <returns>The discount category</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DiscountCategoryShortOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<DiscountCategoryOutput>> GetDiscountCategoryByIdAsync([FromRoute] int id)
    {
        var discountCategoryDto = await _sender.Send(new GetDiscountCategoryByIdRequest.Query(id), HttpContext.RequestAborted);

        var discountCategoryOutput = DiscountCategoryOutput.Of(discountCategoryDto);

        return Ok(discountCategoryOutput);
    }

    /// <summary>
    /// Get a discount category tags
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="discountCategoryId">Discount category id</param>
    /// <returns>Tags for discount category</returns>
    [HttpGet("{discountCategoryId:int}/tags")]
    [ProducesResponseType(typeof(DiscountCategoryShortOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<DiscountCategoryOutput>> GetTagsByDiscountCategoryIdAsync([FromRoute] int discountCategoryId)
    {
        var tagsDto = await _sender.Send(new GetTagsByDiscountCategoryIdRequest.Query(discountCategoryId), HttpContext.RequestAborted);

        var tagsOutput = tagsDto.Select(TagOutput.Of).ToList();

        return Ok(tagsOutput);
    }

    /// <summary>
    /// Create discount category
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="createDiscountCategoryInput">Model with discount category properties</param>
    /// <returns>Created discount category</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DiscountCategoryShortOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCategoryPolicy)]
    public async Task<ActionResult<DiscountCategoryOutput>> CreateDiscountCategoryAsync([FromBody] DiscountCategoryInput createDiscountCategoryInput)
    {
        var discountCategoryDto = await _sender.Send(new CreateDiscountCategoryRequest.Command
        {
            Name = createDiscountCategoryInput.Name,
            DiscountCategoryTags = createDiscountCategoryInput.DiscountCategoryTags
        },
        HttpContext.RequestAborted);

        var discountCategoryOutput = DiscountCategoryOutput.Of(discountCategoryDto);

        return Ok(discountCategoryOutput);
    }

    /// <summary>
    /// Update discount category
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="updateDiscountCategoryInput">Model with new discount category properties</param>
    /// <param name="id">Discount category id</param>
    /// <returns>Updated discount category</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DiscountCategoryShortOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCategoryPolicy)]
    public async Task<ActionResult<DiscountCategoryOutput>> UpdateDiscountCategoryAsync(
        [FromRoute] int id,
        [FromBody] DiscountCategoryInput updateDiscountCategoryInput)
    {
        var discountCategoryDto = await _sender.Send(new UpdateDiscountCategoryRequest.Command
        {
            Id = id,
            Name = updateDiscountCategoryInput.Name,
            DiscountCategoryTags = updateDiscountCategoryInput.DiscountCategoryTags
        },
        HttpContext.RequestAborted);

        var discountCategoryOutput = DiscountCategoryOutput.Of(discountCategoryDto);

        return Ok(discountCategoryOutput);
    }

    /// <summary>
    /// Delete discount category
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="id">Discount category id</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCategoryPolicy)]
    public async Task<ActionResult> DeleteDiscountCategoryAsync([FromRoute] int id)
    {
        await _sender.Send(new DeleteDiscountCategoryRequest.Command(id), HttpContext.RequestAborted);

        return Ok();
    }

    /// <summary>
    /// Update discount categoris order
    /// Allowed Roles : CTO
    /// </summary>
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountOrderPolicy)]
    public async Task<ActionResult> UpdateDiscountCategoryOrderAsync([FromBody] DiscountCategoryOrdersInput discountCategoryOrdersInput)
    {
        await _sender.Send(new UpdateDiscountCategoryOrderRequest.Command
        {
            Orders = discountCategoryOrdersInput.DiscountCategoryOrders
        }, HttpContext.RequestAborted);

        return Ok();
    }
}
