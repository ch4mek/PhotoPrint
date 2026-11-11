using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

public class FinanceProjectCoreDto
{
    public int Id { get; set; }
    public required string ProjectName { get; set; }
    public required int ClientId { get; set; }
    public required string ClientName { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }

    internal static FinanceProjectCoreDto? Of(FinanceProjectEntity? source)
    {
        if (source is null)
        {
            return null;
        }

        return new FinanceProjectCoreDto
        {
            Id = source.Id,
            ProjectName = source.ProjectName,
            ClientId = source.ClientId,
            ClientName = source.Client?.Name ?? "",
            Color = source.Color,
            IsActive = source.IsActive
        };
    }
}
