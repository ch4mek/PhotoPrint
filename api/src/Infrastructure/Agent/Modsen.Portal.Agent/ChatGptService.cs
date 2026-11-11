using System.ClientModel;
using System.Net.Http.Headers;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Agent.Contract;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using OpenAI.Assistants;
using OpenAI.Chat;

#pragma warning disable OPENAI001

namespace Modsen.Portal.Agent;

public sealed class ChatGptService : IChatGptService
{
    private readonly IConfiguration _configuration;
    private readonly IIdentityContract _identityContract;
    private readonly ILogger<ChatGptService> _logger;

    public ChatGptService(IConfiguration configuration,
        IIdentityContract identityContract,
        ILogger<ChatGptService> logger)
    {
        _configuration = configuration;
        _identityContract = identityContract;
        _logger = logger;
    }

    public async Task<IEnumerable<string>?> Compare(string fileUrl, string greetings, string description, string jiraToken)
    {
        using var httpClient = new HttpClient();

        if (!jiraToken.IsNullOrWhiteSpace())
        {
            httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", jiraToken);
        }

        var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);

        var contentType = response.Content.Headers.ContentType?.MediaType;
        if(contentType != PortalMediaTypeNames.Word &&
            contentType != PortalMediaTypeNames.WordX)
        {
            throw new ExternalUnauthorizedException();
        }

        var document = await response.Content.ReadAsByteArrayAsync();

        using var memoryStream = new MemoryStream(document);
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(greetings);
        stringBuilder.AppendLine();
        stringBuilder.Append("Описание: ");
        stringBuilder.AppendLine(description);
        stringBuilder.AppendLine();
        stringBuilder.Append("CV: ");
        stringBuilder.AppendLine(ExtractTextFromDocx(memoryStream));

        var apiKey = _configuration.GetConnectionString("ChatGpt");
        var client = new ChatClient("gpt-4o", apiKey);
        var completion = await client.CompleteChatAsync(stringBuilder.ToString());

        return completion.Value.Content.Select(x => x.Text);
    }

    public async Task<string?> AnalyzeQuestion(string question, string? userEmail, string? key, string? assistantId, CancellationToken cancellationToken)
    {
        var (apiKey, hrAssistantId) = GetCredentials(key, assistantId);
        var assistantClient = new AssistantClient(apiKey);
        var threadOptions = new ThreadCreationOptions
        {
            InitialMessages = { question }
        };
        await assistantClient.GetAssistantAsync(hrAssistantId, cancellationToken);

        var savedThreadId = userEmail.IsNullOrEmpty()
            ? null
            : await _identityContract.GetThreadId(userEmail, hrAssistantId, cancellationToken);

        ClientResult<ThreadRun>? threadClientResult = null;
        if (!savedThreadId.IsNullOrEmpty())
        {
            try
            {
                var thread = await assistantClient.GetThreadAsync(savedThreadId, cancellationToken);
                if (thread != null)
                {
                    threadClientResult = await assistantClient.CreateRunAsync(savedThreadId, hrAssistantId,
                        new RunCreationOptions { AdditionalMessages = { question } }, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not run thread using saved ThreadID");
            }
        }

        if (threadClientResult == null)
        {
            threadClientResult = await assistantClient.CreateThreadAndRunAsync(hrAssistantId, threadOptions, cancellationToken: cancellationToken);

            if (!userEmail.IsNullOrEmpty())
            {
                await _identityContract.SaveThreadId(userEmail, threadClientResult?.Value.ThreadId, hrAssistantId, cancellationToken);
            }
        }

        if (threadClientResult is { Value: not null })
        {
            var threadRun = threadClientResult.Value;
            do
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                threadRun = await assistantClient.GetRunAsync(threadRun.ThreadId, threadRun.Id, cancellationToken);
            } while (!threadRun.Status.IsTerminal);

            var responseMessages = assistantClient.GetMessages(threadRun.ThreadId, cancellationToken: cancellationToken);
            var messages = responseMessages
                .Where(x => x.Role == MessageRole.Assistant)
                .OrderByDescending(x => x.CreatedAt)
                .Take(1)
                .SelectMany(message => message.Content
                    .Select(messageContent => messageContent.TextAnnotations.Aggregate(
                        seed: messageContent.Text,
                        func: (current, textAnnotation) => current.Replace(textAnnotation.TextToReplace, string.Empty,
                            StringComparison.InvariantCulture))
                    )
                )
                .Where(message => message != question)
                .Distinct();
            var responseText = string.Join(Environment.NewLine, messages);

            return responseText;
        }

        return string.Empty;
    }

    private (string ApiKey, string AssistantId) GetCredentials(string? key, string? assistantId)
    {
        if (!key.IsNullOrEmpty() && !assistantId.IsNullOrEmpty())
        {
            return (ApiKey: key, AssistantId: assistantId);
        }

        var apiKeyConfigured = _configuration.GetConnectionString("ChatGptHr");
        var assistantIdConfigured = _configuration.GetConnectionString("ChatGptHrAssistant");

        ArgumentException.ThrowIfNullOrEmpty(apiKeyConfigured);
        ArgumentException.ThrowIfNullOrEmpty(assistantIdConfigured);

        return (ApiKey: apiKeyConfigured, AssistantId: assistantIdConfigured);
    }

    public async Task<string> CreateThreadForUser(CancellationToken cancellationToken)
    {
        var apiKey = _configuration.GetConnectionString("ChatGptHr");
        var assistantId = _configuration.GetConnectionString("ChatGptHrAssistant");
        var assistantClient = new AssistantClient(apiKey);

        var createResult = await assistantClient
            .CreateThreadAndRunAsync(
            assistantId,
            new ThreadCreationOptions(),
            cancellationToken: cancellationToken);

        return createResult?.Value?.ThreadId ?? throw new InvalidOperationException("Thread creation failed");
    }

    public string GetAssistantId()
    {
        return _configuration.GetConnectionString("ChatGptHrAssistant")!;
    }

    private static string ExtractTextFromDocx(MemoryStream ms)
    {
        using var wordDoc = WordprocessingDocument.Open(ms, false);

        var body = wordDoc.MainDocumentPart!.Document.Body;
        var text = new StringBuilder();

        ExtractTextFromElement(body!, text);

        return text.ToString();
    }

    private static void ExtractTextFromElement(OpenXmlElement element, StringBuilder text)
    {
        if (element is Run run)
        {
            var runText = run.Elements<Text>().FirstOrDefault()?.Text;

            if (runText != null)
            {
                text.AppendLine(runText);
            }
        }

        foreach (var child in element.Elements())
        {
            ExtractTextFromElement(child, text);
        }
    }
}
