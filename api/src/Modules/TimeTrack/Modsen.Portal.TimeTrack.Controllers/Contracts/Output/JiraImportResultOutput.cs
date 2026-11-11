using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public sealed class JiraImportResultOutput
{
    public int Created { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
    public int Total { get; set; }
    public IReadOnlyCollection<string> ExceptionMessages { get; init; } = [];

    internal static JiraImportResultOutput Of(JiraImportResultDto source)
    {
        return new JiraImportResultOutput
        {
            Created = source.Created,
            Updated = source.Updated,
            Skipped = source.Skipped,
            Total = source.Total,
            ExceptionMessages = source.ExceptionMessages
        };
    }
}
