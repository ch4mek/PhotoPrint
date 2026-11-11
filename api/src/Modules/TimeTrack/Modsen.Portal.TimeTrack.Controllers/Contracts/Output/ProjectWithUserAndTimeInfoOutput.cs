using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class ProjectWithUserAndTimeInfoOutput
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Color { get; set; }

    public int ClientId { get; set; }

    public required string ClientName { get; set; }

    public int? FactTrackedTime { get; set; }
    public int? BillableTrackedTime { get; set; }

    public bool IsPublic { get; set; }

    public bool IsBillable { get; set; }

    public bool IsActive { get; set; }

    public IReadOnlyCollection<ProjectUserInfoOutput> ProjectUsers { get; set; } = [];

    public static ProjectWithUserAndTimeInfoOutput? Of(ProjectWithUserInfoAndTimeDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectWithUserAndTimeInfoOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            FactTrackedTime = source.FactTrackedTime,
            BillableTrackedTime = source.BillableTrackedTime,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            ProjectUsers = source.ProjectUsers
                .Select(x => ProjectUserInfoOutput.Of(x)!)
                .ToList()
        };
    }

    public static ProjectWithUserAndTimeInfoOutput? Of(ProjectWithUserInfoForHeadOfDepartmentDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectWithUserAndTimeInfoOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            FactTrackedTime = null,
            BillableTrackedTime = null,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            ProjectUsers = source.ProjectUsers
                .Select(x => ProjectUserInfoOutput.Of(x)!)
                .ToList()
        };
    }
}
