using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

namespace Modsen.Portal.Finance.Controllers.Contracts.Output;

public sealed class ProjectRateHistoryOutput
{
    public int UserId { get; init; }
    public UserOutput? User { get; init; }
    public int ProjectId { get; init; }
    public ProjectOutput? Project { get; init; }
    public required IReadOnlyList<ProjectRateShortOutput> ProjectRates { get; init; }

    public bool CanRateBeAddedByCurrentUser { get; init; }

    internal static ProjectRateHistoryOutput Of(ProjectRateHistoryCoreDto source)
    {
        return new ProjectRateHistoryOutput
        {
            UserId = source.UserId,
            User = UserOutput.Of(source.User),
            ProjectId = source.ProjectId,
            Project = ProjectOutput.Of(source.Project),
            ProjectRates = source.ProjectRates.Select(x => ProjectRateShortOutput.Of(x)).ToList(),
            CanRateBeAddedByCurrentUser = source.CanRateBeAddedByCurrentUser
        };
    }
}
