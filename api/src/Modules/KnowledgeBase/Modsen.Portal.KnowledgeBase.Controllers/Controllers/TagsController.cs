using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

namespace Modsen.Portal.KnowledgeBase.Controllers.Controllers;

[Area("[Knowledge-base] Tags")]
[Route("api/tags")]
internal class TagsController : BaseApiController
{
    private readonly ISender _sender;

    public TagsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of tags
    /// Allowed Roles : CTO
    /// </summary>
    /// <returns>A list of tags</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<TagOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<List<TagOutput>>> GetTagsAsync()
    {
        var tagsDto = await _sender.Send(new GetAllTagsRequest.Query(), HttpContext.RequestAborted);

        var tagsOutput = tagsDto.Select(TagOutput.Of).ToList();

        return Ok(tagsOutput);
    }

    /// <summary>
    /// Get a tag
    /// Allowed Roles : CTO
    /// </summary>
    /// <param name="id">Tag id</param>
    /// <returns>The tag</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TagOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDiscountsPolicy)]
    public async Task<ActionResult<TagOutput>> GetTagByIdAsync([FromRoute] int id)
    {
        var tagDto = await _sender.Send(new GetTagByIdRequest.Query(id), HttpContext.RequestAborted);

        var tagOutput = TagOutput.Of(tagDto);

        return Ok(tagOutput);
    }
}
