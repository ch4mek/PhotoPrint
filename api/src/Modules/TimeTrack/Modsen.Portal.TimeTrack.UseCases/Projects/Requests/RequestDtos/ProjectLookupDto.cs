using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects.LookupProjects.Lookup;
using Modsen.Portal.TimeTrack.Domain;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class ProjectLookupDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string ClientName { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsCurrentUserAbleToSeeBillableTrackedTime { get; set; }
    public string? Color { get; set; }

    public static ProjectLookupDto? Of(ProjectLookup? source, bool isCurrentUserAbleToSeeBillableTrackedTime)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectLookupDto
        {
            Id = source.Id,
            ClientName = source.ClientName,
            IsBillable = source.IsBillable,
            Name = source.Name,
            IsCurrentUserAbleToSeeBillableTrackedTime = isCurrentUserAbleToSeeBillableTrackedTime,
            Color = source.Color
        };
    }

    public static ProjectLookupDto? Of(ProjectLookupDto? source, bool isCurrentUserAbleToSeeBillableTrackedTime)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectLookupDto
        {
            Id = source.Id,
            ClientName = source.ClientName,
            IsBillable = source.IsBillable,
            Name = source.Name,
            IsCurrentUserAbleToSeeBillableTrackedTime = isCurrentUserAbleToSeeBillableTrackedTime,
            Color = source.Color
        };
    }

    public static ProjectLookupDto? Of(ProjectsLookupDbQueryModel? source, bool isCurrentUserAbleToSeeBillableTrackedTime)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectLookupDto
        {
            Id = source.Id,
            ClientName = source.ClientName,
            IsBillable = source.IsBillable,
            Name = source.Name,
            IsCurrentUserAbleToSeeBillableTrackedTime = isCurrentUserAbleToSeeBillableTrackedTime,
            Color = source.Color
        };
    }
}
