using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal class ProjectCoreDto
{
    public int Id { get; init; }
    public required string ProjectName { get; init; }
    public required int ClientId { get; init; }
    public string? Color { get; init; }

    public static ProjectCoreDto Of(FinanceProjectEntity source)
    {
        return new ProjectCoreDto
        {
            Id = source.Id,
            ProjectName = source.ProjectName,
            ClientId = source.ClientId,
            Color = source.Color
        };
    }
}
