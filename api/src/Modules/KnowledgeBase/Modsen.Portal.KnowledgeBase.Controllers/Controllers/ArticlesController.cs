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
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

namespace Modsen.Portal.KnowledgeBase.Controllers.Controllers;

[Area("[KnowledgeBase] Articles")]
[Route("api/articles")]
internal class ArticlesController : BaseApiController
{
    private readonly ISender _sender;

    public ArticlesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get article
    /// Allowed Claims : ViewWiki
    /// </summary>
    /// <param name="id">Article id</param>
    /// <returns>Article</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewWikiPolicy)]
    public async Task<ActionResult<ArticleOutput>> GetArticleByIdAsync([FromRoute] int id)
    {
        var article = await _sender.Send(
            new GetArticleByIdRequest.Query(
                Id: id),
            HttpContext.RequestAborted);

        var articleOutput = ArticleOutput.Of(article);

        return Ok(articleOutput);
    }

    /// <summary>                                                       
    /// Export article to pdf                              
    /// Allowed Claims : ManageArticle
    /// </summary>
    /// <returns>Pdf file</returns>
    [HttpGet("export")]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageArticlePolicy)]
    public async Task<IActionResult> ExportToPdfAsync()
    {
        var reportInBytes = await _sender.Send(new ExportArticlesToPdfRequest.Query(), HttpContext.RequestAborted);

        return File(reportInBytes, PortalMediaTypeNames.Pdf, PortalSettingNames.DefaultArticleReportName + PortalFileExtensionNames.Pdf);
    }

    /// <summary>
    /// Search articles
    /// Allowed Claims : ViewWiki
    /// </summary>
    /// <param name="groupId">Article group id for search</param>
    /// <param name="tagId">Tag id for search</param>
    /// <param name="name">Article name for search</param>
    /// <param name="keywords">Keywords for search</param>
    /// <returns>Articles</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ArticleOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewWikiPolicy)]
    public async Task<ActionResult<List<ArticleOutput>>> GetArticlesByKeywordsAsync(
        [FromQuery] int? groupId,
        [FromQuery] int? tagId,
        [FromQuery] string? name,
        [FromQuery] string? keywords)
    {
        var articles = await _sender.Send(
          new GetArticlesBySearchParameters.Query(
              groupId,
              tagId,
              name,
              keywords),
          HttpContext.RequestAborted);

        var articlesOutputs = articles.Select(ArticleOutput.Of).ToList();

        return Ok(articlesOutputs);
    }

    /// <summary>
    /// Get a list of article tags
    /// Allowed Claims : ViewWiki
    /// </summary>
    /// <returns>A list of article tags</returns>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(List<TagOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewWikiPolicy)]
    public async Task<ActionResult<List<TagOutput>>> GetArticleTagsAsync()
    {
        var tagsDto = await _sender.Send(new GetArticleTagsRequest.Query(), HttpContext.RequestAborted);

        var tagsOutput = tagsDto.Select(TagOutput.Of).ToList();

        return Ok(tagsOutput);
    }

    /// <summary>
    /// Create article
    /// Allowed Claims : ManageArticle
    /// </summary>
    /// <param name="input">Create article input model</param>
    /// <returns>Created Article</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageArticlePolicy)]
    public async Task<ActionResult<ArticleOutput>> CreateArticleAsync(
        [FromBody] CreateArticleInput input)
    {
        var article = await _sender.Send(
            new CreateArticleRequest.Command(
                GroupId: input.GroupId,
                Name: input.Name,
                Icon: input.Icon,
                Text: input.Text,
                Keywords: input.Keywords,
                IsActive: input.IsActive,
                IsAvailableOnMainScreen: input.IsAvailableOnMainScreen,
                Tags: input.Tags ?? []),
            HttpContext.RequestAborted);

        var articleOutput = ArticleOutput.Of(article);

        return Ok(articleOutput);
    }

    /// <summary>
    /// Update article
    /// Allowed Claims : ManageArticle
    /// </summary>
    /// <param name="id">Article id</param>
    /// <param name="input">Update article input model</param>
    /// <returns>Updated Article</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ArticleOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageArticlePolicy)]
    public async Task<ActionResult<ArticleOutput>> UpdateArticleAsync(
        [FromRoute] int id,
        [FromBody] UpdateArticleInput input)
    {
        var article = await _sender.Send(
            new UpdateArticleRequest.Command(
                Id: id,
                GroupId: input.GroupId,
                Name: input.Name,
                Icon: input.Icon,
                Text: input.Text,
                Keywords: input.Keywords,
                IsActive: input.IsActive,
                IsAvailableOnMainScreen: input.IsAvailableOnMainScreen,
                ArticleTags: input.ArticleTags),
            HttpContext.RequestAborted);

        var articleOutput = ArticleOutput.Of(article);

        return Ok(articleOutput);
    }

    /// <summary>
    /// Delete article
    /// Allowed Claims : ManageArticle
    /// </summary>
    /// <param name="id">Article id to delete</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageArticlePolicy)]
    public async Task<ActionResult> DeleteArticleAsync(
        [FromRoute] int id)
    {
        await _sender.Send(
            new DeleteArticleRequest.Command(
                Id: id),
            HttpContext.RequestAborted);

        return Ok();
    }
}
