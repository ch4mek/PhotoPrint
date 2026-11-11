using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Core.Authentication.ApiKey;
using Modsen.Portal.Core.Exceptions;


#pragma warning disable OPENAI001

namespace Modsen.Portal.Utilities.Controllers.ChatGpt;

[AllowAnonymous]
[Area("[Shared] ChatGPT")]
[Route("api/chat-gpt")]
[ServiceFilter(typeof(ApiKeyHeaderAttribute))]
public class ChatGptController : Controller
{
    private readonly IChatGptService _chatGptService;

    public ChatGptController(IChatGptService chatGptService)
    {
        _chatGptService = chatGptService;
    }

    /// <summary>
    /// Analyze document
    /// Allowed roles: Anonymous
    /// </summary>
    /// <param name="input">Request</param>
    /// <param name="jiraToken">X-Jira-Token header</param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    [HttpPost("analyze-document")]
    public async Task<IActionResult> Compare(
        [FromHeader(Name = "X-Jira-Token")] string jiraToken,
        [FromBody] ChatGptInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentException.ThrowIfNullOrEmpty(input.FileUrl);
        ArgumentException.ThrowIfNullOrEmpty(input.Greetings);
        ArgumentException.ThrowIfNullOrEmpty(input.Description);

        var result = await _chatGptService.Compare(input.FileUrl, input.Greetings, input.Description, jiraToken);

        return Ok(result);
    }

    /// <summary>
    /// Analyze question
    /// Allowed roles: Anonymous
    /// </summary>
    /// <param name="input">Request</param>
    /// <returns></returns>
    /// <exception cref="AppException"></exception>
    [HttpPost("analyze-question")]
    public async Task<IActionResult> AnalyzeQuestion([FromBody] AnalyzeQuestionInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var result = await _chatGptService.AnalyzeQuestion(input.Question, input.UserEmail, input.Key, input.AssistantId, HttpContext.RequestAborted);
        return Ok(result);
    }
}
