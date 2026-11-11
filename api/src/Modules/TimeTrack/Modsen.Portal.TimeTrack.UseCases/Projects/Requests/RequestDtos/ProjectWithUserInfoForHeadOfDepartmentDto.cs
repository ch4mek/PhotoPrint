using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

public class ProjectWithUserInfoForHeadOfDepartmentDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Color { get; set; }

    public int ClientId { get; set; }

    public required string ClientName { get; set; }

    public bool IsPublic { get; set; }

    public bool IsBillable { get; set; }

    public bool IsActive { get; set; }

    public required IReadOnlyCollection<ProjectUserInfoDto> ProjectUsers { get; set; } = [];

    public static ProjectWithUserInfoForHeadOfDepartmentDto? Of(OdpProject? source)
    {
        if (source == null)
        {
            return null;
        }

        return new ProjectWithUserInfoForHeadOfDepartmentDto
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            IsPublic = source.IsPublic,
            IsBillable = source.IsBillable,
            IsActive = source.IsActive,
            ProjectUsers = source.ProjectUsers.Select(x => new ProjectUserInfoDto
            {
                Id = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DepartmentId = x.DepartmentId
            }).ToList()
        };
    }
}
