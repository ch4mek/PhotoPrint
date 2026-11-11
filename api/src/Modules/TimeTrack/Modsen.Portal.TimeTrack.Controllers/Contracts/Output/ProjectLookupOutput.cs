using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectLookupOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string ClientName { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsCurrentUserAbleToSeeBillableTrackedTime { get; set; }
    public string? Color { get; set; }

    public static ProjectLookupOutput? Of(ProjectLookupDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectLookupOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientName = source.ClientName,
            IsCurrentUserAbleToSeeBillableTrackedTime = source.IsCurrentUserAbleToSeeBillableTrackedTime,
            IsBillable = source.IsBillable
        };
    }
}
