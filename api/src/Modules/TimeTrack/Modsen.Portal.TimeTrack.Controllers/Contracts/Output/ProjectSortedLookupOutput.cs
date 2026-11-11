using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectSortedLookupOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required string Type { get; set; }
    public required string Client { get; set; }

    internal static ProjectSortedLookupOutput Of(ProjectSortedLookupDto source)
    {
        return new ProjectSortedLookupOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            Type = source.Type,
            Client = source.ClientName,
        };
    }
}
