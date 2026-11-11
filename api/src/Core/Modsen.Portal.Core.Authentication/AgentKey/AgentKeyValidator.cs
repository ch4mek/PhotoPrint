using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.AgentKey;

internal sealed class AgentKeyValidator : IAgentKeyValidator
{
    private readonly string? _agentApiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AgentKeyValidator(string? agentApiKey, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentException.ThrowIfNullOrEmpty(agentApiKey);
        _agentApiKey = agentApiKey;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsValidProvidedKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        return httpContext.Request.Headers.TryGetValue("X-Agent-Api-Key", out var values) &&
            values.Count != 0 &&
            values.Contains(_agentApiKey);
    }
}
