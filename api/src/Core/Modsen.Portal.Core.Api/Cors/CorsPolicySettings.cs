namespace Modsen.Portal.Core.Api.Cors;

public sealed class CorsPolicySettings
{
    public string? PolicyName { get; init; }
    public IReadOnlyCollection<string> AllowedOrigins { get; init; } = [];
    public bool AllowAnyHeader { get; init; }
    public IReadOnlyCollection<string> AllowedHeaders { get; init; } = [];
    public bool AllowAnyMethod { get; init; }
    public IReadOnlyCollection<string> AllowedMethods { get; init; } = [];
}
