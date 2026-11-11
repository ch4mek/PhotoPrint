using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

namespace Modsen.Portal.KnowledgeBase.Controllers.Controllers;

[Area("[Knowledge-base] Discounts")]
[Route("api/discounts")]
internal class DiscountsController : BaseApiController
{
    private readonly ISender _sender;

    public DiscountsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a discount
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="id">Discount id</param>
    /// <returns>The discount</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DiscountOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<DiscountOutput>> GetDiscountByIdAsync([FromRoute] int id)
    {
        var discountDto = await _sender.Send(new GetDiscountByIdRequest.Query(id), HttpContext.RequestAborted);

        var discountOutput = DiscountOutput.Of(discountDto);

        return Ok(discountOutput);
    }

    /// <summary>
    /// Get all discounts within requested discount category id
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="discountCategoryId">ID for discount category</param>
    /// <returns>List of DiscountOutput</returns>
    [HttpGet("~/api/discount-categories/{discountCategoryId:int}/discounts")]
    [ProducesResponseType(typeof(List<DiscountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<List<DiscountOutput>>> GetDiscountsByCategoryAsync([FromRoute] int discountCategoryId)
    {
        var discounts = await _sender.Send(
            new GetDiscountsByDiscountCategoryIdRequest.Query(discountCategoryId),
            HttpContext.RequestAborted);

        var discountOutputs = discounts.Select(DiscountOutput.Of).ToList();

        return Ok(discountOutputs);
    }

    /// <summary>
    /// Export discount to pdf
    /// Allowed Roles : CTO
    /// </summary>
    /// <returns>Pdf file</returns>
    [HttpGet("export")]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<IActionResult> ExportToPdfAsync()
    {
        var reportInBytes = await _sender.Send(new ExportDiscountsToPdfRequest.Query(), HttpContext.RequestAborted);

        return File(reportInBytes, PortalMediaTypeNames.Pdf, PortalSettingNames.DefaultDiscountReportName + PortalFileExtensionNames.Pdf);
    }

    /// <summary>
    /// Get a list of discount tags
    /// Allowed Roles : CTO
    /// </summary>
    /// <returns>A list of discount tags</returns>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(List<TagOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<List<TagOutput>>> GetDiscountTagsAsync()
    {
        var tagsDto = await _sender.Send(new GetDiscountTagsRequest.Query(), HttpContext.RequestAborted);

        var tagsOutput = tagsDto.Select(TagOutput.Of).ToList();

        return Ok(tagsOutput);
    }

    /// <summary>
    /// Get a list of available for discounts cities
    /// Allowed Roles : CTO
    /// </summary>
    /// <returns>A list of discount cities</returns>
    [HttpGet("cities")]
    [ProducesResponseType(typeof(List<CityOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<List<CityOutput>>> GetDiscountCitiesAsync()
    {
        var citiesDto = await _sender.Send(new GetAvailableDiscountCitiesRequest.Query(), HttpContext.RequestAborted);

        var citiesOutput = citiesDto.Select(CityOutput.Of).ToList();

        return Ok(citiesOutput);
    }

    /// <summary>
    /// Filter discounts by some condition
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="discountCategoryId">Discount category id for search</param>
    /// <param name="tagId">Tag id for search</param>
    /// <param name="name">Discount name for search</param>
    /// <param name="cityId">City id for search</param>
    /// <returns>List of DiscountsOutput</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<DiscountOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewExportDiscountsPolicy)]
    public async Task<ActionResult<List<DiscountOutput>>> SearchDiscountsAsync(
        [FromQuery] int? discountCategoryId,
        [FromQuery] int? tagId,
        [FromQuery] string? name,
        [FromQuery] int? cityId)
    {
        var discounts = await _sender.Send(
          new GetDiscountBySearchParametersRequest.Query(
              discountCategoryId,
              tagId,
              name,
              cityId),
          HttpContext.RequestAborted);

        var discountOutputs = discounts.Select(DiscountOutput.Of).ToList();

        return Ok(discountOutputs);
    }

    /// <summary>
    /// Create discount
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="createDiscountInput">Discount input model for entity creating</param>
    /// <returns>DiscountsGroupOutput</returns>
    [HttpPost]
    [ProducesResponseType(typeof(DiscountOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCardPolicy)]
    public async Task<ActionResult<DiscountOutput>> CreateDiscountsAsync([FromBody] CreateDiscountInput createDiscountInput)
    {
        var discount = await _sender.Send(new CreateDiscountRequest.Command
        {
            DiscountCategoryId = createDiscountInput.DiscountCategoryId,
            Name = createDiscountInput.Name,
            ImageUrl = createDiscountInput.ImageUrl,
            ImageFileId = createDiscountInput.ImageFileId,
            DiscountPercent = createDiscountInput.DiscountPercent,
            DiscountPercentTo = createDiscountInput.DiscountPercentTo,
            CompanyType = createDiscountInput.CompanyType,
            Website = createDiscountInput.Website,
            PhoneNumber = createDiscountInput.PhoneNumber,
            Location = createDiscountInput.Location,
            Text = createDiscountInput.Text,
            IsActive = createDiscountInput.IsActive,
            DiscountTagIds = createDiscountInput.DiscountTagIds,
            DiscountCityIds = createDiscountInput.DiscountCityIds,
            DiscountSocialMediaUrls = createDiscountInput.DiscountSocialMediaUrls
        },
        HttpContext.RequestAborted);

        var discountOutput = DiscountOutput.Of(discount);

        return Ok(discountOutput);
    }

    /// <summary>
    /// Update discount
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="updateDiscountInput">Discount input model for entity updating</param>
    /// <param name="id">Discount id</param>
    /// <returns>DiscountsGroupOutput</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DiscountOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCardPolicy)]
    public async Task<ActionResult<DiscountOutput>> UpdateDiscountsAsync(
        [FromRoute] int id,
        [FromBody] UpdateDiscountInput updateDiscountInput)
    {
        var discount = await _sender.Send(new UpdateDiscountRequest.Command
        {
            Id = id,
            DiscountCategoryId = updateDiscountInput.DiscountCategoryId,
            Name = updateDiscountInput.Name,
            ImageUrl = updateDiscountInput.ImageUrl,
            ImageFileId = updateDiscountInput.ImageFileId,
            DiscountPercent = updateDiscountInput.DiscountPercent,
            DiscountPercentTo = updateDiscountInput.DiscountPercentTo,
            CompanyType = updateDiscountInput.CompanyType,
            Website = updateDiscountInput.Website,
            PhoneNumber = updateDiscountInput.PhoneNumber,
            Location = updateDiscountInput.Location,
            Text = updateDiscountInput.Text,
            IsActive = updateDiscountInput.IsActive,
            DiscountTagIds = updateDiscountInput.DiscountTagIds,
            DiscountCityIds = updateDiscountInput.DiscountCityIds,
            DiscountSocialMediaUrls = updateDiscountInput.DiscountSocialMediaUrls
        },
        HttpContext.RequestAborted);

        var discountOutput = DiscountOutput.Of(discount);

        return Ok(discountOutput);
    }

    /// <summary>
    /// Updates discounts group and returns updated one
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="id">Discount id for deleting</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountCardPolicy)]
    public async Task<ActionResult> DeleteDiscountAsync([FromRoute] int id)
    {
        var discountId = await _sender.Send(new DeleteDiscountRequest.Command(id), HttpContext.RequestAborted);

        return Ok(discountId);
    }

    /// <summary>
    /// Update discounts order
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="discountOrdersInput">Discount orders input model</param>
    [HttpPut("order")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [Authorize(Policy = PortalPolicyNames.ManageDiscountOrderPolicy)]
    public async Task<ActionResult> UpdateDiscountsOrderAsync([FromBody] DiscountOrdersInput discountOrdersInput)
    {
        await _sender.Send(new UpdateDiscountOrderRequest.Command
        {
            Orders = discountOrdersInput.DiscountOrders
        }, HttpContext.RequestAborted);

        return Ok();
    }
}
