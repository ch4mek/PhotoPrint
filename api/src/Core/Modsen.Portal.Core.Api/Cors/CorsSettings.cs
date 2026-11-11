namespace Modsen.Portal.Core.Api.Cors;

public sealed class CorsSettings
{
    public bool UseCors { get; init; }
    public string? UsePolicyWithName { get; init; }
    public IReadOnlyCollection<CorsPolicySettings> Policies { get; init; } = [];
}
