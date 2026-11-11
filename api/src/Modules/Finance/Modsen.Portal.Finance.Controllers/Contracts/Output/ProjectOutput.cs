using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ProjectOutput
{
    public int Id { get; init; }
    public required string ProjectName { get; init; }
    public required int ClientId { get; init; }
    public string? Color { get; init; }

    internal static ProjectOutput Of(ProjectCoreDto source)
    {
        return new ProjectOutput
        {
            Id = source.Id,
            ProjectName = source.ProjectName,
            ClientId = source.ClientId,
            Color = source.Color
        };
    }
}
