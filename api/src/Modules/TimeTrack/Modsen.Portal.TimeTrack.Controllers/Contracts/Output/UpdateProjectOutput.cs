using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Contracts.Output;

public class UpdateProjectOutput
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public required int ClientId { get; set; }
    public required string ClientName { get; set; }
    public required bool IsPublic { get; set; }
    public required bool IsActive { get; set; }
    public required bool IsBillable { get; set; }
    public required bool IsCreatedByCurrentUser { get; set; }
    public required int TrackedTime { get; set; }
    public int? ManagerId { get; set; }

    public static UpdateProjectOutput? Of(UpdateProjectDto? source)
    {
        if (source == null)
        {
            return null;
        }

        return new UpdateProjectOutput
        {
            Id = source.Id,
            Name = source.Name,
            Color = source.Color,
            ClientId = source.ClientId,
            ClientName = source.ClientName,
            IsPublic = source.IsPublic,
            IsActive = source.IsActive,
            IsBillable = source.IsBillable,
            IsCreatedByCurrentUser = source.IsCreatedByCurrentUser,
            ManagerId = source.ManagerId,
            TrackedTime = source.TrackedTime
        };
    }
}
